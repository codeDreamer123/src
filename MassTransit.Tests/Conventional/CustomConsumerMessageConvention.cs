// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.Conventional
{
    using System.Collections.Generic;
    using System.Linq;


    class CustomConsumerMessageConvention<T> :
        IConsumerMessageConvention
        where T : class
    {
        public IEnumerable<IMessageInterfaceType> GetMessageTypes()
        {
            if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(IHandler<>))
            {
                var interfaceType = new CustomConsumerInterfaceType(typeof(T).GetGenericArguments()[0], typeof(T));
                if (interfaceType.MessageType.IsValueType == false && interfaceType.MessageType != typeof(string))
                    yield return interfaceType;
            }

            IEnumerable<CustomConsumerInterfaceType> types = typeof(T).GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(IHandler<>))
                .Select(x => new CustomConsumerInterfaceType(x.GetGenericArguments()[0], typeof(T)))
                .Where(x => x.MessageType.IsValueType == false && x.MessageType != typeof(string));

            foreach (CustomConsumerInterfaceType type in types)
                yield return type;
        }
    }
}