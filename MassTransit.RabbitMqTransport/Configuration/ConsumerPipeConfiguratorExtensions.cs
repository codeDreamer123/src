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
namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using GreenPipes;
    using Pipeline.Pipes;
    using RabbitMqTransport;
    using RabbitMqTransport.Specifications;
    using RabbitMqTransport.Topology;
    using Util;


    public static class ConsumerPipeConfiguratorExtensions
    {
        /// <summary>
        /// Adds a RabbitMQ Basic Consumer to the pipeline
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="pipe"></param>
        /// <param name="settings"></param>
        /// <param name="receiveObserver"></param>
        /// <param name="transportObserver"></param>
        /// <param name="exchangeBindings"></param>
        /// <param name="supervisor"></param>
        /// <param name="managementPipe"></param>
        /// <param name="sendEndpointProvider"></param>
        /// <param name="publishEndpointProvider"></param>
        /// <param name="host"></param>
        public static void RabbitMqConsumer(this IPipeConfigurator<ConnectionContext> configurator, IPipe<ReceiveContext> pipe, ReceiveSettings settings, IReceiveObserver receiveObserver, IReceiveTransportObserver transportObserver, IEnumerable<ExchangeBindingSettings> exchangeBindings, ITaskSupervisor supervisor, IManagementPipe managementPipe, ISendEndpointProvider sendEndpointProvider, IPublishEndpointProvider publishEndpointProvider, IRabbitMqHost host)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var pipeBuilderConfigurator = new RabbitMqConsumerPipeSpecification(pipe, settings, receiveObserver, transportObserver, exchangeBindings,
                supervisor, managementPipe, sendEndpointProvider, publishEndpointProvider, host);

            configurator.AddPipeSpecification(pipeBuilderConfigurator);
        }
    }
}