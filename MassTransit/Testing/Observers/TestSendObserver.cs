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
namespace MassTransit.Testing.Observers
{
    using System;
    using System.Threading.Tasks;
    using MessageObservers;


    /// <summary>
    /// Observes sent messages for test fixtures
    /// </summary>
    public class TestSendObserver :
        ISendObserver
    {
        readonly SentMessageList _messages;

        public TestSendObserver(TimeSpan timeout)
        {
            _messages = new SentMessageList(timeout);
        }

        public ISentMessageList Messages => _messages;

        public async Task PreSend<T>(SendContext<T> context)
            where T : class
        {
        }

        public async Task PostSend<T>(SendContext<T> context)
            where T : class
        {
            _messages.Add(context);
        }

        public async Task SendFault<T>(SendContext<T> context, Exception exception)
            where T : class
        {
            _messages.Add(context, exception);
        }
    }
}