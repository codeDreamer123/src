﻿// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.StructureMapIntegration
{
    using System;
    using System.Collections.Concurrent;
    using Saga;
    using StructureMap;


    public static class SagaConfiguratorCache
    {
        static CachedConfigurator GetOrAdd(Type type)
        {
            return Cached.Instance.GetOrAdd(type, _ =>
                (CachedConfigurator)Activator.CreateInstance(typeof(CachedConfigurator<>).MakeGenericType(type)));
        }

        public static void Configure(Type sagaType, IReceiveEndpointConfigurator configurator, IContainer container)
        {
            GetOrAdd(sagaType).Configure(configurator, container);
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<Type, CachedConfigurator> Instance =
                new ConcurrentDictionary<Type, CachedConfigurator>();
        }


        interface CachedConfigurator
        {
            void Configure(IReceiveEndpointConfigurator configurator, IContainer container);
        }


        class CachedConfigurator<T> :
            CachedConfigurator
            where T : class, ISaga
        {
            public void Configure(IReceiveEndpointConfigurator configurator, IContainer container)
            {
                var sagaRepository = container.GetInstance<ISagaRepository<T>>();

                var structureMapSagaRepository = new StructureMapSagaRepository<T>(sagaRepository, container);

                configurator.Saga(structureMapSagaRepository);
            }
        }
    }
}