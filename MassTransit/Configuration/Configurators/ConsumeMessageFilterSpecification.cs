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
namespace MassTransit.Configurators
{
    using System;
    using System.Linq;
    using Util.Scanning;


    public class ConsumeMessageFilterSpecification :
        IMessageFilterConfigurator
    {
        readonly CompositeFilter<ConsumeContext> _filter;

        public ConsumeMessageFilterSpecification()
        {
            _filter = new CompositeFilter<ConsumeContext>();
        }

        public CompositeFilter<ConsumeContext> Filter => _filter;

        void IMessageFilterConfigurator.Include(params Type[] messageTypes) =>
            _filter.Includes += message => Match(message, messageTypes);

        void IMessageFilterConfigurator.Include<T>() =>
            _filter.Includes += message => Match(message, typeof(T));

        void IMessageFilterConfigurator.Include<T>(Func<T, bool> filter) =>
            _filter.Includes += message => Match(message, filter);

        void IMessageFilterConfigurator.Exclude(params Type[] messageTypes) =>
            _filter.Excludes += message => Match(message, messageTypes);

        void IMessageFilterConfigurator.Exclude<T>() =>
            _filter.Excludes += message => Match(message, typeof(T));

        void IMessageFilterConfigurator.Exclude<T>(Func<T, bool> filter) =>
            _filter.Excludes += message => Match(message, filter);

        static bool Match(ConsumeContext context, params Type[] messageTypes)
        {
            return messageTypes.Any(context.HasMessageType);
        }

        static bool Match<T>(ConsumeContext context, Func<T, bool> filter)
            where T : class
        {
            ConsumeContext<T> consumeContext;
            return context.TryGetMessage(out consumeContext) && filter(consumeContext.Message);
        }
    }
}