// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

using Microsoft.Extensions.Logging.Payloads;

namespace Microsoft.Extensions.Logging;

public interface IPayloadLogger : ILogger
{
    void Log<TPayload>(
        LogLevel logLevel,
        EventId eventId,
        in TPayload? payload,
        Exception? exception,
        string message,
        LoggingPayloadConverter<TPayload>? converter = null,
        LoggingPayloadSerializerOptions? options = null);
}
