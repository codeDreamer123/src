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
namespace MassTransit.AzureServiceBusTransport.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using Events;
    using GreenPipes;
    using Logging;
    using MassTransit.Pipeline.Observables;
    using Policies;
    using Transports;
    using Util;


    public class ReceiveTransport :
        IReceiveTransport
    {
        static readonly ILog _log = Logger.Get<ReceiveTransport>();
        readonly ReceiveTransportObservable _endpointObservers;
        readonly IServiceBusHost _host;
        readonly IPublishEndpointProvider _publishEndpointProvider;
        readonly ReceiveObservable _receiveObservers;
        readonly ClientSettings _settings;
        readonly IPipeSpecification<NamespaceContext>[] _specifications;

        public ReceiveTransport(IServiceBusHost host, ClientSettings settings, IPublishEndpointProvider publishEndpointProvider,
            IPipeSpecification<NamespaceContext>[] specifications)
        {
            _host = host;
            _settings = settings;
            _publishEndpointProvider = publishEndpointProvider;
            _specifications = specifications;
            _receiveObservers = new ReceiveObservable();
            _endpointObservers = new ReceiveTransportObservable();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateScope("transport");
            scope.Set(new
            {
                Type = "Azure Service Bus",
                _settings.Path,
                _settings.PrefetchCount,
                _settings.MaxConcurrentCalls
            });
        }

        public ReceiveTransportHandle Start(IPipe<ReceiveContext> receivePipe)
        {
            Uri inputAddress = _settings.GetInputAddress(_host.Settings.ServiceUri);

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Starting receive transport: {0}", inputAddress);

            var supervisor = new TaskSupervisor($"{TypeMetadataCache<ReceiveTransport>.ShortName} - {inputAddress}");

            IPipe<NamespaceContext> pipe = Pipe.New<NamespaceContext>(x =>
            {
                for (var i = 0; i < _specifications.Length; i++)
                {
                    x.AddPipeSpecification(_specifications[i]);
                }
            });

            Task.Factory.StartNew(() => Receiver(pipe, supervisor), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);

            return new Handle(supervisor);
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveObservers.Connect(observer);
        }

        public ConnectHandle ConnectReceiveTransportObserver(IReceiveTransportObserver observer)
        {
            return _endpointObservers.Connect(observer);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishEndpointProvider.ConnectPublishObserver(observer);
        }

        async Task Receiver(IPipe<NamespaceContext> pipe, TaskSupervisor supervisor)
        {
            Uri inputAddress = _settings.GetInputAddress(_host.Settings.ServiceUri);

            try
            {
                await _host.RetryPolicy.RetryUntilCancelled(async () =>
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Connecting receive transport: {0}", inputAddress);

                    var context = new ServiceBusNamespaceContext(_host, _receiveObservers, _endpointObservers, supervisor);

                    try
                    {
                        await pipe.Send(context).ConfigureAwait(false);
                    }
                    catch (TaskCanceledException)
                    {
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception ex)
                    {
                        if (_log.IsErrorEnabled)
                            _log.Error($"Azure Service Bus receiver faulted: {inputAddress}", ex);

                        await _endpointObservers.Faulted(new ReceiveTransportFaultedEvent(inputAddress, ex)).ConfigureAwait(false);

                        throw;
                    }
                }, supervisor.StoppingToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _log.Error($"Unhandled exception on Receiver: {inputAddress}", ex);
            }
        }


        class Handle :
            ReceiveTransportHandle
        {
            readonly TaskSupervisor _supervisor;

            public Handle(TaskSupervisor supervisor)
            {
                _supervisor = supervisor;
            }

            Task ReceiveTransportHandle.Stop(CancellationToken cancellationToken)
            {
                return _supervisor.Stop("Stop Receive Transport", cancellationToken);
            }
        }
    }
}