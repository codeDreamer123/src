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
namespace MassTransit.Turnout.Contracts
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// Published when a job completes
    /// </summary>
    public interface JobCompleted
    {
        /// <summary>
        /// The job identifier
        /// </summary>
        Guid JobId { get; }

        /// <summary>
        /// The time the job completed
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The arguments used to start the job
        /// </summary>
        IDictionary<string, object> Arguments { get; }

        /// <summary>
        /// The results of the job, serialized
        /// </summary>
        IDictionary<string, object> Results { get; }
    }


    /// <summary>
    /// Published when a job completes with a result
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface JobCompleted<out TResult> :
        JobCompleted
        where TResult : class
    {
        /// <summary>
        /// The result of the job
        /// </summary>
        TResult Result { get; }
    }
}