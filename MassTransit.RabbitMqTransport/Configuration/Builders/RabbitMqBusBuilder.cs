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
namespace MassTransit.RabbitMqTransport.Builders
{
    using System;
    using BusConfigurators;
    using Configurators;
    using MassTransit.Builders;
    using MassTransit.Pipeline;
    using Topology;
    using Transport;
    using Transports;


    public class RabbitMqBusBuilder :
        BusBuilder
    {
        readonly RabbitMqReceiveEndpointSpecification _busEndpointSpecification;
        readonly BusHostCollection<RabbitMqHost> _hosts;

        public RabbitMqBusBuilder(BusHostCollection<RabbitMqHost> hosts, IConsumePipeFactory consumePipeFactory, ISendPipeFactory sendPipeFactory,
            IPublishPipeFactory publishPipeFactory, RabbitMqReceiveSettings busSettings)
            : base(consumePipeFactory, sendPipeFactory, publishPipeFactory, hosts)
        {
            _hosts = hosts;

            _busEndpointSpecification = new RabbitMqReceiveEndpointSpecification(_hosts[0], busSettings, ConsumePipe);

            foreach (var host in hosts.Hosts)
            {
                var factory = new RabbitMqReceiveEndpointFactory(this, host);

                host.ReceiveEndpointFactory = factory;
            }
        }

        public BusHostCollection<RabbitMqHost> Hosts => _hosts;

        public override IPublishEndpointProvider PublishEndpointProvider => _busEndpointSpecification.PublishEndpointProvider;

        public override ISendEndpointProvider SendEndpointProvider => _busEndpointSpecification.SendEndpointProvider;

        protected override void PreBuild()
        {
            _busEndpointSpecification.Apply(this);
        }

        protected override Uri GetInputAddress()
        {
            return _busEndpointSpecification.InputAddress;
        }

        protected override IConsumePipe GetConsumePipe()
        {
            return CreateConsumePipe();
        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return new RabbitMqSendTransportProvider(_hosts);
        }
    }
}