﻿// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Observers;


    /// <summary>
    /// Uses the message redelivery mechanism, if available, to delay a retry without blocking message delivery
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RedeliveryRetryFilter<T> :
        IFilter<ConsumeContext<T>>
        where T : class
    {
        readonly RetryObservable _observers;
        readonly IRetryPolicy _retryPolicy;

        public RedeliveryRetryFilter(IRetryPolicy retryPolicy, RetryObservable observers)
        {
            _retryPolicy = retryPolicy;
            _observers = observers;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("retry");
            scope.Add("type", "redelivery");

            _retryPolicy.Probe(scope);
        }

        [DebuggerNonUserCode]
        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            RetryPolicyContext<ConsumeContext<T>> policyContext = _retryPolicy.CreatePolicyContext(context);

            await _observers.PostCreate(policyContext).ConfigureAwait(false);

            try
            {
                await next.Send(policyContext.Context).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                RetryContext<ConsumeContext<T>> payloadRetryContext;
                if (context.TryGetPayload(out payloadRetryContext))
                {
                    await policyContext.RetryFaulted(exception).ConfigureAwait(false);

                    await _observers.RetryFault(payloadRetryContext).ConfigureAwait(false);

                    throw;
                }

                RetryContext genericRetryContext;
                if (context.TryGetPayload(out genericRetryContext))
                {
                    await policyContext.RetryFaulted(exception).ConfigureAwait(false);

                    await _observers.RetryFault(genericRetryContext).ConfigureAwait(false);

                    throw;
                }

                RetryContext<ConsumeContext<T>> retryContext;
                if (!policyContext.CanRetry(exception, out retryContext))
                {
                    await retryContext.RetryFaulted(exception).ConfigureAwait(false);

                    await _observers.RetryFault(retryContext).ConfigureAwait(false);

                    context.GetOrAddPayload(() => retryContext);
                    throw;
                }

                int previousDeliveryCount = context.Headers.Get(MessageHeaders.RedeliveryCount, default(int?)) ?? 0;
                for (int retryIndex = 0; retryIndex < previousDeliveryCount; retryIndex++)
                {
                    if (!retryContext.CanRetry(exception, out retryContext))
                    {
                        await retryContext.RetryFaulted(exception).ConfigureAwait(false);

                        await _observers.RetryFault(retryContext).ConfigureAwait(false);

                        context.GetOrAddPayload(() => retryContext);
                        throw;
                    }
                }

                await _observers.PostFault(retryContext).ConfigureAwait(false);

                try
                {
                    MessageRedeliveryContext redeliveryContext;
                    if (!context.TryGetPayload(out redeliveryContext))
                        throw new ContextException("The message redelivery context was not available to delay the message", exception);

                    var delay = retryContext.Delay ?? TimeSpan.Zero;

                    await redeliveryContext.ScheduleRedelivery(delay).ConfigureAwait(false);
                }
                catch (Exception redeliveryException)
                {
                    throw new ContextException("The message delivery could not be rescheduled", new AggregateException(redeliveryException, exception));
                }
            }
        }
    }
}