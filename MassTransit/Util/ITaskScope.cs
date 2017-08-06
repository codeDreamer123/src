// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Util
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// A task participant that hosts additional participants
    /// </summary>
    public interface ITaskScope :
        ITaskParticipant,
        ITaskSupervisor,
        IDisposable
    {
        Task Stop(string reason, Func<Task> afterStopped, CancellationToken cancellationToken = default(CancellationToken));
    }
}