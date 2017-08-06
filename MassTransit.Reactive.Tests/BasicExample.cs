// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Reactive.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class BasicExample :
        InMemoryTestFixture
    {
        [Test]
        public async Task The_message_should_be_observed()
        {
            await _thatJustHappened.Task;
        }

        [OneTimeSetUp]
        public void A_reactive_query_is_observing_a_bus_message()
        {
            _observable = Bus.AsObservable<PingMessage>();

            _thatJustHappened = GetTask<PingMessage>();
            _subscription = _observable.Subscribe(m => _thatJustHappened.TrySetResult(m));

            BusSendEndpoint.Send(new PingMessage());
        }

        IObservable<PingMessage> _observable;
        TaskCompletionSource<PingMessage> _thatJustHappened;
        IDisposable _subscription;

        [OneTimeTearDown]
        public void Finally()
        {
            _subscription.Dispose();
        }
    }
}