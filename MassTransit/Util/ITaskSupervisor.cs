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
namespace MassTransit.Util
{
    using System.Threading.Tasks;


    public interface ITaskSupervisor
    {
        /// <summary>
        /// Completed when the supervisor and subordinates are ready
        /// </summary>
        Task Ready { get; }

        /// <summary>
        /// Completed when the signal is complete and ready to exit
        /// </summary>
        Task Completed { get; }

        /// <summary>
        /// Connects a participant to the signal for observation by the signaler
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        ITaskParticipant CreateParticipant(string tag);

        /// <summary>
        /// Creates a scope that has it's own participants that can be coordinated
        /// </summary>
        /// <returns></returns>
        ITaskScope CreateScope(string tag);
    }
}