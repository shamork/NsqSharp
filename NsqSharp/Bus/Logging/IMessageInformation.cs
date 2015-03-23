﻿using System;
using System.Reflection;

namespace NsqSharp.Bus.Logging
{
    internal class MessageInformation : IMessageInformation
    {
        public Guid UniqueIdentifier { get; set; }
        public string Topic { get; set; }
        public string Channel { get; set; }
        public Type HandlerType { get; set; }
        public Type MessageType { get; set; }
        public Message Message { get; set; }
        public object DeserializedMessageBody { get; set; }
        public DateTime Started { get; set; }
        public DateTime? Finished { get; set; }
        public bool BackoffTriggered { get; set; }
        public DateTime? RequeuedUntil { get; set; }
    }

    internal class FailedMessageInformation : IFailedMessageInformation
    {
        private readonly IMessageInformation _info;

        public FailedMessageInformation(
            IMessageInformation info,
            FailedMessageQueueAction action,
            FailedMessageReason reason,
            Exception exception
        )
        {
            if (info == null)
                throw new ArgumentNullException("info");

            if (exception != null)
            {
                while (exception is TargetInvocationException && exception.InnerException != null)
                    exception = exception.InnerException;
            }

            _info = info;
            FailedAction = action;
            FailedReason = reason;
            Exception = exception;
        }

        public FailedMessageQueueAction FailedAction { get; private set; }
        public FailedMessageReason FailedReason { get; private set; }
        public Exception Exception { get; private set; }

        public Guid UniqueIdentifier { get { return _info.UniqueIdentifier; } }
        public string Topic { get { return _info.Topic; } }
        public string Channel { get { return _info.Channel; } }
        public Type HandlerType { get { return _info.HandlerType; } }
        public Type MessageType { get { return _info.MessageType; } }
        public Message Message { get { return _info.Message; } }
        public object DeserializedMessageBody { get { return _info.DeserializedMessageBody; } }
        public DateTime Started { get { return _info.Started; } }
        public DateTime? Finished { get { return _info.Finished; } }
    }

    /// <summary>
    /// Message information including topic, channel, handler type, message type, raw NSQ message, and deserialized message.
    /// <seealso cref="IMessageAuditor"/>
    /// </summary>
    public interface IMessageInformation
    {
        /// <summary>A unique identifier for this instance of handling this message.</summary>
        Guid UniqueIdentifier { get; }
        /// <summary>The topic the message was delivered on.</summary>
        string Topic { get; }
        /// <summary>The channel the message was delivered on.</summary>
        string Channel { get; }
        /// <summary>The handler .NET type.</summary>
        Type HandlerType { get; }
        /// <summary>The message .NET type.</summary>
        Type MessageType { get; }
        /// <summary>The message.</summary>
        Message Message { get; }
        /// <summary>The deserialized message body (can be <c>null</c>).</summary>
        object DeserializedMessageBody { get; }
        /// <summary>The <see cref="DateTime"/> the handler started processing this message.</summary>
        DateTime Started { get; }
        /// <summary>The <see cref="DateTime"/> the handler finished processing this message (can be <c>null</c>).</summary>
        DateTime? Finished { get; }
    }

    /// <summary>
    /// Failed message information. In addition to properties provided by <see cref="IMessageInformation"/> the following are
    /// added: <see cref="FailedAction"/>, <see cref="FailedReason"/>, <see cref="Exception"/>.
    /// <seealso cref="IMessageAuditor"/>
    /// <seealso cref="IMessageInformation"/>
    /// </summary>
    public interface IFailedMessageInformation : IMessageInformation
    {
        /// <summary>The queue action taken for the failed message.</summary>
        FailedMessageQueueAction FailedAction { get; }
        /// <summary>The category of mesage failure.</summary>
        FailedMessageReason FailedReason { get; }
        /// <summary>The exception (can be <c>null</c>).</summary>
        Exception Exception { get; }
    }
}