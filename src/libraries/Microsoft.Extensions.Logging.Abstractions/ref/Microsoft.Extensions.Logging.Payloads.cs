// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// ------------------------------------------------------------------------------
// Changes to this file must follow the https://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace Microsoft.Extensions.Logging
{
    public interface IPayloadLogger : Microsoft.Extensions.Logging.ILogger
    {
        void Log<TPayload>(
            Microsoft.Extensions.Logging.LogLevel logLevel,
            Microsoft.Extensions.Logging.EventId eventId,
            in TPayload? payload,
            System.Exception? exception,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null);
    }

    public static partial class LoggerExtensions
    {
        public static void LogPayload<TPayload>(
            this Microsoft.Extensions.Logging.ILogger logger,
            Microsoft.Extensions.Logging.LogLevel logLevel,
            Microsoft.Extensions.Logging.EventId eventId,
            in TPayload? payload,
            System.Exception? exception,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }

        public static void LogPayloadAsState<TPayload>(
            Microsoft.Extensions.Logging.ILogger logger,
            Microsoft.Extensions.Logging.LogLevel logLevel,
            Microsoft.Extensions.Logging.EventId eventId,
            in TPayload? payload,
            System.Exception? exception,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }

        public static void LogCriticalPayload<TPayload>(
            this Microsoft.Extensions.Logging.ILogger logger,
            Microsoft.Extensions.Logging.EventId eventId,
            in TPayload? payload,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }

        public static void LogCriticalPayload<TPayload>(
            this Microsoft.Extensions.Logging.ILogger logger,
            in TPayload? payload,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }

        public static void LogCriticalPayload<TPayload>(
            this Microsoft.Extensions.Logging.ILogger logger,
            in TPayload? payload,
            System.Exception? exception,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }

        public static void LogCriticalPayload<TPayload>(
            this Microsoft.Extensions.Logging.ILogger logger,
            Microsoft.Extensions.Logging.EventId eventId,
            in TPayload? payload,
            System.Exception? exception,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }

        public static void LogDebugPayload<TPayload>(
            this Microsoft.Extensions.Logging.ILogger logger,
            Microsoft.Extensions.Logging.EventId eventId,
            in TPayload? payload,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }

        public static void LogDebugPayload<TPayload>(
            this Microsoft.Extensions.Logging.ILogger logger,
            in TPayload? payload,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }

        public static void LogDebugPayload<TPayload>(
            this Microsoft.Extensions.Logging.ILogger logger,
            in TPayload? payload,
            System.Exception? exception,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }

        public static void LogDebugPayload<TPayload>(
            this Microsoft.Extensions.Logging.ILogger logger,
            Microsoft.Extensions.Logging.EventId eventId,
            in TPayload? payload,
            System.Exception? exception,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }

        public static void LogErrorPayload<TPayload>(
            this Microsoft.Extensions.Logging.ILogger logger,
            Microsoft.Extensions.Logging.EventId eventId,
            in TPayload? payload,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }

        public static void LogErrorPayload<TPayload>(
            this Microsoft.Extensions.Logging.ILogger logger,
            in TPayload? payload,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }

        public static void LogErrorPayload<TPayload>(
            this Microsoft.Extensions.Logging.ILogger logger,
            in TPayload? payload,
            System.Exception? exception,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }

        public static void LogErrorPayload<TPayload>(
            this Microsoft.Extensions.Logging.ILogger logger,
            Microsoft.Extensions.Logging.EventId eventId,
            in TPayload? payload,
            System.Exception? exception,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }

        public static void LogInfoPayload<TPayload>(
            this Microsoft.Extensions.Logging.ILogger logger,
            Microsoft.Extensions.Logging.EventId eventId,
            in TPayload? payload,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }

        public static void LogInfoPayload<TPayload>(
            this Microsoft.Extensions.Logging.ILogger logger,
            in TPayload? payload,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }

        public static void LogInfoPayload<TPayload>(
            this Microsoft.Extensions.Logging.ILogger logger,
            in TPayload? payload,
            System.Exception? exception,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }

        public static void LogInfoPayload<TPayload>(
            this Microsoft.Extensions.Logging.ILogger logger,
            Microsoft.Extensions.Logging.EventId eventId,
            in TPayload? payload,
            System.Exception? exception,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }

        public static void LogTracePayload<TPayload>(
            this Microsoft.Extensions.Logging.ILogger logger,
            Microsoft.Extensions.Logging.EventId eventId,
            in TPayload? payload,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }

        public static void LogTracePayload<TPayload>(
            this Microsoft.Extensions.Logging.ILogger logger,
            in TPayload? payload,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }

        public static void LogTracePayload<TPayload>(
            this Microsoft.Extensions.Logging.ILogger logger,
            in TPayload? payload,
            System.Exception? exception,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }

        public static void LogTracePayload<TPayload>(
            this Microsoft.Extensions.Logging.ILogger logger,
            Microsoft.Extensions.Logging.EventId eventId,
            in TPayload? payload,
            System.Exception? exception,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }

        public static void LogWarningPayload<TPayload>(
            this Microsoft.Extensions.Logging.ILogger logger,
            Microsoft.Extensions.Logging.EventId eventId,
            in TPayload? payload,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }

        public static void LogWarningPayload<TPayload>(
            this Microsoft.Extensions.Logging.ILogger logger,
            in TPayload? payload,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }

        public static void LogWarningPayload<TPayload>(
            this Microsoft.Extensions.Logging.ILogger logger,
            in TPayload? payload,
            System.Exception? exception,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }

        public static void LogWarningPayload<TPayload>(
            this Microsoft.Extensions.Logging.ILogger logger,
            Microsoft.Extensions.Logging.EventId eventId,
            in TPayload? payload,
            System.Exception? exception,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }
    }
}

namespace Microsoft.Extensions.Logging.Payloads
{
    public readonly ref struct LogPayloadEntry<TPayload>
    {
        public LogPayloadEntry(
            Microsoft.Extensions.Logging.LogLevel logLevel,
            string category,
            Microsoft.Extensions.Logging.EventId eventId,
            in TPayload? payload,
            System.Exception? exception,
            string message,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }
        public Microsoft.Extensions.Logging.LogLevel LogLevel { get { throw null; } }
        public string Category { get { throw null; } }
        public Microsoft.Extensions.Logging.EventId EventId { get { throw null; } }
        public System.Exception? Exception { get { throw null; } }
        public string Message { get { throw null; } }
        public Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? Converter { get { throw null; } }
        public Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? Options { get { throw null; } }
        public static ref readonly TPayload? GetPayload(in LogPayloadEntry<TPayload> logEntry) => throw null;
    }

    public abstract class LoggingPayloadConverter
    {
        internal LoggingPayloadConverter() { }
    }

    public abstract class LoggingPayloadConverter<T> : Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter
    {
        protected LoggingPayloadConverter() { }
        public abstract void Write(in T value, ref Microsoft.Extensions.Logging.Payloads.LoggingPayloadWriter writer);
    }

    public class LoggingPayloadSerializerOptions
    {
        public static LoggingPayloadSerializerOptions Default { get; }
        public bool IsDefaultInstance { get; }
        public LoggingPayloadSerializerOptions() { }
        public System.Func<Microsoft.Extensions.Logging.Payloads.LoggingPayloadWriteTarget>? DefaultWriteTargetFactory { get; set; }
        public System.Collections.Generic.IDictionary<System.Type, Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter> Converters { get; }
        public Microsoft.Extensions.Logging.Payloads.LoggingPayloadIgnoreCondition DefaultIgnoreCondition { get; set; }
        public int MaxDepth { get; set; }
        public int MaxArrayLength { get; set; }
        public int MaxPropertyCount { get; set; }
        public bool IncludePublicFields { get; set; }
        public LoggingPayloadEnumOptions EnumOptions { get; set; }
    }

    public class LoggingPayloadEnumOptions
    {
        public bool WriteEnumValuesAsStrings { get; set; }
        public bool WriteUnknownEnumValuesAsNumeric { get; set; }
    }

    public class LoggingPayloadMaskOptions
    {
        public char MaskCharacter { get; set; }
        public int MaskLength { get; set; }
        public int UnmaskedStartingCharacterCount { get; set; }
        public int UnmaskedEndingCharacterCount { get; set; }
    }

    public enum LoggingPayloadIgnoreCondition
    {
        Never,
        Always,
        WhenWritingNull,
        WhenWritingNullOrDefault,
    }

    public readonly ref partial struct LoggingPayloadWriter
    {
        public LoggingPayloadWriter(
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadWriteTarget target,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }

        public void BeginObject(string? typeName = null) { }
        public void EndObject() { }
        public void BeginArray(string? typeName = null) { }
        public void EndArray() { }
        public void BeginProperty(string propertyName) { }
        public void EndProperty() { }
        public void Flush() { }
        public void WriteNullProperty(string propertyName) { }
        public void WriteProperty(string propertyName, int value) { }
        public void WriteProperty(string propertyName, int? value) { }
        [System.CLSCompliant(false)]
        public void WriteProperty(string propertyName, uint value) { }
        [System.CLSCompliant(false)]
        public void WriteProperty(string propertyName, uint? value) { }
        public void WriteProperty(string propertyName, long value) { }
        public void WriteProperty(string propertyName, long? value) { }
        [System.CLSCompliant(false)]
        public void WriteProperty(string propertyName, ulong value) { }
        [System.CLSCompliant(false)]
        public void WriteProperty(string propertyName, ulong? value) { }
        public void WriteProperty(string propertyName, short value) { }
        public void WriteProperty(string propertyName, short? value) { }
        [System.CLSCompliant(false)]
        public void WriteProperty(string propertyName, ushort value) { }
        [System.CLSCompliant(false)]
        public void WriteProperty(string propertyName, ushort? value) { }
        public void WriteProperty(string propertyName, byte value) { }
        public void WriteProperty(string propertyName, byte? value) { }
        [System.CLSCompliant(false)]
        public void WriteProperty(string propertyName, sbyte value) { }
        [System.CLSCompliant(false)]
        public void WriteProperty(string propertyName, sbyte? value) { }
        public void WriteProperty(string propertyName, char value) { }
        public void WriteProperty(string propertyName, char? value) { }
        public void WriteProperty(string propertyName, double value) { }
        public void WriteProperty(string propertyName, double? value) { }
        public void WriteProperty(string propertyName, float value) { }
        public void WriteProperty(string propertyName, float? value) { }
        public void WriteProperty(string propertyName, decimal value) { }
        public void WriteProperty(string propertyName, decimal? value) { }
        public void WriteProperty(string propertyName, byte[]? value) { }
        public void WriteProperty(string propertyName, bool value) { }
        public void WriteProperty(string propertyName, bool? value) { }
        public void WriteProperty(string propertyName, System.ReadOnlySpan<byte> value) { }

        public void WriteProperty(
            string propertyName,
            string? value,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadMaskOptions? maskOptions = null)
        {
        }
        public void WriteProperty(
            string propertyName,
            System.ReadOnlySpan<char> value,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadMaskOptions? maskOptions = null)
        {
        }
        public void WriteProperty<T>(
            string propertyName,
            System.Collections.Generic.IEnumerable<T>? value)
        {
        }
        public void WritePropertyObject<T>(
            string propertyName,
            T? value,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<T>? converter = null)
            where T : class
        {
        }
        public void WritePropertyStruct<T>(
            string propertyName,
            in T value,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<T>? converter = null)
            where T : struct
        {
        }
        public void WritePropertyNullable<T>(
            string propertyName,
            in T? value,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<T>? converter = null)
            where T : struct
        {
        }

        public void WriteNullValue() { }
        public void WriteValue(int value) { }
        [System.CLSCompliant(false)]
        public void WriteValue(uint value) { }
        public void WriteValue(long value) { }
        [System.CLSCompliant(false)]
        public void WriteValue(ulong value) { }
        public void WriteValue(short value) { }
        [System.CLSCompliant(false)]
        public void WriteValue(ushort value) { }
        public void WriteValue(byte value) { }
        [System.CLSCompliant(false)]
        public void WriteValue(sbyte value) { }
        public void WriteValue(char value) { }
        public void WriteValue(double value) { }
        public void WriteValue(float value) { }
        public void WriteValue(decimal value) { }
        public void WriteValue(byte[] value) { }
        public void WriteValue(bool value) { }
        public void WriteValue(System.ReadOnlySpan<byte> value) { }

        public void WriteValue(
            string value,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadMaskOptions? maskOptions = null)
        {
        }
        public void WriteValue(
            System.ReadOnlySpan<char> value,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadMaskOptions? maskOptions = null)
        {
        }
        public void WriteValue<T>(
            string propertyName,
            System.Collections.Generic.IEnumerable<T> value)
        {
        }
        public void WriteValueObject<T>(
            string propertyName,
            T value,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<T>? converter = null)
            where T : class
        {
        }
        public void WriteValueStruct<T>(
            string propertyName,
            in T value,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<T>? converter = null)
            where T : struct
        {
        }
    }

    public class LoggingPayloadWriteTarget
    {
        public virtual void Reset() { }
        public virtual void OnBeginObject() { }
        public virtual void OnEndObject() { }
        public virtual void OnBeginArray() { }
        public virtual void OnEndArray() { }
        public virtual void OnBeginProperty(string propertyName) { }
        public virtual void OnEndProperty() { }
        public virtual void OnWriteSeparator() { }
        public virtual void OnWriteNullValue() { }
        public virtual void OnWriteValue(string value) { }
        public virtual void OnWriteValue(System.ReadOnlySpan<char> value) { }
        public virtual void OnWriteValue(int value) { }
        [System.CLSCompliant(false)]
        public virtual void OnWriteValue(uint value) { }
        public virtual void OnWriteValue(long value) { }
        [System.CLSCompliant(false)]
        public virtual void OnWriteValue(ulong value) { }
        public virtual void OnWriteValue(byte value) { }
        [System.CLSCompliant(false)]
        public virtual void OnWriteValue(sbyte value) { }
        public virtual void OnWriteValue(char value) { }
        public virtual void OnWriteValue(short value) { }
        [System.CLSCompliant(false)]
        public virtual void OnWriteValue(ushort value) { }
        public virtual void OnWriteValue(double value) { }
        public virtual void OnWriteValue(float value) { }
        public virtual void OnWriteValue(decimal value) { }
        public virtual void OnWriteValue(byte[] value) { }
        public virtual void OnWriteValue(System.ReadOnlySpan<byte> value) { }
        public virtual void OnWriteValue(bool value) { }
        public virtual void Flush() { }
    }

    public sealed class LoggingPayloadJsonWriteTarget : Microsoft.Extensions.Logging.Payloads.LoggingPayloadWriteTarget
    {
        public LoggingPayloadJsonWriteTarget(int initialCapacity = 1024) { }
        public void CopyTo(System.IO.TextWriter textWriter) { }
        public void CopyTo(System.Text.StringBuilder stringBuilder) { }
    }

    public sealed class LoggingPayloadStringWriteTarget : Microsoft.Extensions.Logging.Payloads.LoggingPayloadWriteTarget
    {
        public LoggingPayloadStringWriteTarget(int initialCapacity = 1024) { }
        public void CopyTo(System.IO.TextWriter textWriter) { }
        public void CopyTo(System.Text.StringBuilder stringBuilder) { }
    }

    public static class LoggingPayloadMetadataServices
    {
#if NET6_0_OR_GREATER
        public static LoggingPayloadConverter<System.DateOnly> DateOnlyConverter { get; }
        public static LoggingPayloadConverter<System.TimeOnly> TimeOnlyConverter { get; }
#endif
        public static LoggingPayloadConverter<System.DateTime> DateTimeConverter { get; }
        public static LoggingPayloadConverter<System.DateTimeOffset> DateTimeOffsetConverter { get; }
        public static LoggingPayloadConverter<System.Guid> GuidConverter { get; }
        public static LoggingPayloadConverter<System.TimeSpan> TimeSpanConverter { get; }
        public static LoggingPayloadConverter<System.Version> VersionConverter { get; }
        public static LoggingPayloadConverter<System.Uri> UriConverter { get; }

        public static LoggingPayloadConverter<T[]> CreateArrayConverter<T>() => throw null;
        public static LoggingPayloadConverter<TEnumerable> CreateEnumerableConverter<TEnumerable, TItem>()
            where TEnumerable : System.Collections.Generic.IEnumerable<TItem> => throw null;
        public static LoggingPayloadConverter<TDictionary> CreateDictionaryConverter<TDictionary, TValue>()
            where TDictionary : System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, TValue>> => throw null;
        public static LoggingPayloadConverter<TObject> CreateObjectConverter<
            [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(
                System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicProperties | System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.NonPublicProperties
                | System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicFields | System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.NonPublicFields)]
        TObject>() => throw null;
        public static LoggingPayloadConverter<TEnum> CreateNumericEnumConverter<TEnum>()
            where TEnum : struct, System.Enum => throw null;
        public static LoggingPayloadConverter<TEnum> CreateStringEnumConverter<TEnum>(bool? writeUnknownEnumValuesAsNumeric = null)
            where TEnum : struct, System.Enum => throw null;
    }

    [System.Serializable]
    public sealed class LoggingPayloadSerializationException : System.Exception
    {
        public LoggingPayloadSerializationException() { }
        public LoggingPayloadSerializationException(string message) { }
        public LoggingPayloadSerializationException(string? message, System.Exception? innerException) { }
    }

    public static class LoggingPayloadSerializer
    {
        public static void Serialize<TPayload>(
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadWriteTarget target,
            in TPayload? payload,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions options)
        {
        }
        public static void Serialize<TPayload>(
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadWriteTarget target,
            in TPayload? payload,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload> converter)
        {
        }
        public static void Serialize<TPayload>(
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadWriteTarget target,
            in TPayload? payload,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadConverter<TPayload>? converter = null,
            Microsoft.Extensions.Logging.Payloads.LoggingPayloadSerializerOptions? options = null)
        {
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct | System.AttributeTargets.Property | System.AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class LoggingPayloadConverterAttribute : System.Attribute
    {
        public System.Type ConverterType { get; }

        public LoggingPayloadConverterAttribute(
            [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicConstructors | System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.NonPublicConstructors)]
            System.Type converterType)
        {
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class LoggingPayloadIgnoreAttribute : System.Attribute
    {
        public Microsoft.Extensions.Logging.Payloads.LoggingPayloadIgnoreCondition Condition { get; set; }
    }

    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class LoggingPayloadIncludeAttribute : System.Attribute
    {
    }

    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class LoggingPayloadMaskAttribute : System.Attribute
    {
        public char MaskCharacter { get; set; }
        public int MaskLength { get; set; }
        public int UnmaskedStartingCharacterCount { get; set; }
        public int UnmaskedEndingCharacterCount { get; set; }
    }

    [System.AttributeUsage(System.AttributeTargets.Enum | System.AttributeTargets.Property | System.AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class LoggingPayloadNumericEnumAttribute : System.Attribute
    {
    }

    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class LoggingPayloadPropertyNameAttribute : System.Attribute
    {
        public string PropertyName { get; }

        public LoggingPayloadPropertyNameAttribute(string propertyName)
        {
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Enum | System.AttributeTargets.Property | System.AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class LoggingPayloadStringEnumAttribute : System.Attribute
    {
        public bool WriteUnknownEnumValuesAsNumeric { get; set; }
    }
}
