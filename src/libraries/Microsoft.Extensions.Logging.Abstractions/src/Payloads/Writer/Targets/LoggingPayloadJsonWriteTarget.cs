// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Extensions.Logging.Payloads;

[DebuggerDisplay("{ToStringNoCache()}")]
public sealed class LoggingPayloadJsonWriteTarget : LoggingPayloadWriteTarget
{
    private readonly StringBuilder _Builder;
    private string? _ToStringResult;

    public LoggingPayloadJsonWriteTarget(int initialCapacity = 1024)
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

    public override void OnBeginObject() => _Builder.Append('{');

    public override void OnEndObject() => _Builder.Append('}');

    public override void OnBeginArray() => _Builder.Append('[');

    public override void OnEndArray() => _Builder.Append(']');

    public override void OnWriteSeparator() => _Builder.Append(',');

    public override void OnBeginProperty(string propertyName)
    {
        _Builder.Append('"');
        _Builder.Append(propertyName);
        _Builder.Append("\":");
    }

    public override void OnWriteNullValue() => _Builder.Append("null");

    public override void OnWriteValue(char value)
    {
        _Builder.Append('"');
        _Builder.Append(value);
        _Builder.Append('"');
    }

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

    public override unsafe void OnWriteValue(string value)
    {
        fixed (char* ptr = value)
        {
            WriteStringWithEscaping(ptr, value.Length);
        }
    }

    public override unsafe void OnWriteValue(ReadOnlySpan<char> value)
    {
        fixed (char* ptr = value)
        {
            WriteStringWithEscaping(ptr, value.Length);
        }
    }

    private unsafe void WriteStringWithEscaping(char* ptr, int length)
    {
        _Builder.Append('"');

        int startWriteIndex = 0;
        int index = 0;

        while (index < length)
        {
            char c = ptr[index++];
            if (!char.IsSurrogate(c))
            {
                char escapedChar = default;
                switch (c)
                {
                    case '"':
                        escapedChar = '"';
                        break;
                    case '\\':
                        escapedChar = '\\';
                        break;
                    case '/':
                        escapedChar = '/';
                        break;
                    case '\b':
                        escapedChar = 'b';
                        break;
                    case '\f':
                        escapedChar = 'f';
                        break;
                    case '\n':
                        escapedChar = 'n';
                        break;
                    case '\r':
                        escapedChar = 'r';
                        break;
                    case '\t':
                        escapedChar = 't';
                        break;
                }
                if (escapedChar != default)
                {
                    WriteEscapedCharacter(ptr, ref startWriteIndex, ref index, escapedChar);
                }
                continue;
            }
            else if (index < length)
            {
                char sp = ptr[index++];
                if (char.IsSurrogatePair(c, sp))
                {
                    continue;
                }
            }

            throw new FormatException();
        }

        _Builder.Append(ptr + startWriteIndex, length - startWriteIndex);

        _Builder.Append('"');
    }

    private unsafe void WriteEscapedCharacter(char* ptr, ref int startWriteIndex, ref int index, char character)
    {
        int indexMinusOne = index - 1;
        if (indexMinusOne > startWriteIndex)
        {
            _Builder.Append(ptr + startWriteIndex, indexMinusOne - startWriteIndex);
        }
        startWriteIndex = index;
        _Builder.Append('\\');
        _Builder.Append(character);
    }

    public override void OnWriteValue(bool value)
        => _Builder.Append(value ? "true" : "false");

    public override void OnWriteValue(byte[] value)
    {
        _Builder.Append('"');
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
        _Builder.Append('"');
    }

    public override void OnWriteValue(ReadOnlySpan<byte> value)
    {
        _Builder.Append('"');
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
        _Builder.Append(Convert.ToBase64String(value.ToArray()));
#endif
        _Builder.Append('"');
    }

#if NETCOREAPP3_1_OR_GREATER
    private void OnWriteValueInternal(ReadOnlySpan<byte> value)
    {
        Span<char> data = stackalloc char[128];
        bool result = Convert.TryToBase64Chars(value, data, out int charsWritten);
        Debug.Assert(result);
        _Builder.Append(data.Slice(0, charsWritten));
    }
#endif
}
