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
    private readonly bool _Indented;
    private string? _ToStringResult;
    private int _IndentLevel;

    public LoggingPayloadJsonWriteTarget(bool indented = false, int initialCapacity = 1024)
    {
        _Builder = new(initialCapacity);
        _Indented = indented;
    }

    protected override void OnReset()
    {
        _Builder.Clear();
        _ToStringResult = null;
        _IndentLevel = 0;
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

    public override void AppendBeginObject()
    {
        if (_Indented && !InProperty)
        {
            WritePadding();
        }
        _Builder.Append('{');
        if (_Indented)
        {
            _Builder.AppendLine();
            _IndentLevel++;
        }
    }

    public override void AppendEndObject()
    {
        if (_Indented)
        {
            _Builder.AppendLine();
            _IndentLevel--;
            WritePadding();
        }
        _Builder.Append('}');
    }

    public override void AppendBeginArray()
    {
        if (_Indented && !InProperty)
        {
            WritePadding();
        }
        _Builder.Append('[');
        if (_Indented)
        {
            _Builder.AppendLine();
            _IndentLevel++;
        }
    }

    public override void AppendEndArray()
    {
        if (_Indented)
        {
            _Builder.AppendLine();
            _IndentLevel--;
            WritePadding();
        }
        _Builder.Append(']');
    }

    public override void AppendBeginProperty(string propertyName)
    {
        if (_Indented)
        {
            WritePadding();
        }
        _Builder.Append('"');
        _Builder.Append(propertyName);
        _Builder.Append("\":");
        if (_Indented)
        {
            _Builder.Append(' ');
        }
    }

    public override void AppendSeparator()
    {
        _Builder.Append(',');
        if (_Indented)
        {
            _Builder.AppendLine();
        }
    }

    public override void AppendNullValue() => _Builder.Append("null");

    public override void AppendValue(char value)
    {
        _Builder.Append('"');
        _Builder.Append(value);
        _Builder.Append('"');
    }

#if NET6_0_OR_GREATER
    public override void AppendValue(int value) => _Builder.Append(CultureInfo.InvariantCulture, $"{value:G}");

    [CLSCompliant(false)]
    public override void AppendValue(uint value) => _Builder.Append(CultureInfo.InvariantCulture, $"{value:G}");

    public override void AppendValue(long value) => _Builder.Append(CultureInfo.InvariantCulture, $"{value:G}");

    [CLSCompliant(false)]
    public override void AppendValue(ulong value) => _Builder.Append(CultureInfo.InvariantCulture, $"{value:G}");

    public override void AppendValue(short value) => _Builder.Append(CultureInfo.InvariantCulture, $"{value:G}");

    [CLSCompliant(false)]
    public override void AppendValue(ushort value) => _Builder.Append(CultureInfo.InvariantCulture, $"{value:G}");

    public override void AppendValue(byte value) => _Builder.Append(CultureInfo.InvariantCulture, $"{value:G}");

    [CLSCompliant(false)]
    public override void AppendValue(sbyte value) => _Builder.Append(CultureInfo.InvariantCulture, $"{value:G}");

    public override void AppendValue(double value) => _Builder.Append(CultureInfo.InvariantCulture, $"{value:G}");

    public override void AppendValue(float value) => _Builder.Append(CultureInfo.InvariantCulture, $"{value:G}");

    public override void AppendValue(decimal value) => _Builder.Append(CultureInfo.InvariantCulture, $"{value:G}");
#else
    public override void AppendValue(int value) => _Builder.Append(value.ToString("{0:G}", CultureInfo.InvariantCulture));

    [CLSCompliant(false)]
    public override void AppendValue(uint value) => _Builder.Append(value.ToString("{0:G}", CultureInfo.InvariantCulture));

    public override void AppendValue(long value) => _Builder.Append(value.ToString("{0:G}", CultureInfo.InvariantCulture));

    [CLSCompliant(false)]
    public override void AppendValue(ulong value) => _Builder.Append(value.ToString("{0:G}", CultureInfo.InvariantCulture));

    public override void AppendValue(short value) => _Builder.Append(value.ToString("{0:G}", CultureInfo.InvariantCulture));

    [CLSCompliant(false)]
    public override void AppendValue(ushort value) => _Builder.Append(value.ToString("{0:G}", CultureInfo.InvariantCulture));

    public override void AppendValue(byte value) => _Builder.Append(value.ToString("{0:G}", CultureInfo.InvariantCulture));

    [CLSCompliant(false)]
    public override void AppendValue(sbyte value) => _Builder.Append(value.ToString("{0:G}", CultureInfo.InvariantCulture));

    public override void AppendValue(double value) => _Builder.Append(value.ToString("{0:G}", CultureInfo.InvariantCulture));

    public override void AppendValue(float value) => _Builder.Append(value.ToString("{0:G}", CultureInfo.InvariantCulture));

    public override void AppendValue(decimal value) => _Builder.Append(value.ToString("{0:G}", CultureInfo.InvariantCulture));
#endif

    public override unsafe void AppendValue(string value)
    {
        fixed (char* ptr = value)
        {
            WriteStringWithEscaping(ptr, value.Length);
        }
    }

    public override unsafe void AppendValue(ReadOnlySpan<char> value)
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

    public override void AppendValue(bool value)
        => _Builder.Append(value ? "true" : "false");

    public override void AppendValue(byte[] value)
    {
        _Builder.Append('"');
#if NETCOREAPP3_1_OR_GREATER
        if (value.Length < 64)
        {
            AppendValueInternal(value.AsSpan());
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

    public override void AppendValue(ReadOnlySpan<byte> value)
    {
        _Builder.Append('"');
#if NETCOREAPP3_1_OR_GREATER
        if (value.Length < 64)
        {
            AppendValueInternal(value);
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

    private void WritePadding()
    {
        for (int i = 0; i < _IndentLevel; i++)
        {
            _Builder.Append('\t');
        }
    }

#if NETCOREAPP3_1_OR_GREATER
    private void AppendValueInternal(ReadOnlySpan<byte> value)
    {
        Span<char> data = stackalloc char[128];
        bool result = Convert.TryToBase64Chars(value, data, out int charsWritten);
        Debug.Assert(result);
        _Builder.Append(data.Slice(0, charsWritten));
    }
#endif
}
