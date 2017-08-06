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
namespace MassTransit.Transformation
{
    /// <summary>
    /// The result of a message transformation
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface TransformResult<out TResult>
    {
        /// <summary>
        /// The transformed message
        /// </summary>
        TResult Value { get; }

        /// <summary>
        /// True if the transform has returned a new value, otherwise false. Some transforms
        /// actually apply to the original message, versus creating a new message leaving the
        /// original message unmodified.
        /// </summary>
        bool IsNewValue { get; }
    }
}