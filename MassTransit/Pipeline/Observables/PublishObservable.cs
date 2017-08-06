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
namespace MassTransit.Pipeline.Observables
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes.Util;


    public class PublishObservable :
        Connectable<IPublishObserver>,
        IPublishObserver
    {
        public Task PrePublish<T>(PublishContext<T> context) where T : class
        {
            return ForEachAsync(x => x.PrePublish(context));
        }

        public Task PostPublish<T>(PublishContext<T> context) where T : class
        {
            return ForEachAsync(x => x.PostPublish(context));
        }

        public Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class
        {
            return ForEachAsync(x => x.PublishFault(context, exception));
        }
    }
}