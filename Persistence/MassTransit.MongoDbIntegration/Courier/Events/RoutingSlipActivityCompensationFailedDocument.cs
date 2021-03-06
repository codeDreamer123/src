﻿// Copyright 2007-2013 Chris Patterson
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
namespace MassTransit.MongoDbIntegration.Courier.Events
{
    using System;
    using Documents;
    using MassTransit.Courier.Contracts;


    public class RoutingSlipActivityCompensationFailedDocument :
        RoutingSlipEventDocument
    {
        public RoutingSlipActivityCompensationFailedDocument(RoutingSlipActivityCompensationFailed message)
            : base(message.Timestamp, message.Duration, message.Host)
        {
            ActivityName = message.ActivityName;
            ExecutionId = message.ExecutionId;

            if (message.ExceptionInfo != null)
                ExceptionInfo = new ExceptionInfoDocument(message.ExceptionInfo);
        }

        public string ActivityName { get; private set; }
        public Guid ExecutionId { get; private set; }
        public ExceptionInfoDocument ExceptionInfo { get; private set; }
    }
}