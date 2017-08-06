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
namespace MassTransit.HttpTransport.Contexts
{
    using System;
    using System.Threading;
    using GreenPipes;
    using Hosting;
    using Util;


    public class SharedHttpOwinHostContext :
        OwinHostContext,
        IDisposable
    {
        readonly OwinHostContext _context;
        readonly ITaskParticipant _participant;

        public SharedHttpOwinHostContext(OwinHostContext context, CancellationToken cancellationToken, ITaskSupervisor scope)
        {
            _context = context;
            CancellationToken = cancellationToken;


            _participant = scope.CreateParticipant($"{TypeMetadataCache<SharedHttpOwinHostContext>.ShortName} - {context.HostSettings.ToDebugString()}");
            _participant.SetReady();
        }

        public void Dispose()
        {
            _participant.SetComplete();
        }

        public CancellationToken CancellationToken { get; }

        bool PipeContext.HasPayloadType(Type contextType)
        {
            return _context.HasPayloadType(contextType);
        }

        bool PipeContext.TryGetPayload<TPayload>(out TPayload payload)
        {
            return _context.TryGetPayload(out payload);
        }

        TPayload PipeContext.GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
        {
            return _context.GetOrAddPayload(payloadFactory);
        }

        public HttpHostSettings HostSettings => _context.HostSettings;

        void OwinHostContext.RegisterEndpointHandler(string pathMatch, HttpConsumerAction handler)
        {
            _context.RegisterEndpointHandler(pathMatch, handler);
        }

        void OwinHostContext.StopHttpListener()
        {
            _context.StopHttpListener();
        }

        void OwinHostContext.StartHost()
        {
            _context.StartHost();
        }
    }
}