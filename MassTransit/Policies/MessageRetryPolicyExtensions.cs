// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Policies
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;


    public static class MessageRetryPolicyExtensions
    {
        public static async Task Retry<T>(this IRetryPolicy retryPolicy, ConsumeContext<T> context,
            Func<ConsumeContext<T>, Task> retryMethod)
            where T : class
        {
            RetryPolicyContext<ConsumeContext<T>> policyContext = retryPolicy.CreatePolicyContext(context);

            try
            {
                await retryMethod(policyContext.Context).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                RetryContext<ConsumeContext<T>> retryContext;
                if (context.TryGetPayload(out retryContext))
                    throw;

                if (!policyContext.CanRetry(exception, out retryContext))
                {
                    context.GetOrAddPayload(() => retryContext);
                    throw;
                }

                await Attempt(retryContext, retryMethod).ConfigureAwait(false);
            }
        }

        static async Task Attempt<T>(RetryContext<ConsumeContext<T>> retryContext, Func<ConsumeContext<T>, Task> retryMethod)
            where T : class
        {
            try
            {
                await retryMethod(retryContext.Context).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                RetryContext<ConsumeContext<T>> nextRetryContext;
                if (retryContext.Context.TryGetPayload(out nextRetryContext))
                    throw;

                if (!retryContext.CanRetry(exception, out nextRetryContext))
                {
                    retryContext.Context.GetOrAddPayload(() => nextRetryContext);
                    throw;
                }

                if (nextRetryContext.Delay.HasValue)
                    await Task.Delay(nextRetryContext.Delay.Value).ConfigureAwait(false);

                await Attempt(nextRetryContext, retryMethod).ConfigureAwait(false);
            }
        }
    }
}