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
namespace RapidTransit.Sample
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using MassTransit;


    public class FriesConsumer :
        IConsumer<MakeFries>
    {
        public async Task Consume(ConsumeContext<MakeFries> context)
        {
            await Console.Out.WriteAsync($"Making fries for {context.Message.OrderId}");

            await Task.Delay(TimeSpan.FromSeconds(3));

            await Console.Out.WriteAsync($"Fries are fried for {context.Message.OrderId}");
        }
    }
}