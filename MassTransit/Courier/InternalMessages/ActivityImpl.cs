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
namespace MassTransit.Courier.InternalMessages
{
    using System;
    using System.Collections.Generic;
    using Contracts;


    class ActivityImpl :
        Activity
    {
        public ActivityImpl(string name, Uri address, IDictionary<string, object> arguments)
        {
            Name = name;
            Address = address;
            Arguments = arguments;
        }

        public string Name { get; private set; }
        public Uri Address { get; private set; }
        public IDictionary<string, object> Arguments { get; private set; }
    }
}