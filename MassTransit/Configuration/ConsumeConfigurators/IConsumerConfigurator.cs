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
namespace MassTransit.ConsumeConfigurators
{
    using System;
    using GreenPipes;


    public interface IConsumerConfigurator :
        IConsumeConfigurator,
        IConsumerConfigurationObserverConnector
    {
    }


    public interface IConsumerConfigurator<TConsumer> :
        IPipeConfigurator<ConsumerConsumeContext<TConsumer>>,
        IConsumeConfigurator
        where TConsumer : class
    {
        /// <summary>
        /// Configure a message type for the consumer, such as adding middleware to the pipeline for
        /// the message type.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configure">The callback to configure the message pipeline</param>
        [Obsolete("This is just too long/noisy, so use Message<T> instead")]
        void ConfigureMessage<T>(Action<IConsumerMessageConfigurator<T>> configure)
            where T : class;

        /// <summary>
        /// Configure a message type for the consumer, such as adding middleware to the pipeline for
        /// the message type.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configure">The callback to configure the message pipeline</param>
        void Message<T>(Action<IConsumerMessageConfigurator<T>> configure)
            where T : class;

        /// <summary>
        /// Configure a message type for the consumer, such as adding middleware to the pipeline for
        /// the message type.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configure">The callback to configure the message pipeline</param>
        void ConsumerMessage<T>(Action<IConsumerMessageConfigurator<TConsumer, T>> configure)
            where T : class;
    }
}