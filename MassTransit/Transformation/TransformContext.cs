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
    using System;
    using System.Threading;
    using GreenPipes;


    /// <summary>
    /// Context used by a message transform
    /// </summary>
    public interface TransformContext
    {
        /// <summary>
        /// Used to cancel the execution of the context
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// The messageId assigned to the message when it was initially Sent. This is different
        /// than the transport MessageId, which is only for the Transport.
        /// </summary>
        Guid? MessageId { get; }

        /// <summary>
        /// If the message is a request, or related to a request, such as a response or a fault,
        /// this contains the requestId.
        /// </summary>
        Guid? RequestId { get; }

        /// <summary>
        /// If the message implements the CorrelatedBy(Guid) interface, this field should be 
        /// populated by default to match that value. It can, of course, be overwritten with
        /// something else.
        /// </summary>
        Guid? CorrelationId { get; }

        /// <summary>
        /// The conversationId of the message, which is copied and carried throughout the message
        /// flow by the infrastructure.
        /// </summary>
        Guid? ConversationId { get; }

        /// <summary>
        /// If this message was produced within the context of a previous message, the CorrelationId
        /// of the message is contained in this property. If the message was produced from a saga
        /// instance, the CorrelationId of the saga is used.
        /// </summary>
        Guid? InitiatorId { get; }

        /// <summary>
        /// The address of the message producer that sent the message
        /// </summary>
        Uri SourceAddress { get; }

        /// <summary>
        /// The destination address of the message
        /// </summary>
        Uri DestinationAddress { get; }

        /// <summary>
        /// Additional application-specific headers that are added to the message by the application
        /// or by features within MassTransit, such as when a message is moved to an error queue.
        /// </summary>
        Headers Headers { get; }

        /// <summary>
        /// The host information of the message producer. This may not be present if the message was sent
        /// from an earlier version of MassTransit.
        /// </summary>
        HostInfo Host { get; }

        /// <summary>
        /// Checks if a payload is present in the context
        /// </summary>
        /// <param name="contextType"></param>
        /// <returns></returns>
        bool HasPayloadType(Type contextType);

        /// <summary>
        /// Retrieves a payload from the pipe context
        /// </summary>
        /// <typeparam name="TPayload">The payload type</typeparam>
        /// <param name="payload">The payload</param>
        /// <returns></returns>
        bool TryGetPayload<TPayload>(out TPayload payload)
            where TPayload : class;

        /// <summary>
        /// Returns an existing payload or creates the payload using the factory method provided
        /// </summary>
        /// <typeparam name="TPayload">The payload type</typeparam>
        /// <param name="payloadFactory">The payload factory is the payload is not present</param>
        /// <returns>The payload</returns>
        TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
            where TPayload : class;
    }


    /// <summary>
    /// Context used by a message transform
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public interface TransformContext<out TInput> :
        TransformContext
    {
        /// <summary>
        /// The input message to be transformed
        /// </summary>
        TInput Input { get; }

        /// <summary>
        /// True if the input is present, otherwise false
        /// </summary>
        bool HasInput { get; }
    }
}