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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Collections.Generic;
    using Courier;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public abstract class RabbitMqActivityTestFixture :
        RabbitMqTestFixture
    {
        protected RabbitMqActivityTestFixture()
        {
            ActivityTestContexts = new Dictionary<Type, ActivityTestContext>();

            RabbitMqTestHarness.PreCreateBus += PreCreateBus;
        }

        protected IDictionary<Type, ActivityTestContext> ActivityTestContexts { get; private set; }

        void PreCreateBus(BusTestHarness harness)
        {
            SetupActivities(harness);
        }

        class BusFactoryConfigurator :
            ActivityTestContextConfigurator
        {
            readonly IRabbitMqBusFactoryConfigurator _configurator;
            readonly IRabbitMqHost _host;

            public BusFactoryConfigurator(IRabbitMqHost host, IRabbitMqBusFactoryConfigurator configurator)
            {
                _host = host;
                _configurator = configurator;
            }

            public void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configure)
            {
                _configurator.ReceiveEndpoint(_host, queueName, x =>
                {
                    x.PrefetchCount = 1;
                    x.PurgeOnStartup = true;
                    configure(x);
                });
            }
        }


        protected void AddActivityContext<T, TArguments, TLog>(Func<T> activityFactory,
            Action<IExecuteActivityConfigurator<T, TArguments>> configureExecute = null,
            Action<ICompensateActivityConfigurator<T, TLog>> configureCompensate = null)
            where TArguments : class
            where TLog : class
            where T : class, Activity<TArguments, TLog>
        {
            var context = new ActivityTestContext<T, TArguments, TLog>(BusTestHarness, activityFactory, configureExecute, configureCompensate);

            ActivityTestContexts.Add(typeof(T), context);
        }

        protected void AddActivityContext<T, TArguments>(Func<T> activityFactory, Action<IExecuteActivityConfigurator<T, TArguments>> configure = null)
            where TArguments : class
            where T : class, ExecuteActivity<TArguments>
        {
            var context = new ActivityTestContext<T, TArguments>(BusTestHarness, activityFactory, configure);

            ActivityTestContexts.Add(typeof(T), context);
        }

        protected ActivityTestContext GetActivityContext<T>()
        {
            return ActivityTestContexts[typeof(T)];
        }

        protected virtual void SetupActivities(BusTestHarness testHarness)
        {
        }
    }
}