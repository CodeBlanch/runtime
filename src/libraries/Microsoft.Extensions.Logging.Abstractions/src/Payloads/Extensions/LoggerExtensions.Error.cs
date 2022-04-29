// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

using Microsoft.Extensions.Logging.Payloads;

namespace Microsoft.Extensions.Logging;

public static partial class LoggerExtensions
{
    public static void LogError<TPayload>(
        this ILogger logger,
        EventId eventId,
        in TPayload? payload,
        string message,
        LoggingPayloadConverter<TPayload>? converter = null,
        LoggingPayloadSerializerOptions? options = null)
    {
        Log(
            logger,
            LogLevel.Error,
            eventId,
            in payload,
            exception: null,
            message,
            converter,
            options);
    }

    public static void LogError<TPayload>(
        this ILogger logger,
        in TPayload? payload,
        string message,
        LoggingPayloadConverter<TPayload>? converter = null,
        LoggingPayloadSerializerOptions? options = null)
    {
        Log(
            logger,
            LogLevel.Error,
            eventId: default,
            in payload,
            exception: null,
            message,
            converter,
            options);
    }

    public static void LogError<TPayload>(
        this ILogger logger,
        in TPayload? payload,
        Exception? exception,
        string message,
        LoggingPayloadConverter<TPayload>? converter = null,
        LoggingPayloadSerializerOptions? options = null)
    {
        Log(
            logger,
            LogLevel.Error,
            eventId: default,
            in payload,
            exception,
            message,
            converter,
            options);
    }

    public static void LogError<TPayload>(
        this ILogger logger,
        EventId eventId,
        in TPayload? payload,
        Exception? exception,
        string message,
        LoggingPayloadConverter<TPayload>? converter = null,
        LoggingPayloadSerializerOptions? options = null)
    {
        Log(
            logger,
            LogLevel.Error,
            eventId,
            in payload,
            exception,
            message,
            converter,
            options);
    }
}
