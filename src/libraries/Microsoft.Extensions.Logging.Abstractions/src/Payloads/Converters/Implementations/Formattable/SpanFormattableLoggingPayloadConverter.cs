// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET6_0_OR_GREATER
using System;
using System.Buffers;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Extensions.Logging.Payloads;

internal delegate bool TryFormatAction<T>(
    in T value,
    Span<char> destination,
    out int charsWritten,
    ReadOnlySpan<char> format,
    IFormatProvider? provider);

internal sealed class SpanFormattableLoggingPayloadConverter<T> : LoggingPayloadConverter<T>
{
    private readonly TryFormatAction<T> _TryFormatAction;
    private readonly string? _Format;

    public SpanFormattableLoggingPayloadConverter(TryFormatAction<T> tryFormatAction, string? format = null)
    {
        _TryFormatAction = tryFormatAction;
        _Format = format;
    }

    public override void Write(in T value, ref LoggingPayloadWriter writer)
    {
        Debug.Assert(value != null);

        Span<char> destination = stackalloc char[128];
        if (_TryFormatAction(in value, destination, out int charsWritten, _Format, CultureInfo.InvariantCulture))
        {
            writer.WriteValue(destination[..charsWritten]);
        }
        else
        {
            WriteSpanFormattableInternalRare(in value, ref writer);
        }
    }

    private void WriteSpanFormattableInternalRare(in T value, ref LoggingPayloadWriter writer)
    {
        int size = 1024;
        while (true)
        {
            char[] buffer = ArrayPool<char>.Shared.Rent(size);
            try
            {
                Span<char> destination = buffer;
                if (!_TryFormatAction(in value, destination, out int charsWritten, _Format, CultureInfo.InvariantCulture))
                {
                    size *= 2;
                    continue;
                }
                writer.WriteValue(destination[..charsWritten]);
                break;
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer);
            }
        }
    }
}
#endif
