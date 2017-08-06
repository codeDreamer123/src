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
namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConsumeConfigurators;
    using Internals.Extensions;
    using Saga;
    using Saga.SubscriptionConfigurators;
    using StructureMap;
    using StructureMapIntegration;


    public static class StructureMapExtensions
    {
        /// <summary>
        /// Specify that the service bus should load its subscribers from the container passed as an argument.
        /// </summary>
        /// <param name="configurator">The configurator the extension method works on.</param>
        /// <param name="container">The StructureMap container.</param>
        public static void LoadFrom(this IReceiveEndpointConfigurator configurator, IContainer container)
        {
            IList<Type> concreteTypes = FindTypes<IConsumer>(container, x => !x.HasInterface<ISaga>());
            if (concreteTypes.Count > 0)
            {
                foreach (Type concreteType in concreteTypes)
                    ConsumerConfiguratorCache.Configure(concreteType, configurator, container);
            }

            IList<Type> sagaTypes = FindTypes<ISaga>(container, x => true);
            if (sagaTypes.Count > 0)
            {
                foreach (Type sagaType in sagaTypes)
                    SagaConfiguratorCache.Configure(sagaType, configurator, container);
            }
        }

        /// <summary>
        /// Specify that the service bus should load its subscribers from the container passed as an argument.
        /// </summary>
        /// <param name="configurator">The configurator the extension method works on.</param>
        /// <param name="context"></param>
        public static void LoadFrom(this IReceiveEndpointConfigurator configurator, IContext context)
        {
            var container = context.GetInstance<IContainer>();

            configurator.LoadFrom(container);
        }

        public static void Consumer<T>(this IReceiveEndpointConfigurator configurator, IContainer container, Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            var consumerFactory = new StructureMapConsumerFactory<T>(container);

            configurator.Consumer(consumerFactory, configure);
        }

        public static void Saga<T>(this IReceiveEndpointConfigurator configurator, IContainer container, Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            var sagaRepository = container.GetInstance<ISagaRepository<T>>();

            var structureMapSagaRepository = new StructureMapSagaRepository<T>(sagaRepository, container);

            configurator.Saga(structureMapSagaRepository, configure);
        }

        static IList<Type> FindTypes<T>(IContainer container, Func<Type, bool> filter)
        {
            return container
                .Model
                .PluginTypes
                .Where(x => x.PluginType.HasInterface<T>())
                .Select(i => i.PluginType)
                .Concat(container.Model.InstancesOf<T>().Select(x => x.ReturnedType))
                .Where(filter)
                .Distinct()
                .ToList();
        }
    }
}