// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging.Payloads;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Delegates to a new <see cref="ILogger"/> instance using the full name of the given type, created by the
    /// provided <see cref="ILoggerFactory"/>.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    public class Logger<T> : ILogger<T>, IPayloadLogger
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Creates a new <see cref="Logger{T}"/>.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public Logger(ILoggerFactory factory)
        {
            ThrowHelper.ThrowIfNull(factory);

            _logger = factory.CreateLogger(TypeNameHelper.GetTypeDisplayName(typeof(T), includeGenericParameters: false, nestedTypeDelimiter: '.'));
        }

        /// <inheritdoc />
        IDisposable? ILogger.BeginScope<TState>(TState state)
        {
            return _logger.BeginScope(state);
        }

        /// <inheritdoc />
        bool ILogger.IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(logLevel);
        }

        /// <inheritdoc />
        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            _logger.Log(logLevel, eventId, state, exception, formatter);
        }

        /// <inheritdoc />
        void IPayloadLogger.Log<TPayload>(LogLevel logLevel, EventId eventId, in TPayload? payload, Exception? exception, string message, LoggingPayloadConverter<TPayload>? converter, LoggingPayloadSerializerOptions? options)
            where TPayload : default
        {
            if (_logger is IPayloadLogger payloadLogger)
            {
                payloadLogger.Log(logLevel, eventId, in payload, exception, message, converter, options);
            }
            else
            {
                LoggerExtensions.LogPayloadAsState(_logger, logLevel, eventId, in payload, exception, message, converter, options);
            }
        }
    }
}
