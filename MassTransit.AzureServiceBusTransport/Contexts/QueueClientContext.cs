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
namespace MassTransit.AzureServiceBusTransport.Contexts
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.ServiceBus.Messaging;


    public class QueueClientContext :
        ClientContext
    {
        readonly QueueClient _client;

        public QueueClientContext(QueueClient client, Uri inputAddress)
        {
            _client = client;
            InputAddress = inputAddress;
        }

        public Task RegisterSessionHandlerFactoryAsync(IMessageSessionAsyncHandlerFactory factory, SessionHandlerOptions options)
        {
            return _client.RegisterSessionHandlerFactoryAsync(factory, options);
        }

        public void OnMessageAsync(Func<BrokeredMessage, Task> callback, OnMessageOptions options)
        {
            _client.OnMessageAsync(callback, options);
        }

        public Uri InputAddress { get; }
    }
}