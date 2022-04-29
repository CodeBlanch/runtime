// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
#if !NETCOREAPP3_1_OR_GREATER
using System.Buffers;
#endif
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Extensions.Logging.Payloads;

[DebuggerDisplay("{ToStringNoCache()}")]
public sealed class LoggingPayloadStringWriteTarget : LoggingPayloadWriteTarget
{
    private readonly StringBuilder _Builder;
    private string? _ToStringResult;

    public LoggingPayloadStringWriteTarget(int initialCapacity = 1024)
    {
        _Builder = new(initialCapacity);
    }

    public override void Reset()
    {
        _Builder.Clear();
        _ToStringResult = null;

        base.Reset();
    }

    public void CopyTo(TextWriter textWriter)
    {
        if (textWriter == null)
            throw new ArgumentNullException(nameof(textWriter));

        textWriter.Write(_Builder);
    }

    public void CopyTo(StringBuilder stringBuilder)
    {
        if (stringBuilder == null)
            throw new ArgumentNullException(nameof(stringBuilder));

        stringBuilder.Append(_Builder);
    }

    public override string ToString()
        => _ToStringResult ??= _Builder.ToString();

    private string ToStringNoCache()
        => _Builder.ToString();

    public override void OnBeginObject() => _Builder.Append("{ ");

    public override void OnEndObject() => _Builder.Append(" }");

    public override void OnBeginArray() => _Builder.Append("[ ");

    public override void OnEndArray() => _Builder.Append(" ]");

    public override void OnWriteSeparator() => _Builder.Append(", ");

    public override void OnBeginProperty(string propertyName)
    {
        _Builder.Append(propertyName);
        _Builder.Append(" = ");
    }

    public override void OnWriteNullValue() => _Builder.Append("[null]");

    public override void OnWriteValue(char value) => _Builder.Append(value);

#if NET6_0_OR_GREATER
    public override void OnWriteValue(int value) => _Builder.Append(CultureInfo.InvariantCulture, $"{value:G}");

    [CLSCompliant(false)]
    public override void OnWriteValue(uint value) => _Builder.Append(CultureInfo.InvariantCulture, $"{value:G}");

    public override void OnWriteValue(long value) => _Builder.Append(CultureInfo.InvariantCulture, $"{value:G}");

    [CLSCompliant(false)]
    public override void OnWriteValue(ulong value) => _Builder.Append(CultureInfo.InvariantCulture, $"{value:G}");

    public override void OnWriteValue(short value) => _Builder.Append(CultureInfo.InvariantCulture, $"{value:G}");

    [CLSCompliant(false)]
    public override void OnWriteValue(ushort value) => _Builder.Append(CultureInfo.InvariantCulture, $"{value:G}");

    public override void OnWriteValue(byte value) => _Builder.Append(CultureInfo.InvariantCulture, $"{value:G}");

    [CLSCompliant(false)]
    public override void OnWriteValue(sbyte value) => _Builder.Append(CultureInfo.InvariantCulture, $"{value:G}");

    public override void OnWriteValue(double value) => _Builder.Append(CultureInfo.InvariantCulture, $"{value:G}");

    public override void OnWriteValue(float value) => _Builder.Append(CultureInfo.InvariantCulture, $"{value:G}");

    public override void OnWriteValue(decimal value) => _Builder.Append(CultureInfo.InvariantCulture, $"{value:G}");
#else
    public override void OnWriteValue(int value) => _Builder.Append(value.ToString("{0:G}", CultureInfo.InvariantCulture));

    [CLSCompliant(false)]
    public override void OnWriteValue(uint value) => _Builder.Append(value.ToString("{0:G}", CultureInfo.InvariantCulture));

    public override void OnWriteValue(long value) => _Builder.Append(value.ToString("{0:G}", CultureInfo.InvariantCulture));

    [CLSCompliant(false)]
    public override void OnWriteValue(ulong value) => _Builder.Append(value.ToString("{0:G}", CultureInfo.InvariantCulture));

    public override void OnWriteValue(short value) => _Builder.Append(value.ToString("{0:G}", CultureInfo.InvariantCulture));

    [CLSCompliant(false)]
    public override void OnWriteValue(ushort value) => _Builder.Append(value.ToString("{0:G}", CultureInfo.InvariantCulture));

    public override void OnWriteValue(byte value) => _Builder.Append(value.ToString("{0:G}", CultureInfo.InvariantCulture));

    [CLSCompliant(false)]
    public override void OnWriteValue(sbyte value) => _Builder.Append(value.ToString("{0:G}", CultureInfo.InvariantCulture));

    public override void OnWriteValue(double value) => _Builder.Append(value.ToString("{0:G}", CultureInfo.InvariantCulture));

    public override void OnWriteValue(float value) => _Builder.Append(value.ToString("{0:G}", CultureInfo.InvariantCulture));

    public override void OnWriteValue(decimal value) => _Builder.Append(value.ToString("{0:G}", CultureInfo.InvariantCulture));
#endif

    public override void OnWriteValue(string value) => _Builder.Append(value);

    public override unsafe void OnWriteValue(ReadOnlySpan<char> value)
#if NETCOREAPP3_1_OR_GREATER
        => _Builder.Append(value);
#else
    {
        fixed (char* ptr = value)
        {
            _Builder.Append(ptr, value.Length);
        }
    }
#endif

    public override void OnWriteValue(bool value)
        => _Builder.Append(value ? "true" : "false");

    public override void OnWriteValue(byte[] value)
    {
#if NETCOREAPP3_1_OR_GREATER
        if (value.Length < 64)
        {
            OnWriteValueInternal(value.AsSpan());
        }
        else
        {
            _Builder.Append(Convert.ToBase64String(value));
        }
#else
        _Builder.Append(Convert.ToBase64String(value));
#endif
    }

    public override void OnWriteValue(ReadOnlySpan<byte> value)
    {
#if NETCOREAPP3_1_OR_GREATER
        if (value.Length < 64)
        {
            OnWriteValueInternal(value);
        }
        else
        {
            _Builder.Append(Convert.ToBase64String(value));
        }
#else
        int binaryLength = value.Length;

        byte[] binaryData = ArrayPool<byte>.Shared.Rent(binaryLength);
        char[] charData = ArrayPool<char>.Shared.Rent(binaryLength * 2);
        try
        {
            value.CopyTo(binaryData);

            int charsWritten = Convert.ToBase64CharArray(binaryData, 0, binaryLength, charData, 0);

            _Builder.Append(charData, 0, charsWritten);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(binaryData);
            ArrayPool<char>.Shared.Return(charData);
        }
#endif
    }

#if NETCOREAPP3_1_OR_GREATER
    private void OnWriteValueInternal(ReadOnlySpan<byte> value)
    {
        Span<char> data = stackalloc char[128];
        bool result = Convert.TryToBase64Chars(value, data, out int charsWritten);
        Debug.Assert(result);
        _Builder.Append(data[..charsWritten]);
    }
#endif

}
