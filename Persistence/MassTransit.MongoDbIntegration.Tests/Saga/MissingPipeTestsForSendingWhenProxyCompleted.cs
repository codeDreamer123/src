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
namespace MassTransit.MongoDbIntegration.Tests.Saga
{
    using System.Threading;
    using GreenPipes;
    using MongoDbIntegration.Saga.Context;
    using MongoDbIntegration.Saga.Pipeline;
    using MongoDB.Driver;
    using Moq;
    using NUnit.Framework;
    using Pipeline;
    using Util;


    [TestFixture]
    public class MissingPipeTestsForSendingWhenProxyCompleted
    {
        [Test]
        public void ThenNextPipeCalled()
        {
            _nextPipe.Verify(m => m.Send(It.IsAny<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>>()), Times.Once);
        }

        [Test]
        public void ThenSagaNotInsertedIntoCollection()
        {
            _collection.Verify(m => m.InsertOneAsync(It.IsAny<SimpleSaga>(), null, It.IsAny<CancellationToken>()), Times.Never);
        }

        Mock<IPipe<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>>> _nextPipe;
        MissingPipe<SimpleSaga, InitiateSimpleSaga> _pipe;
        Mock<IMongoCollection<SimpleSaga>> _collection;
        Mock<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>> _context;
        Mock<IMongoDbSagaConsumeContextFactory> _consumeContextFactory;
        Mock<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>> _proxy;

        [OneTimeSetUp]
        public void GivenAMissingPipe_WhenSendingAndProxyCompleted()
        {
            _collection = new Mock<IMongoCollection<SimpleSaga>>();
            _nextPipe = new Mock<IPipe<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>>>();
            _proxy = new Mock<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>>();
            _proxy.SetupGet(m => m.IsCompleted).Returns(true);
            _consumeContextFactory = new Mock<IMongoDbSagaConsumeContextFactory>();
            _context = new Mock<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>>();
            _consumeContextFactory.Setup(m => m.Create(_collection.Object, _context.Object, _context.Object.Saga, false)).Returns(_proxy.Object);

            _pipe = new MissingPipe<SimpleSaga, InitiateSimpleSaga>(_collection.Object, _nextPipe.Object, _consumeContextFactory.Object);

            TaskUtil.Await(() => _pipe.Send(_context.Object));
        }
    }
}