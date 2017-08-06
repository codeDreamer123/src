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
namespace MassTransit.AutofacIntegration
{
    using System.Threading.Tasks;
    using Autofac;
    using Courier;
    using Courier.Hosts;
    using GreenPipes;
    using Logging;
    using Util;


    /// <summary>
    /// A factory to create an activity from Autofac, that manages the lifetime scope of the activity
    /// </summary>
    /// <typeparam name="TActivity"></typeparam>
    /// <typeparam name="TLog"></typeparam>
    public class AutofacCompensateActivityFactory<TActivity, TLog> :
        CompensateActivityFactory<TActivity, TLog>
        where TActivity : class, CompensateActivity<TLog>
        where TLog : class
    {
        static readonly ILog _log = Logger.Get<AutofacCompensateActivityFactory<TActivity, TLog>>();
        readonly ILifetimeScope _lifetimeScope;

        public AutofacCompensateActivityFactory(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public async Task<ResultContext<CompensationResult>> Compensate(CompensateContext<TLog> context,
            IRequestPipe<CompensateActivityContext<TActivity, TLog>, CompensationResult> next)
        {
            using (var innerScope = _lifetimeScope.BeginLifetimeScope(x => ConfigureScope(x, context)))
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("CompensateActivityFactory: Compensating: {0}", TypeMetadataCache<TActivity>.ShortName);

                var activity = innerScope.Resolve<TActivity>(TypedParameter.From(context.Log));

                CompensateActivityContext<TActivity, TLog> activityContext = new HostCompensateActivityContext<TActivity, TLog>(activity, context);

                var consumerLifetimeScope = innerScope;
                activityContext.GetOrAddPayload(() => consumerLifetimeScope);

                return await next.Send(activityContext).ConfigureAwait(false);
            }
        }

        static void ConfigureScope(ContainerBuilder containerBuilder, CompensateContext<TLog> compensateContext)
        {
            containerBuilder.RegisterInstance(compensateContext.ConsumeContext)
                .ExternallyOwned();
        }
    }
}