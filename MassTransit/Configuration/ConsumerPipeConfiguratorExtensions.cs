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
namespace GreenPipes
{
    using System;
    using MassTransit;
    using MassTransit.PipeConfigurators;


    public static class ConsumerPipeConfiguratorExtensions
    {
        /// <summary>
        /// Adds a filter to the pipe
        /// </summary>
        /// <typeparam name="T">The context type</typeparam>
        /// <typeparam name="TConsumer"></typeparam>
        /// <param name="configurator">The pipe configurator</param>
        /// <param name="filter">The already built pipe</param>
        public static void UseFilter<TConsumer, T>(this IPipeConfigurator<ConsumerConsumeContext<TConsumer, T>> configurator,
            IFilter<ConsumerConsumeContext<TConsumer>> filter)
            where T : class
            where TConsumer : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new ConsumerPipeSpecification<TConsumer, T>(filter);

            configurator.AddPipeSpecification(specification);
        }
    }
}