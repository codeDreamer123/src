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
namespace Automatonymous
{
    using System;


    /// <summary>
    /// Provides an address for the request service
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate Uri ServiceAddressProvider<in TInstance>(ConsumeEventContext<TInstance> context);

    /// <summary>
    /// Provides an address for the request service
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TData"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate Uri ServiceAddressProvider<in TInstance, in TData>(ConsumeEventContext<TInstance, TData> context)
        where TData : class;


    /// <summary>
    /// Provides an address for the request service
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TException"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate Uri ServiceAddressProvider<in TInstance, in TData, in TException>(ConsumeExceptionEventContext<TInstance, TData, TException> context)
        where TData : class
        where TException : Exception;
}