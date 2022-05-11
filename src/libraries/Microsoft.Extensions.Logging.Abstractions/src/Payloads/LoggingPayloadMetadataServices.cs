// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
#if NETCOREAPP
using System.Runtime.CompilerServices;
#endif

namespace Microsoft.Extensions.Logging.Payloads;

public static class LoggingPayloadMetadataServices
{
    private static LoggingPayloadConverter<DateTime>? s_DateTimeConverter;
    private static LoggingPayloadConverter<DateTimeOffset>? s_DateTimeOffsetConverter;
    private static LoggingPayloadConverter<Guid>? s_GuidConverter;
    private static LoggingPayloadConverter<TimeSpan>? s_TimeSpanConverter;
    private static LoggingPayloadConverter<Version>? s_VersionConverter;
    private static LoggingPayloadConverter<Uri>? s_UriConverter;
#if NET6_0_OR_GREATER
    private static LoggingPayloadConverter<DateOnly>? s_DateOnlyConverter;
    private static LoggingPayloadConverter<TimeOnly>? s_TimeOnlyConverter;
#endif

#if NET6_0_OR_GREATER
    public static LoggingPayloadConverter<DateOnly> DateOnlyConverter => s_DateOnlyConverter ??= new SpanFormattableLoggingPayloadConverter<DateOnly>(
        (in DateOnly value, Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => value.TryFormat(destination, out charsWritten, format, provider),
        "O");

    public static LoggingPayloadConverter<DateTime> DateTimeConverter => s_DateTimeConverter ??= new SpanFormattableLoggingPayloadConverter<DateTime>(
        (in DateTime value, Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => value.TryFormat(destination, out charsWritten, format, provider),
        "O");

    public static LoggingPayloadConverter<DateTimeOffset> DateTimeOffsetConverter => s_DateTimeOffsetConverter ??= new SpanFormattableLoggingPayloadConverter<DateTimeOffset>(
        (in DateTimeOffset value, Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => value.TryFormat(destination, out charsWritten, format, provider),
        "O");

    public static LoggingPayloadConverter<Guid> GuidConverter => s_GuidConverter ??= new SpanFormattableLoggingPayloadConverter<Guid>(
        (in Guid value, Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => value.TryFormat(destination, out charsWritten, format),
        "N");

    public static LoggingPayloadConverter<TimeOnly> TimeOnlyConverter => s_TimeOnlyConverter ??= new SpanFormattableLoggingPayloadConverter<TimeOnly>(
        (in TimeOnly value, Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => value.TryFormat(destination, out charsWritten, format, provider),
        "O");

    public static LoggingPayloadConverter<TimeSpan> TimeSpanConverter => s_TimeSpanConverter ??= new SpanFormattableLoggingPayloadConverter<TimeSpan>(
        (in TimeSpan value, Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => value.TryFormat(destination, out charsWritten, format, provider),
        "c");

    public static LoggingPayloadConverter<Version> VersionConverter => s_VersionConverter ??= new SpanFormattableLoggingPayloadConverter<Version>(
        (in Version value, Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => value.TryFormat(destination, out charsWritten));
#else
    public static LoggingPayloadConverter<DateTime> DateTimeConverter => s_DateTimeConverter ??= new FormattableLoggingPayloadConverter<DateTime>(
        (in DateTime value, string? format, IFormatProvider? provider) => value.ToString(format, provider),
        "O");

    public static LoggingPayloadConverter<DateTimeOffset> DateTimeOffsetConverter => s_DateTimeOffsetConverter ??= new FormattableLoggingPayloadConverter<DateTimeOffset>(
        (in DateTimeOffset value, string? format, IFormatProvider? provider) => value.ToString(format, provider),
        "O");

    public static LoggingPayloadConverter<Guid> GuidConverter => s_GuidConverter ??= new FormattableLoggingPayloadConverter<Guid>(
        (in Guid value, string? format, IFormatProvider? provider) => value.ToString(format),
        "N");

    public static LoggingPayloadConverter<TimeSpan> TimeSpanConverter => s_TimeSpanConverter ??= new FormattableLoggingPayloadConverter<TimeSpan>(
        (in TimeSpan value, string? format, IFormatProvider? provider) => value.ToString(format, provider),
        "c");

    public static LoggingPayloadConverter<Version> VersionConverter => s_VersionConverter ??= new ToStringLoggingPayloadConverter<Version>();
#endif

    public static LoggingPayloadConverter<Uri> UriConverter => s_UriConverter ??= new ToStringLoggingPayloadConverter<Uri>();

    public static LoggingPayloadConverter<TObject> CreateObjectConverter<
        [DynamicallyAccessedMembers(
            DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties
            | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)]
    TObject>()
#if NETCOREAPP
        => RuntimeFeature.IsDynamicCodeSupported
            ? new EmitObjectLoggingPayloadConverter<TObject>()
            : new ReflectionObjectLoggingPayloadConverter<TObject>();
#elif NETFRAMEWORK
        => new EmitObjectLoggingPayloadConverter<TObject>();
#else
        => new ReflectionObjectLoggingPayloadConverter<TObject>();
#endif

    public static LoggingPayloadConverter<TEnum> CreateNumericEnumConverter<TEnum>()
        where TEnum : struct, Enum
        => new NumericEnumLoggingPayloadConverter<TEnum>();

    public static LoggingPayloadConverter<TEnum> CreateStringEnumConverter<TEnum>(bool? writeUnknownEnumValuesAsNumeric = null)
        where TEnum : struct, Enum
        => new StringEnumLoggingPayloadConverter<TEnum>(writeUnknownEnumValuesAsNumeric);
}
