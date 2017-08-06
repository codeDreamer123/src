﻿namespace MassTransit.MartenIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Marten;
    using NUnit.Framework;
    using Saga;
    using Shouldly;
    using TestFramework;


    [TestFixture, Category("Integration")]
    public class LocatingAnExistingSaga : InMemoryTestFixture
    {
        [Test]
        public async Task A_correlated_message_should_find_the_correct_saga()
        {
            Guid sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            var found = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            found.ShouldBe(sagaId);

            var nextMessage = new CompleteSimpleSaga {CorrelationId = sagaId};

            await InputQueueSendEndpoint.Send(nextMessage);

            found = await _sagaRepository.Value.ShouldContainSaga(x => x.CorrelationId == sagaId && x.Completed, TestTimeout);
            found.ShouldBe(sagaId);
        }

        [Test]
        public async Task An_initiating_message_should_start_the_saga()
        {
            Guid sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            var found = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            found.ShouldBe(sagaId);
        }

        readonly Lazy<ISagaRepository<SimpleSaga>> _sagaRepository;

        public LocatingAnExistingSaga()
        {
            var connectionString =
                "server=localhost;port=5432;database=MartenTest;user id=postgres;password=Password12!;";
            var store = DocumentStore.For(connectionString);
            _sagaRepository = new Lazy<ISagaRepository<SimpleSaga>>(() => new MartenSagaRepository<SimpleSaga>(store));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(_sagaRepository.Value);
        }
    }
}