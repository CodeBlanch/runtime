// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Microsoft.Extensions.Logging.Payloads;

public readonly ref struct LogPayloadEntry<TPayload>
{
    // TODO: Use ref field here when it is available.
    private readonly TPayload? payload;

    /// <summary>
    /// Initializes an instance of the LogPayloadEntry struct.
    /// </summary>
    /// <param name="logLevel">The log level.</param>
    /// <param name="category">The category name for the log.</param>
    /// <param name="eventId">The log event Id.</param>
    /// <param name="payload">The payload for which log is being written.</param>
    /// <param name="exception">The log exception.</param>
    /// <param name="message">The log message.</param>
    /// <param name="converter">The log payload converter.</param>
    /// <param name="options">The log payload serializer options.</param>
    public LogPayloadEntry(
        LogLevel logLevel,
        string category,
        EventId eventId,
        in TPayload? payload,
        Exception? exception,
        string message,
        LoggingPayloadConverter<TPayload>? converter = null,
        LoggingPayloadSerializerOptions? options = null)
    {
        LogLevel = logLevel;
        Category = category;
        EventId = eventId;
        this.payload = payload;
        Exception = exception;
        Message = message;
        Converter = converter;
        Options = options;
    }

    /// <summary>
    /// Gets the LogLevel
    /// </summary>
    public LogLevel LogLevel { get; }

    /// <summary>
    /// Gets the log category
    /// </summary>
    public string Category { get; }

    /// <summary>
    /// Gets the log EventId
    /// </summary>
    public EventId EventId { get; }

    /// <summary>
    /// Gets the log exception
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// Gets the log message
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the log payload converter
    /// </summary>
    public LoggingPayloadConverter<TPayload>? Converter { get; }

    /// <summary>
    /// Gets the log payload serializer options
    /// </summary>
    public LoggingPayloadSerializerOptions? Options { get; }

    /// <summary>
    /// Gets a reference to the log payload.
    /// </summary>
    /// <param name="logEntry">Log entry.</param>
    /// <returns>Payload reference</returns>
    public static ref readonly TPayload? GetPayload(in LogPayloadEntry<TPayload> logEntry)
    {
        return ref logEntry.payload;
    }
}
