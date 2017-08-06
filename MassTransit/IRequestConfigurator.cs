// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading.Tasks;
    using ConsumeConfigurators;


    /// <summary>
    /// Sending of a request, allowing specification of response handlers, etc.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    public interface IRequestConfigurator<TRequest> :
        SendContext<TRequest>
        where TRequest : class
    {
        /// <summary>
        /// The timeout before the pending tasks are cancelled
        /// </summary>
        TimeSpan Timeout { set; }

        /// <summary>
        /// The request Task
        /// </summary>
        Task<TRequest> Task { get; }

        /// <summary>
        /// Specify that the current synchronization context should be used for the request
        /// </summary>
        void UseCurrentSynchronizationContext();

        /// <summary>
        /// Set the synchronization context used for the request and related handlers
        /// </summary>
        /// <param name="taskScheduler"></param>
        void SetTaskScheduler(TaskScheduler taskScheduler);

        /// <summary>
        /// Configures a watcher to be called when a specified type is received. Messages
        /// received do not complete the request, but are merely observed while the request
        /// is pending.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler">The message handler invoked</param>
        /// <param name="configure"></param>
        void Watch<T>(MessageHandler<T> handler, Action<IHandlerConfigurator<T>> configure = null)
            where T : class;

        /// <summary>
        /// Configure a handler called when the specified response type is received. Receiving
        /// a response completes the request and either completes or fails the awaiting task
        /// </summary>
        /// <typeparam name="T">The response type</typeparam>
        /// <param name="handler">The reponse handler</param>
        /// <param name="configure"></param>
        /// <returns>The response task</returns>
        Task<T> Handle<T>(MessageHandler<T> handler, Action<IHandlerConfigurator<T>> configure = null)
            where T : class;

        /// <summary>
        /// Configure a handler called when the specified response type is received. Receiving
        /// a response completes the request and either completes or fails the awaiting task
        /// </summary>
        /// <typeparam name="T">The response type</typeparam>
        /// <param name="configure"></param>
        /// <returns>The response task</returns>
        Task<T> Handle<T>(Action<IHandlerConfigurator<T>> configure = null)
            where T : class;
    }
}