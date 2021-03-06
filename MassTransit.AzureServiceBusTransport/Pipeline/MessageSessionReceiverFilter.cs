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
namespace MassTransit.AzureServiceBusTransport.Pipeline
{
    using System.Threading.Tasks;
    using Events;
    using GreenPipes;
    using Logging;
    using Transport;
    using Util;


    /// <summary>
    /// Creates a message session receiver
    /// </summary>
    public class MessageSessionReceiverFilter :
        IFilter<NamespaceContext>
    {
        static readonly ILog _log = Logger.Get<MessageReceiverFilter>();
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly ISendEndpointProvider _sendEndpointProvider;
        readonly IPublishEndpointProvider _publishEndpointProvider;

        public MessageSessionReceiverFilter(IPipe<ReceiveContext> receivePipe, ISendEndpointProvider sendEndpointProvider, IPublishEndpointProvider publishEndpointProvider)
        {
            _receivePipe = receivePipe;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpointProvider = publishEndpointProvider;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        async Task IFilter<NamespaceContext>.Send(NamespaceContext context, IPipe<NamespaceContext> next)
        {
            var clientContext = context.GetPayload<ClientContext>();

            var clientSettings = context.GetPayload<ClientSettings>();

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Creating message receiver for {0}", clientContext.InputAddress);

            using (var scope = context.CreateScope($"{TypeMetadataCache<MessageReceiverFilter>.ShortName} - {clientContext.InputAddress}"))
            {
                var receiver = new SessionReceiver(clientContext, _receivePipe, clientSettings, scope, _sendEndpointProvider, _publishEndpointProvider);

                await receiver.Start(context).ConfigureAwait(false);

                await scope.Ready.ConfigureAwait(false);

                await context.Ready(new ReceiveTransportReadyEvent(clientContext.InputAddress)).ConfigureAwait(false);

                scope.SetReady();

                try
                {
                    await scope.Completed.ConfigureAwait(false);
                }
                finally
                {
                    var metrics = receiver.GetDeliveryMetrics();

                    await context.Completed(new ReceiveTransportCompletedEvent(clientContext.InputAddress, metrics)).ConfigureAwait(false);

                    if (_log.IsDebugEnabled)
                    {
                        _log.DebugFormat("Consumer {0}: {1} received, {2} concurrent", clientContext.InputAddress,
                            metrics.DeliveryCount,
                            metrics.ConcurrentDeliveryCount);
                    }
                }
            }

            await next.Send(context).ConfigureAwait(false);
        }
    }
}