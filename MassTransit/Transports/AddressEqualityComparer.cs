// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports
{
    using System;
    using System.Collections.Generic;


    public class AddressEqualityComparer :
        IEqualityComparer<Uri>
    {
        public static readonly IEqualityComparer<Uri> Comparer = new AddressEqualityComparer();

        public bool Equals(Uri x, Uri y)
        {
            return ReferenceEquals(x, y)
                || (x != null
                    && y != null
                    && x.Scheme.Equals(y.Scheme, StringComparison.OrdinalIgnoreCase)
                    && x.Host.Equals(y.Host, StringComparison.OrdinalIgnoreCase)
                    && x.Port.Equals(y.Port)
                    && x.AbsolutePath.Equals(y.AbsolutePath, StringComparison.OrdinalIgnoreCase));
        }

        public int GetHashCode(Uri obj)
        {
            return obj.AbsolutePath.GetHashCode();
        }
    }
}