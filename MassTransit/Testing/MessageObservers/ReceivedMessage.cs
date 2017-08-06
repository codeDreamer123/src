﻿// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Testing.MessageObservers
{
    using System;


    public class ReceivedMessage<T> :
        IReceivedMessage<T>
        where T : class
    {
        readonly ConsumeContext<T> _context;

        public ReceivedMessage(ConsumeContext<T> context, Exception exception = null)
        {
            _context = context;
            Exception = exception;
        }

        ConsumeContext IReceivedMessage.Context => _context;

        public Exception Exception { get; }

        public Type MessageType => typeof(T);

        public ConsumeContext<T> Context => _context;
    }
}