// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Extensions.Logging.Payloads;

namespace Microsoft.Extensions.Logging;

public static partial class LoggerExtensions
{
    [ThreadStatic]
    private static LoggingPayloadWriteTarget? s_WriteTarget;

    internal static LoggingPayloadWriteTarget GetDefaultLoggingPayloadWriteTarget(LoggingPayloadSerializerOptions? options = null)
    {
        if (options?.DefaultWriteTargetFactory == null || options.IsDefaultInstance)
        {
            LoggingPayloadWriteTarget target = s_WriteTarget ??= (options ?? LoggingPayloadSerializerOptions.Default).DefaultWriteTargetFactory!();
            target.Reset();
            return target;
        }

        return options.DefaultWriteTargetFactory();
    }

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
        else
        {
            LogLegacyPayload(logger, logLevel, eventId, in payload, exception, message, converter, options);
        }
    }

    internal static void LogLegacyPayload<TPayload>(
        ILogger logger,
        LogLevel logLevel,
        EventId eventId,
        in TPayload? payload,
        Exception? exception,
        string message,
        LoggingPayloadConverter<TPayload>? converter = null,
        LoggingPayloadSerializerOptions? options = null)
    {
        if (logger.IsEnabled(logLevel))
        {
            LoggingPayloadWriteTarget target = GetDefaultLoggingPayloadWriteTarget(options);

            LoggingPayloadSerializer.Serialize(target, in payload, converter, options);

            logger.Log(logLevel, eventId, new FormattedPayload(message, target), exception, s_Formatter);
        }
    }

    private static readonly Func<FormattedPayload, Exception?, string> s_Formatter = (f, e) => f.ToString();

    private readonly struct FormattedPayload : IReadOnlyList<KeyValuePair<string, object?>>
    {
        private readonly string _Message;
        private readonly LoggingPayloadWriteTarget _Target;

        public FormattedPayload(string message, LoggingPayloadWriteTarget target)
        {
            _Message = message;
            _Target = target;
        }

        public int Count => 2;

        public KeyValuePair<string, object?> this[int index]
            => index == 0
                ? new KeyValuePair<string, object?>("{OriginalFormat}", _Message)
                : index == 1
                    ? new KeyValuePair<string, object?>("Payload", _Target.ToString())
                    : throw new IndexOutOfRangeException(nameof(index));

        public override string ToString() => $"{_Message} {_Target}";

        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
