﻿namespace MassTransit.MartenIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;
    using Logging;
    using Marten;
    using Saga;
    using Util;


    public class MartenSagaRepository<TSaga> : ISagaRepository<TSaga>,
        IQuerySagaRepository<TSaga>,
        IRetrieveSagaFromRepository<TSaga>
        where TSaga : class, ISaga
    {
        static readonly ILog _log = Logger.Get<MartenSagaRepository<TSaga>>();
        readonly IDocumentStore _store;

        public MartenSagaRepository(IDocumentStore store)
        {
            _store = store;
        }

        public async Task<IEnumerable<Guid>> Find(ISagaQuery<TSaga> query)
        {
            using (var session = _store.QuerySession())
            {
                return await session.Query<TSaga>()
                    .Where(query.FilterExpression)
                    .Select(x => x.CorrelationId)
                    .ToListAsync();
            }
        }

        public TSaga GetSaga(Guid correlationId)
        {
            using (var session = _store.QuerySession())
                return session.Load<TSaga>(correlationId);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("sagaRepository");
            scope.Set(new
            {
                Persistence = "marten"
            });
        }

        async Task ISagaRepository<TSaga>.Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            if (!context.CorrelationId.HasValue)
                throw new SagaException("The CorrelationId was not specified", typeof(TSaga), typeof(T));

            var sagaId = context.CorrelationId.Value;

            using (var session = _store.DirtyTrackedSession())
            {
                TSaga instance;
                if (policy.PreInsertInstance(context, out instance))
                    PreInsertSagaInstance<T>(session, instance);

                if (instance == null)
                    instance = session.Load<TSaga>(sagaId);

                if (instance == null)
                {
                    var missingSagaPipe = new MissingPipe<T>(session, next);
                    await policy.Missing(context, missingSagaPipe).ConfigureAwait(false);
                }
                else
                {
                    await SendToInstance(context, policy, instance, next, session).ConfigureAwait(false);
                }
            }
        }

        public async Task SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next) where T : class
        {
            using (var session = _store.LightweightSession())
            {
                try
                {
                    IList<TSaga> instances = await session.Query<TSaga>()
                        .Where(context.Query.FilterExpression)
                        .ToListAsync().ConfigureAwait(false);

                    if (instances.Count == 0)
                    {
                        var missingSagaPipe = new MissingPipe<T>(session, next);
                        await policy.Missing(context, missingSagaPipe).ConfigureAwait(false);
                    }
                    else
                    {
                        foreach (var instance in instances)
                            await SendToInstance(context, policy, instance, next, session).ConfigureAwait(false);
                    }
                }
                catch (SagaException sex)
                {
                    if (_log.IsErrorEnabled)
                        _log.Error($"SAGA:{TypeMetadataCache<TSaga>.ShortName} Exception {TypeMetadataCache<T>.ShortName}", sex);

                    throw;
                }
                catch (Exception ex)
                {
                    if (_log.IsErrorEnabled)
                        _log.Error($"SAGA:{TypeMetadataCache<TSaga>.ShortName} Exception {TypeMetadataCache<T>.ShortName}", ex);

                    throw new SagaException(ex.Message, typeof(TSaga), typeof(T), Guid.Empty, ex);
                }
            }
        }

        static bool PreInsertSagaInstance<T>(IDocumentSession session, TSaga instance)
        {
            var inserted = false;
            try
            {
                session.Store(instance);
                session.SaveChanges();
                inserted = true;

                _log.DebugFormat("SAGA:{0}:{1} Insert {2}", TypeMetadataCache<TSaga>.ShortName, instance.CorrelationId,
                    TypeMetadataCache<T>.ShortName);
            }
            catch (Exception ex)
            {
                if (_log.IsDebugEnabled)
                {
                    _log.DebugFormat("SAGA:{0}:{1} Dupe {2} - {3}", TypeMetadataCache<TSaga>.ShortName,
                        instance.CorrelationId,
                        TypeMetadataCache<T>.ShortName, ex.Message);
                }
            }
            return inserted;
        }

        static async Task SendToInstance<T>(ConsumeContext<T> context,
            ISagaPolicy<TSaga, T> policy, TSaga instance,
            IPipe<SagaConsumeContext<TSaga, T>> next, IDocumentSession session)
            where T : class
        {
            try
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("SAGA:{0}:{1} Used {2}", TypeMetadataCache<TSaga>.ShortName, instance.CorrelationId,
                        TypeMetadataCache<T>.ShortName);

                var sagaConsumeContext = new MartenSagaConsumeContext<TSaga, T>(session, context, instance);

                await policy.Existing(sagaConsumeContext, next).ConfigureAwait(false);

                if (!sagaConsumeContext.IsCompleted)
                {
                    session.Store(instance);
                    session.SaveChanges();
                }
            }
            catch (SagaException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SagaException(ex.Message, typeof(TSaga), typeof(T), instance.CorrelationId, ex);
            }
        }


        /// <summary>
        ///     Once the message pipe has processed the saga instance, add it to the saga repository
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        class MissingPipe<TMessage> :
            IPipe<SagaConsumeContext<TSaga, TMessage>>
            where TMessage : class
        {
            readonly IPipe<SagaConsumeContext<TSaga, TMessage>> _next;
            readonly IDocumentSession _session;

            public MissingPipe(IDocumentSession session, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
            {
                _session = session;
                _next = next;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
                _next.Probe(context);
            }

            public async Task Send(SagaConsumeContext<TSaga, TMessage> context)
            {
                if (_log.IsDebugEnabled)
                {
                    _log.DebugFormat("SAGA:{0}:{1} Added {2}", TypeMetadataCache<TSaga>.ShortName,
                        context.Saga.CorrelationId,
                        TypeMetadataCache<TMessage>.ShortName);
                }

                SagaConsumeContext<TSaga, TMessage> proxy = new MartenSagaConsumeContext<TSaga, TMessage>(_session,
                    context, context.Saga);

                await _next.Send(proxy).ConfigureAwait(false);

                if (!proxy.IsCompleted)
                {
                    _session.Store(context.Saga);
                    _session.SaveChanges();
                }
            }
        }
    }
}