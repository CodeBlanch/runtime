// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;

using Microsoft.Extensions.Logging.Payloads;

namespace Microsoft.Extensions.Logging;

public static partial class LoggerExtensions
{
    public static void Log<TPayload>(
        this ILogger logger,
        LogLevel logLevel,
        EventId eventId,
        in TPayload? payload,
        Exception? exception,
        string message,
        LoggingPayloadConverter<TPayload>? converter = null,
        LoggingPayloadSerializerOptions? options = null)
    {
        Debug.Assert(logger != null);

        if (logger is IPayloadLogger payloadLogger)
        {
            payloadLogger.Log(logLevel, eventId, in payload, exception, message, converter, options);
        }
        else if (logger.IsEnabled(logLevel))
        {
            LogPayloadAsState(logger, logLevel, eventId, in payload, exception, message, converter, options);
        }
    }
}
