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
namespace MassTransit.Pipeline.Filters
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using GreenPipes;
    using Pipes;
    using Util;


    /// <summary>
    /// Splits a context item off the pipe and carries it out-of-band to be merged
    /// once the next filter has completed
    /// </summary>
    /// <typeparam name="TConsumer"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class MessageSplitFilter<TConsumer, TMessage> :
        IFilter<ConsumerConsumeContext<TConsumer, TMessage>>
        where TMessage : class
        where TConsumer : class
    {
        readonly IFilter<ConsumeContext<TMessage>> _next;

        public MessageSplitFilter(IFilter<ConsumeContext<TMessage>> next)
        {
            _next = next;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("split");
            scope.Set(new
            {
                MessageType = TypeMetadataCache<TMessage>.ShortName
            });

            _next.Probe(scope);
        }

        [DebuggerNonUserCode]
        public Task Send(ConsumerConsumeContext<TConsumer, TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
        {
            var mergePipe = new ConsumerMessageMergePipe<TConsumer, TMessage>(next, context);

            return _next.Send(context, mergePipe);
        }
    }
}