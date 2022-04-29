// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.Logging.Payloads;

public ref partial struct LoggingPayloadWriter
{
    public void WriteNullProperty(string propertyName)
    {
        BeginProperty(propertyName);
        BeginValueInternal();
        WriteNullValueInternal();
        EndPropertyInternal();
    }

    public void WriteProperty(
        string propertyName,
        int value)
    {
        if (_IgnoreState >= 2u && value == default)
            return;

        BeginProperty(propertyName);
        BeginValueInternal();
        WriteValueInternal(value);
        EndPropertyInternal();
    }

    public void WriteProperty(
        string propertyName,
        int? value)
    {
        if (!value.HasValue)
        {
            HandleNullProperty(propertyName);
        }
        else
        {
            BeginProperty(propertyName);
            BeginValueInternal();
            WriteValueInternal(value.Value);
            EndPropertyInternal();
        }
    }

    [CLSCompliant(false)]
    public void WriteProperty(
        string propertyName,
        uint value)
    {
        if (_IgnoreState >= 2u && value == default)
            return;

        BeginProperty(propertyName);
        BeginValueInternal();
        WriteValueInternal(value);
        EndPropertyInternal();
    }

    [CLSCompliant(false)]
    public void WriteProperty(
        string propertyName,
        uint? value)
    {
        if (!value.HasValue)
        {
            HandleNullProperty(propertyName);
        }
        else
        {
            BeginProperty(propertyName);
            BeginValueInternal();
            WriteValueInternal(value.Value);
            EndPropertyInternal();
        }
    }

    public void WriteProperty(
        string propertyName,
        long value)
    {
        if (_IgnoreState >= 2u && value == default)
            return;

        BeginProperty(propertyName);
        BeginValueInternal();
        WriteValueInternal(value);
        EndPropertyInternal();
    }

    public void WriteProperty(
        string propertyName,
        long? value)
    {
        if (!value.HasValue)
        {
            HandleNullProperty(propertyName);
        }
        else
        {
            BeginProperty(propertyName);
            BeginValueInternal();
            WriteValueInternal(value.Value);
            EndPropertyInternal();
        }
    }

    [CLSCompliant(false)]
    public void WriteProperty(
        string propertyName,
        ulong value)
    {
        if (_IgnoreState >= 2u && value == default)
            return;

        BeginProperty(propertyName);
        BeginValueInternal();
        WriteValueInternal(value);
        EndPropertyInternal();
    }

    [CLSCompliant(false)]
    public void WriteProperty(
        string propertyName,
        ulong? value)
    {
        if (!value.HasValue)
        {
            HandleNullProperty(propertyName);
        }
        else
        {
            BeginProperty(propertyName);
            BeginValueInternal();
            WriteValueInternal(value.Value);
            EndPropertyInternal();
        }
    }

    public void WriteProperty(
        string propertyName,
        short value)
    {
        if (_IgnoreState >= 2u && value == default)
            return;

        BeginProperty(propertyName);
        BeginValueInternal();
        WriteValueInternal(value);
        EndPropertyInternal();
    }

    public void WriteProperty(
        string propertyName,
        short? value)
    {
        if (!value.HasValue)
        {
            HandleNullProperty(propertyName);
        }
        else
        {
            BeginProperty(propertyName);
            BeginValueInternal();
            WriteValueInternal(value.Value);
            EndPropertyInternal();
        }
    }

    [CLSCompliant(false)]
    public void WriteProperty(
        string propertyName,
        ushort value)
    {
        if (_IgnoreState >= 2u && value == default)
            return;

        BeginProperty(propertyName);
        BeginValueInternal();
        WriteValueInternal(value);
        EndPropertyInternal();
    }

    [CLSCompliant(false)]
    public void WriteProperty(
        string propertyName,
        ushort? value)
    {
        if (!value.HasValue)
        {
            HandleNullProperty(propertyName);
        }
        else
        {
            BeginProperty(propertyName);
            BeginValueInternal();
            WriteValueInternal(value.Value);
            EndPropertyInternal();
        }
    }

    public void WriteProperty(
        string propertyName,
        byte value)
    {
        if (_IgnoreState >= 2u && value == default)
            return;

        BeginProperty(propertyName);
        BeginValueInternal();
        WriteValueInternal(value);
        EndPropertyInternal();
    }

    public void WriteProperty(
        string propertyName,
        byte? value)
    {
        if (!value.HasValue)
        {
            HandleNullProperty(propertyName);
        }
        else
        {
            BeginProperty(propertyName);
            BeginValueInternal();
            WriteValueInternal(value.Value);
            EndPropertyInternal();
        }
    }

    [CLSCompliant(false)]
    public void WriteProperty(
        string propertyName,
        sbyte value)
    {
        if (_IgnoreState >= 2u && value == default)
            return;

        BeginProperty(propertyName);
        BeginValueInternal();
        WriteValueInternal(value);
        EndPropertyInternal();
    }

    [CLSCompliant(false)]
    public void WriteProperty(
        string propertyName,
        sbyte? value)
    {
        if (!value.HasValue)
        {
            HandleNullProperty(propertyName);
        }
        else
        {
            BeginProperty(propertyName);
            BeginValueInternal();
            WriteValueInternal(value.Value);
            EndPropertyInternal();
        }
    }

    public void WriteProperty(
        string propertyName,
        char value)
    {
        if (_IgnoreState >= 2u && value == default)
            return;

        BeginProperty(propertyName);
        BeginValueInternal();
        WriteValueInternal(value);
        EndPropertyInternal();
    }

    public void WriteProperty(
        string propertyName,
        char? value)
    {
        if (!value.HasValue)
        {
            HandleNullProperty(propertyName);
        }
        else
        {
            BeginProperty(propertyName);
            BeginValueInternal();
            WriteValueInternal(value.Value);
            EndPropertyInternal();
        }
    }

    public void WriteProperty(
        string propertyName,
        double value)
    {
        if (_IgnoreState >= 2u && value == default)
            return;

        BeginProperty(propertyName);
        BeginValueInternal();
        WriteValueInternal(value);
        EndPropertyInternal();
    }

    public void WriteProperty(
        string propertyName,
        double? value)
    {
        if (!value.HasValue)
        {
            HandleNullProperty(propertyName);
        }
        else
        {
            BeginProperty(propertyName);
            BeginValueInternal();
            WriteValueInternal(value.Value);
            EndPropertyInternal();
        }
    }

    public void WriteProperty(
        string propertyName,
        float value)
    {
        if (_IgnoreState >= 2u && value == default)
            return;

        BeginProperty(propertyName);
        BeginValueInternal();
        WriteValueInternal(value);
        EndPropertyInternal();
    }

    public void WriteProperty(
        string propertyName,
        float? value)
    {
        if (!value.HasValue)
        {
            HandleNullProperty(propertyName);
        }
        else
        {
            BeginProperty(propertyName);
            BeginValueInternal();
            WriteValueInternal(value.Value);
            EndPropertyInternal();
        }
    }

    public void WriteProperty(
        string propertyName,
        decimal value)
    {
        if (_IgnoreState >= 2u && value == default)
            return;

        BeginProperty(propertyName);
        BeginValueInternal();
        WriteValueInternal(value);
        EndPropertyInternal();
    }

    public void WriteProperty(
        string propertyName,
        decimal? value)
    {
        if (!value.HasValue)
        {
            HandleNullProperty(propertyName);
        }
        else
        {
            BeginProperty(propertyName);
            BeginValueInternal();
            WriteValueInternal(value.Value);
            EndPropertyInternal();
        }
    }

    public void WriteProperty(
        string propertyName,
        byte[]? value)
    {
        if (value is null)
        {
            HandleNullProperty(propertyName);
            return;
        }

        BeginProperty(propertyName);
        BeginValueInternal();
        WriteValueInternal(value);
        EndPropertyInternal();
    }

    public void WriteProperty(
        string propertyName,
        bool value)
    {
        if (_IgnoreState >= 2u && value == default)
            return;

        BeginProperty(propertyName);
        BeginValueInternal();
        WriteValueInternal(value);
        EndPropertyInternal();
    }

    public void WriteProperty(
        string propertyName,
        bool? value)
    {
        if (!value.HasValue)
        {
            HandleNullProperty(propertyName);
        }
        else
        {
            BeginProperty(propertyName);
            BeginValueInternal();
            WriteValueInternal(value.Value);
            EndPropertyInternal();
        }
    }

    public void WriteProperty(
        string propertyName,
        string? value,
        LoggingPayloadMaskOptions? maskOptions = null)
    {
        if (value is null)
        {
            HandleNullProperty(propertyName);
            return;
        }

        BeginProperty(propertyName);
        BeginValueInternal();

        if (maskOptions != null)
        {
            WriteValueInternal(value.AsSpan(), maskOptions);
        }
        else
        {
            WriteValueInternal(value);
        }

        EndPropertyInternal();
    }

    public void WriteProperty(
        string propertyName,
        ReadOnlySpan<char> value,
        LoggingPayloadMaskOptions? maskOptions = null)
    {
        BeginProperty(propertyName);
        BeginValueInternal();
        if (maskOptions != null)
        {
            WriteValueInternal(value, maskOptions);
        }
        else
        {
            WriteValueInternal(value);
        }
        EndPropertyInternal();
    }

    public void WriteProperty(
        string propertyName,
        ReadOnlySpan<byte> value)
    {
        BeginProperty(propertyName);
        BeginValueInternal();
        WriteValueInternal(value);
        EndPropertyInternal();
    }

    public void WriteProperty<T>(
        string propertyName,
        IEnumerable<T>? value)
    {
        if (value is null)
        {
            HandleNullProperty(propertyName);
        }
        else
        {
            LoggingPayloadWriter writer = new(_Target, Options);

            writer.BeginProperty(propertyName);
            LoggingPayloadSerializer.SerializeInternal(in value, ref writer);
            writer.EndPropertyInternal();
        }
    }

    public void WritePropertyObject<T>(
        string propertyName,
        T? value,
        LoggingPayloadConverter<T>? converter = null)
        where T : class
    {
        if (value is null)
        {
            HandleNullProperty(propertyName);
        }
        else
        {
            LoggingPayloadWriter writer = new(_Target, Options);

            writer.BeginProperty(propertyName);

            if (converter != null)
            {
                converter.Write(in value, ref writer);
            }
            else
            {
                LoggingPayloadSerializer.SerializeInternal(in value, ref writer);
            }

            writer.EndPropertyInternal();
        }
    }

    public void WritePropertyStruct<T>(
        string propertyName,
        in T value,
        LoggingPayloadConverter<T>? converter = null)
        where T : struct
    {
        if (_IgnoreState == 2u
            && EqualityComparer<T>.Default.Equals(default, value))
        {
            return;
        }

        LoggingPayloadWriter writer = new(_Target, Options);

        writer.BeginProperty(propertyName);

        if (converter != null)
        {
            converter.Write(in value, ref writer);
        }
        else
        {
            LoggingPayloadSerializer.SerializeInternal(in value, ref writer);
        }

        writer.EndPropertyInternal();
    }

    public void WritePropertyNullable<T>(
        string propertyName,
        in T? value,
        LoggingPayloadConverter<T>? converter = null)
        where T : struct
    {
        if (value is null)
        {
            HandleNullProperty(propertyName);
        }
        else
        {
            LoggingPayloadWriter writer = new(_Target, Options);

            writer.BeginProperty(propertyName);

#if NET7_0_OR_GREATER
            ref readonly T innerValue = ref Nullable.GetValueRefOrDefaultRef(in value);
#else
            T innerValue = value.Value;
#endif

            if (converter != null)
            {
                converter.Write(in innerValue, ref writer);
            }
            else
            {
                LoggingPayloadSerializer.SerializeInternal(in innerValue, ref writer);
            }

            writer.EndPropertyInternal();
        }
    }
}
