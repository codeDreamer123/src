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
namespace MassTransit.Saga.SubscriptionConfigurators
{
    using System.Collections.Generic;
    using GreenPipes;


    public class SagaMessageConfigurator<TSaga, TMessage> :
        ISagaMessageConfigurator<TSaga, TMessage>,
        ISagaMessageConfigurator<TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly ISagaConfigurator<TSaga> _configurator;

        public SagaMessageConfigurator(ISagaConfigurator<TSaga> configurator)
        {
            _configurator = configurator;
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext<TMessage>> specification)
        {
            _configurator.AddPipeSpecification(new ConsumeContextSpecificationProxy(specification));
        }

        public void AddPipeSpecification(IPipeSpecification<SagaConsumeContext<TSaga, TMessage>> specification)
        {
            _configurator.AddPipeSpecification(new SagaConsumeContextSpecificationProxy(specification));
        }


        class ConsumeContextSpecificationProxy :
            IPipeSpecification<SagaConsumeContext<TSaga>>
        {
            readonly IPipeSpecification<ConsumeContext<TMessage>> _specification;

            public ConsumeContextSpecificationProxy(IPipeSpecification<ConsumeContext<TMessage>> specification)
            {
                _specification = specification;
            }

            public void Apply(IPipeBuilder<SagaConsumeContext<TSaga>> builder)
            {
                var messageBuilder = builder as IPipeBuilder<ConsumeContext<TMessage>>;

                if (messageBuilder != null)
                    _specification.Apply(messageBuilder);
            }

            public IEnumerable<ValidationResult> Validate()
            {
                return _specification.Validate();
            }
        }


        class SagaConsumeContextSpecificationProxy :
            IPipeSpecification<SagaConsumeContext<TSaga>>
        {
            readonly IPipeSpecification<SagaConsumeContext<TSaga, TMessage>> _specification;

            public SagaConsumeContextSpecificationProxy(IPipeSpecification<SagaConsumeContext<TSaga, TMessage>> specification)
            {
                _specification = specification;
            }

            public void Apply(IPipeBuilder<SagaConsumeContext<TSaga>> builder)
            {
                var messageBuilder = builder as IPipeBuilder<SagaConsumeContext<TSaga, TMessage>>;

                if (messageBuilder != null)
                    _specification.Apply(messageBuilder);
            }

            public IEnumerable<ValidationResult> Validate()
            {
                return _specification.Validate();
            }
        }
    }
}