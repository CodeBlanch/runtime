// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.Serialization;

namespace Microsoft.Extensions.Logging.Payloads;

[Serializable]
public sealed class LoggingPayloadSerializationException : Exception
{
    private bool _ExceptionMessageUpdated;
    private string? _Message;

    public override string Message => _Message ?? base.Message;

    public LoggingPayloadSerializationException()
        : base()
    {
    }

    public LoggingPayloadSerializationException(string message)
        : base(message)
    {
        _Message = message;
    }

    public LoggingPayloadSerializationException(string? message, Exception? innerException)
        : base(message, innerException)
    {
        _Message = message;
    }

    private LoggingPayloadSerializationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    internal void TryUpdateExceptionMessage(ref LoggingPayloadWriter writer)
    {
        if (_ExceptionMessageUpdated)
            return;

        _ExceptionMessageUpdated = true;

        if (string.IsNullOrEmpty(_Message))
            _Message = "Unhandled Exception thrown serializing logging payload.";

        string path = writer.State.ToPath();
        if (!string.IsNullOrEmpty(path))
            _Message = $"{_Message} Path: {path}.";
    }
}
