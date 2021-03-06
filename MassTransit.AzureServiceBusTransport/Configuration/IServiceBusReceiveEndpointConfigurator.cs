﻿// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport
{
    /// <summary>
    /// Configure an Azure Service Bus receive endpoint
    /// </summary>
    public interface IServiceBusReceiveEndpointConfigurator :
        IReceiveEndpointConfigurator,
        IServiceBusQueueEndpointConfigurator
    {
        /// <summary>
        /// The host on which the endpoint is being configured
        /// </summary>
        IServiceBusHost Host { get; }

        /// <summary>
        /// If true, adds subscriptions for the message types to the related topics.
        /// </summary>
        bool SubscribeMessageTopics { set; }

        /// <summary>
        /// If true, on shutdown, the subscriptions added are removed. This is used to avoid auto-delete
        /// queues from creating abandoned subscriptions on the topic, resulting in a quota overflow.
        /// </summary>
        bool RemoveSubscriptions { set; }
    }
}