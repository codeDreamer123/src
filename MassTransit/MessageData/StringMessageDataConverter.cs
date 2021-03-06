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
namespace MassTransit.MessageData
{
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;


    public class StringMessageDataConverter :
        IMessageDataConverter<string>
    {
        public async Task<string> Convert(Stream stream, CancellationToken cancellationToken)
        {
            using (var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms, 4096, cancellationToken).ConfigureAwait(false);

                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}