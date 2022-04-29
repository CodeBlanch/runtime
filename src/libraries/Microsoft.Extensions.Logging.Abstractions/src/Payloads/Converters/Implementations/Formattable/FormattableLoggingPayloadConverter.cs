// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if !NET6_0_OR_GREATER
using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Extensions.Logging.Payloads;

internal delegate string FormatAction<T>(
    in T value,
    string? format,
    IFormatProvider? provider);

internal sealed class FormattableLoggingPayloadConverter<T> : LoggingPayloadConverter<T>
{
    private readonly FormatAction<T> _FormatAction;
    private readonly string? _Format;

    public FormattableLoggingPayloadConverter(FormatAction<T> formatAction, string? format = null)
    {
        _FormatAction = formatAction;
        _Format = format;
    }

    public override void Write(in T value, ref LoggingPayloadWriter writer)
    {
        Debug.Assert(value != null);

        string stringValue = _FormatAction(in value, _Format, CultureInfo.InvariantCulture);

        writer.WriteValue(stringValue);
    }
}
#endif
