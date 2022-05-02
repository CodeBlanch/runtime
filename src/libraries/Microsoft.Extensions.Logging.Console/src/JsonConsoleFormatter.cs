// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Payloads;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging.Console
{
    internal sealed class JsonConsoleFormatter : ConsoleFormatter, IDisposable
    {
        [ThreadStatic]
        private static LoggingPayloadJsonWriteTarget? s_JsonTarget;

        private IDisposable? _optionsReloadToken;

        public JsonConsoleFormatter(IOptionsMonitor<JsonConsoleFormatterOptions> options)
            : base(ConsoleFormatterNames.Json)
        {
            ReloadLoggerOptions(options.CurrentValue);
            _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
        }

        public override bool SupportsWritingPayload => true;

        public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
        {
            string message = logEntry.Formatter(logEntry.State, logEntry.Exception);
            if (logEntry.Exception == null && message == null)
            {
                return;
            }

            LogLevel logLevel = logEntry.LogLevel;
            string category = logEntry.Category;
            int eventId = logEntry.EventId.Id;
            Exception? exception = logEntry.Exception;

            LoggingPayloadJsonWriteTarget target = GetTarget();
            LoggingPayloadWriter writer = new(target);

            writer.BeginObject();
            var timestampFormat = FormatterOptions.TimestampFormat;
            if (timestampFormat != null)
            {
                WriteTimestamp(ref writer, timestampFormat);
            }
            writer.WriteProperty(nameof(logEntry.EventId), eventId);
            writer.WriteProperty(nameof(logEntry.LogLevel), GetLogLevelString(logLevel));
            writer.WriteProperty(nameof(logEntry.Category), category);
            writer.WriteProperty("Message", message);

            if (exception != null)
            {
                string exceptionMessage = exception.ToString();
                if (!FormatterOptions.JsonWriterOptions.Indented)
                {
                    exceptionMessage = exceptionMessage.Replace(Environment.NewLine, " ");
                }
                writer.WriteProperty(nameof(Exception), exceptionMessage);
            }

            if (logEntry.State != null)
            {
                writer.BeginProperty(nameof(logEntry.State));
                writer.BeginObject();
                writer.WriteProperty("Message", logEntry.State.ToString());
                if (logEntry.State is IReadOnlyCollection<KeyValuePair<string, object>> stateProperties)
                {
                    foreach (KeyValuePair<string, object> item in stateProperties)
                    {
                        WriteItem(ref writer, item);
                    }
                }
                writer.EndObject();
                writer.EndProperty();
            }
            WriteScopeInformation(ref writer, target, scopeProvider);
            writer.EndObject();
            writer.Flush();

            target.CopyTo(textWriter);
            textWriter.Write(Environment.NewLine);
        }

        public override void Write<TPayload>(in LogPayloadEntry<TPayload> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
        {
            if (logEntry.Exception == null && logEntry.Message == null)
            {
                return;
            }

            LogLevel logLevel = logEntry.LogLevel;
            string category = logEntry.Category;
            int eventId = logEntry.EventId.Id;
            Exception? exception = logEntry.Exception;

            LoggingPayloadJsonWriteTarget target = GetTarget();
            LoggingPayloadWriter writer = new(target);

            writer.BeginObject();
            var timestampFormat = FormatterOptions.TimestampFormat;
            if (timestampFormat != null)
            {
                WriteTimestamp(ref writer, timestampFormat);
            }
            writer.WriteProperty(nameof(logEntry.EventId), eventId);
            writer.WriteProperty(nameof(logEntry.LogLevel), GetLogLevelString(logLevel));
            writer.WriteProperty(nameof(logEntry.Category), category);
            writer.WriteProperty(nameof(logEntry.Message), logEntry.Message);

            if (exception != null)
            {
                string exceptionMessage = exception.ToString();
                if (!FormatterOptions.JsonWriterOptions.Indented)
                {
                    exceptionMessage = exceptionMessage.Replace(Environment.NewLine, " ");
                }
                writer.WriteProperty(nameof(Exception), exceptionMessage);
            }

            WriteScopeInformation(ref writer, target, scopeProvider);

            ref readonly TPayload? payload = ref LogPayloadEntry<TPayload>.GetPayload(in logEntry);

            writer.BeginProperty("Payload");
            LoggingPayloadSerializer.Serialize(target, in payload, logEntry.Converter, logEntry.Options);
            writer.EndProperty();

            writer.EndObject();
            writer.Flush();

            target.CopyTo(textWriter);
            textWriter.Write(Environment.NewLine);
        }

        private static LoggingPayloadJsonWriteTarget GetTarget()
        {
            LoggingPayloadJsonWriteTarget target = s_JsonTarget ??= new LoggingPayloadJsonWriteTarget();

            target.Reset();

            return target;
        }

        private static string GetLogLevelString(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace => "Trace",
                LogLevel.Debug => "Debug",
                LogLevel.Information => "Information",
                LogLevel.Warning => "Warning",
                LogLevel.Error => "Error",
                LogLevel.Critical => "Critical",
                _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
            };
        }

        private void WriteTimestamp(ref LoggingPayloadWriter writer, string? timestampFormat)
        {
            DateTimeOffset dateTimeOffset = FormatterOptions.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;
#if NET6_0_OR_GREATER
            Span<char> data = stackalloc char[64];
            if (dateTimeOffset.TryFormat(data, out int charsWritten, timestampFormat))
            {
                writer.WriteProperty("Timestamp", data.Slice(0, charsWritten));
            }
            else
#endif
            {
                writer.WriteProperty("Timestamp", dateTimeOffset.ToString(timestampFormat));
            }
        }

        private void WriteScopeInformation(ref LoggingPayloadWriter writer, LoggingPayloadJsonWriteTarget target, IExternalScopeProvider? scopeProvider)
        {
            if (FormatterOptions.IncludeScopes && scopeProvider != null)
            {
                writer.BeginProperty("Scopes");
                writer.BeginArray();

                scopeProvider.ForEachScope(static (scope, state) =>
                {
                    LoggingPayloadWriter writer = new(state);
                    if (scope is IEnumerable<KeyValuePair<string, object>> scopeItems)
                    {
                        writer.BeginObject();
                        writer.WriteProperty("Message", scope.ToString());
                        foreach (KeyValuePair<string, object> item in scopeItems)
                        {
                            WriteItem(ref writer, item);
                        }
                        writer.EndObject();
                    }
                    else
                    {
                        string? s = ToInvariantString(scope);
                        if (s != null)
                            writer.WriteValue(s);
                    }
                }, target);

                writer.EndArray();
                writer.EndProperty();
            }
        }

        private static void WriteItem(ref LoggingPayloadWriter writer, KeyValuePair<string, object> item)
        {
            var key = item.Key;
            switch (item.Value)
            {
                case bool boolValue:
                    writer.WriteProperty(key, boolValue);
                    break;
                case byte byteValue:
                    writer.WriteProperty(key, byteValue);
                    break;
                case sbyte sbyteValue:
                    writer.WriteProperty(key, sbyteValue);
                    break;
                case char charValue:
                    writer.WriteProperty(key, charValue);
                    break;
                case decimal decimalValue:
                    writer.WriteProperty(key, decimalValue);
                    break;
                case double doubleValue:
                    writer.WriteProperty(key, doubleValue);
                    break;
                case float floatValue:
                    writer.WriteProperty(key, floatValue);
                    break;
                case int intValue:
                    writer.WriteProperty(key, intValue);
                    break;
                case uint uintValue:
                    writer.WriteProperty(key, uintValue);
                    break;
                case long longValue:
                    writer.WriteProperty(key, longValue);
                    break;
                case ulong ulongValue:
                    writer.WriteProperty(key, ulongValue);
                    break;
                case short shortValue:
                    writer.WriteProperty(key, shortValue);
                    break;
                case ushort ushortValue:
                    writer.WriteProperty(key, ushortValue);
                    break;
                case null:
                    writer.WriteNullProperty(key);
                    break;
                default:
                    writer.WriteProperty(key, ToInvariantString(item.Value));
                    break;
            }
        }

        private static string? ToInvariantString(object? obj) => Convert.ToString(obj, CultureInfo.InvariantCulture);

        internal JsonConsoleFormatterOptions FormatterOptions { get; set; }

        [MemberNotNull(nameof(FormatterOptions))]
        private void ReloadLoggerOptions(JsonConsoleFormatterOptions options)
        {
            FormatterOptions = options;
        }

        public void Dispose()
        {
            _optionsReloadToken?.Dispose();
        }
    }
}
