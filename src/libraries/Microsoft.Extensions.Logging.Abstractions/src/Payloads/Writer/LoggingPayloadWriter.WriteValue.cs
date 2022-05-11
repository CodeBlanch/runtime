// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.Logging.Payloads;

public ref partial struct LoggingPayloadWriter
{
    public void WriteNullValue()
    {
        BeginValue();
        WriteNullValueInternal();
    }

    public void WriteValue(int value)
    {
        BeginValue();
        WriteValueInternal(value);
    }

    [CLSCompliant(false)]
    public void WriteValue(uint value)
    {
        BeginValue();
        WriteValueInternal(value);
    }

    public void WriteValue(long value)
    {
        BeginValue();
        WriteValueInternal(value);
    }

    [CLSCompliant(false)]
    public void WriteValue(ulong value)
    {
        BeginValue();
        WriteValueInternal(value);
    }

    public void WriteValue(short value)
    {
        BeginValue();
        WriteValueInternal(value);
    }

    [CLSCompliant(false)]
    public void WriteValue(ushort value)
    {
        BeginValue();
        WriteValueInternal(value);
    }

    public void WriteValue(byte value)
    {
        BeginValue();
        WriteValueInternal(value);
    }

    [CLSCompliant(false)]
    public void WriteValue(sbyte value)
    {
        BeginValue();
        WriteValueInternal(value);
    }

    public void WriteValue(char value)
    {
        BeginValue();
        WriteValueInternal(value);
    }

    public void WriteValue(double value)
    {
        BeginValue();
        WriteValueInternal(value);
    }

    public void WriteValue(float value)
    {
        BeginValue();
        WriteValueInternal(value);
    }

    public void WriteValue(decimal value)
    {
        BeginValue();
        WriteValueInternal(value);
    }

    public void WriteValue(byte[] value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        BeginValue();
        WriteValueInternal(value);
    }

    public void WriteValue(bool value)
    {
        BeginValue();
        WriteValueInternal(value);
    }

    public void WriteValue(
        string value,
        LoggingPayloadMaskOptions? maskOptions = null)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        BeginValue();
        if (maskOptions != null)
        {
            WriteValueInternal(value.AsSpan(), maskOptions);
        }
        else
        {
            WriteValueInternal(value);
        }
    }

    public void WriteValue(
        ReadOnlySpan<char> value,
        LoggingPayloadMaskOptions? maskOptions = null)
    {
        BeginValue();
        if (maskOptions != null)
        {
            WriteValueInternal(value, maskOptions);
        }
        else
        {
            WriteValueInternal(value);
        }
    }

    public void WriteValue(ReadOnlySpan<byte> value)
    {
        BeginValue();
        WriteValueInternal(value);
    }

    public void WriteValue<T>(
        IEnumerable<T> value,
        LoggingPayloadConverter<T>? converter = null)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        LoggingPayloadWriter writer = new(_Target, Options);

        EnumerableLoggingPayloadConverterHelper.WriteEnumerable(in value, ref writer, converter);
    }

    public void WriteValueObject<T>(
        T value,
        LoggingPayloadConverter<T>? converter = null)
        where T : class
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        LoggingPayloadWriter writer = new(_Target, Options);

        writer.BeginValue();

        if (converter != null)
        {
            converter.Write(in value, ref writer);
        }
        else
        {
            LoggingPayloadSerializer.SerializeInternal(in value, ref writer);
        }
    }

    public void WriteValueStruct<T>(
        in T value,
        LoggingPayloadConverter<T>? converter = null)
        where T : struct
    {
        LoggingPayloadWriter writer = new(_Target, Options);

        writer.BeginValue();

        if (converter != null)
        {
            converter.Write(in value, ref writer);
        }
        else
        {
            LoggingPayloadSerializer.SerializeInternal(in value, ref writer);
        }
    }
}
