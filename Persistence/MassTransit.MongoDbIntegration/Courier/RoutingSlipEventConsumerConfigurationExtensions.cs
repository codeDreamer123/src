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
    using Courier.Contracts;
    using GreenPipes;
    using GreenPipes.Partitioning;
    using MongoDbIntegration.Courier;
    using MongoDbIntegration.Courier.Consumers;


    public static class RoutingSlipEventConsumerConfigurationExtensions
    {
        /// <summary>
        /// Configure the routing slip event consumers on a receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="persister">The event persister used to save the events</param>
        public static void RoutingSlipEventConsumers(this IReceiveEndpointConfigurator configurator, IRoutingSlipEventPersister persister)
        {
            configurator.Consumer(() => new RoutingSlipCompletedConsumer(persister));
            configurator.Consumer(() => new RoutingSlipFaultedConsumer(persister));
            configurator.Consumer(() => new RoutingSlipCompensationFailedConsumer(persister));
            configurator.Consumer(() => new RoutingSlipRevisedConsumer(persister));
            configurator.Consumer(() => new RoutingSlipTerminatedConsumer(persister));
        }

        /// <summary>
        /// Configure the routing slip event consumers on a receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="persister">The event persister used to save the events</param>
        /// <param name="partitioner">Use a partitioner to reduce duplicate key errors</param>
        public static void RoutingSlipEventConsumers(this IReceiveEndpointConfigurator configurator, IRoutingSlipEventPersister persister, IPartitioner partitioner)
        {
            configurator.Consumer(() => new RoutingSlipCompletedConsumer(persister),
                x => x.Message<RoutingSlipCompleted>(y => y.UsePartitioner(partitioner, p => p.Message.TrackingNumber)));
            configurator.Consumer(() => new RoutingSlipFaultedConsumer(persister),
                x => x.Message<RoutingSlipFaulted>(y => y.UsePartitioner(partitioner, p => p.Message.TrackingNumber)));
            configurator.Consumer(() => new RoutingSlipCompensationFailedConsumer(persister),
                x => x.Message<RoutingSlipCompensationFailed>(y => y.UsePartitioner(partitioner, p => p.Message.TrackingNumber)));
            configurator.Consumer(() => new RoutingSlipRevisedConsumer(persister),
                x => x.Message<RoutingSlipRevised>(y => y.UsePartitioner(partitioner, p => p.Message.TrackingNumber)));
            configurator.Consumer(() => new RoutingSlipTerminatedConsumer(persister),
                x => x.Message<RoutingSlipTerminated>(y => y.UsePartitioner(partitioner, p => p.Message.TrackingNumber)));
        }

        /// <summary>
        /// Configure the routing slip activity event consumers on a receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="persister"></param>
        public static void RoutingSlipActivityEventConsumers(this IReceiveEndpointConfigurator configurator, IRoutingSlipEventPersister persister)
        {
            configurator.Consumer(() => new RoutingSlipActivityCompensatedConsumer(persister));
            configurator.Consumer(() => new RoutingSlipActivityCompletedConsumer(persister));
            configurator.Consumer(() => new RoutingSlipActivityFaultedConsumer(persister));
            configurator.Consumer(() => new RoutingSlipActivityCompensationFailedConsumer(persister));
        }

        /// <summary>
        /// Configure the routing slip activity event consumers on a receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="persister"></param>
        /// <param name="partitioner">Use a partitioner to reduce duplicate key errors</param>
        public static void RoutingSlipActivityEventConsumers(this IReceiveEndpointConfigurator configurator, IRoutingSlipEventPersister persister, IPartitioner partitioner)
        {
            configurator.Consumer(() => new RoutingSlipActivityCompensatedConsumer(persister),
                x => x.Message<RoutingSlipActivityCompensated>(y => y.UsePartitioner(partitioner, p => p.Message.TrackingNumber)));
            configurator.Consumer(() => new RoutingSlipActivityCompletedConsumer(persister),
                x => x.Message<RoutingSlipActivityCompleted>(y => y.UsePartitioner(partitioner, p => p.Message.TrackingNumber)));
            configurator.Consumer(() => new RoutingSlipActivityFaultedConsumer(persister),
                x => x.Message<RoutingSlipActivityFaulted>(y => y.UsePartitioner(partitioner, p => p.Message.TrackingNumber)));
            configurator.Consumer(() => new RoutingSlipActivityCompensationFailedConsumer(persister),
                x => x.Message<RoutingSlipActivityCompensationFailed>(y => y.UsePartitioner(partitioner, p => p.Message.TrackingNumber)));
        }
    }
}