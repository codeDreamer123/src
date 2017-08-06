// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.PipeConfigurators
{
    using System.Collections.Generic;
    using GreenPipes;
    using Internals.Extensions;
    using Pipeline.Filters;
    using Util;


    public class ConsumerMessageSplitFilterSpecification<TConsumer, TMessage> :
        IPipeSpecification<ConsumerConsumeContext<TConsumer, TMessage>>
        where TMessage : class
        where TConsumer : class
    {
        readonly IPipeSpecification<ConsumeContext<TMessage>> _specification;

        public ConsumerMessageSplitFilterSpecification(IPipeSpecification<ConsumeContext<TMessage>> specification)
        {
            _specification = specification;
        }

        public void Apply(IPipeBuilder<ConsumerConsumeContext<TConsumer, TMessage>> builder)
        {
            _specification.Apply(new BuilderProxy(builder));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (!typeof(TConsumer).HasInterface<IConsumer<TMessage>>())
                yield return this.Failure("MessageType", $"is not consumed by {TypeMetadataCache<TConsumer>.ShortName}");

            foreach (var validationResult in _specification.Validate())
            {
                yield return validationResult;
            }
        }


        class BuilderProxy :
            IPipeBuilder<ConsumeContext<TMessage>>
        {
            readonly IPipeBuilder<ConsumerConsumeContext<TConsumer, TMessage>> _builder;

            public BuilderProxy(IPipeBuilder<ConsumerConsumeContext<TConsumer, TMessage>> builder)
            {
                _builder = builder;
            }

            public void AddFilter(IFilter<ConsumeContext<TMessage>> filter)
            {
                _builder.AddFilter(new MessageSplitFilter<TConsumer, TMessage>(filter));
            }
        }
    }
}