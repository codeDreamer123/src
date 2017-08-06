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
namespace MassTransit.Turnout.Events
{
    using System;
    using System.Collections.Generic;
    using Contracts;


    public class JobStartedEvent :
        JobStarted
    {
        public JobStartedEvent(Guid jobId, int retryCount, Uri managementAddress, IDictionary<string, object> arguments)
        {
            JobId = jobId;
            Timestamp = DateTime.UtcNow;
            RetryCount = retryCount;
            Arguments = arguments;
            ManagementAddress = managementAddress;
        }

        public Guid JobId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public int RetryCount { get; private set; }
        public Uri ManagementAddress { get; private set; }
        public IDictionary<string, object> Arguments { get; private set; }
    }
}