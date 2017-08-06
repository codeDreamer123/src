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
namespace MassTransit.Pipeline.Pipes
{
    using System;
    using Context;
    using GreenPipes;
    using GreenPipes.Pipes;


    public class ManagementPipe :
        DynamicRouter<ConsumeContext, Guid>,
        IManagementPipe
    {
        public ManagementPipe()
            : base(new ConsumeContextConverterFactory(), GetRequestId)
        {
        }

        ConnectHandle IConsumePipeConnector.ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            return ConnectPipe(pipe);
        }

        ConnectHandle IRequestPipeConnector.ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
        {
            return ConnectPipe(requestId, pipe);
        }

        static Guid GetRequestId(ConsumeContext context)
        {
            return context.RequestId ?? Guid.Empty;
        }
    }
}