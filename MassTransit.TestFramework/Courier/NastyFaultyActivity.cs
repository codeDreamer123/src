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
namespace MassTransit.TestFramework.Courier
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Courier;


    public class NastyFaultyActivity :
        Activity<FaultyArguments, FaultyLog>
    {
        public Task<ExecutionResult> Execute(ExecuteContext<FaultyArguments> context)
        {
            Console.WriteLine("NastyFaultyActivity: Execute");
            Console.WriteLine("NastyFaultyActivity: About to blow this up!");

            throw new InvalidOperationException("Things that make you go boom!");
        }

        public async Task<CompensationResult> Compensate(CompensateContext<FaultyLog> context)
        {
            return context.Compensated();
        }
    }
}