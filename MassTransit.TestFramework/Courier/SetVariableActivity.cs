﻿// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MassTransit.Courier;
    using MassTransit.Courier.Exceptions;


    public class SetVariableActivity :
        ExecuteActivity<SetVariableArguments>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<SetVariableArguments> context)
        {
            if (context.Arguments == null)
                throw new RoutingSlipException("The arguments for execution were null");

            return context.CompletedWithVariables(new Dictionary<string, object> {{context.Arguments.Key, context.Arguments.Value}});
        }
    }
}