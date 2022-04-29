// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Microsoft.Extensions.Logging.Payloads;

internal static class ObjectLoggingPayloadConverterHelper
{
    public static MethodInfo WriteFieldObjectGenericMethodInfo { get; } = typeof(ObjectLoggingPayloadConverterHelper).GetMethod(
        nameof(WriteFieldObject),
        BindingFlags.Static | BindingFlags.Public)
        ?? throw new MissingMethodException($"{nameof(WriteFieldObject)} could not be found reflectively.");

    public static MethodInfo WriteFieldStructGenericMethodInfo { get; } = typeof(ObjectLoggingPayloadConverterHelper).GetMethod(
        nameof(WriteFieldStruct),
        BindingFlags.Static | BindingFlags.Public)
        ?? throw new MissingMethodException($"{nameof(WriteFieldStruct)} could not be found reflectively.");

    public static MethodInfo WriteFieldNullableGenericMethodInfo { get; } = typeof(ObjectLoggingPayloadConverterHelper).GetMethod(
        nameof(WriteFieldNullable),
        BindingFlags.Static | BindingFlags.Public)
        ?? throw new MissingMethodException($"{nameof(WriteFieldNullable)} could not be found reflectively.");

    public enum FieldIncludeCondition
    {
        Always,
        IfPublicFieldsIncluded
    }

    public sealed class WriteFieldOptions
    {
        public FieldIncludeCondition IncludeCondition { get; }

        public string PropertyName { get; }

        public LoggingPayloadConverter? Converter { get; }

        public uint? IgnoreStateByAttribute { get; }

        public LoggingPayloadMaskOptions? MaskOptions { get; }

        public WriteFieldOptions(
            FieldIncludeCondition includeCondition,
            string propertyName,
            LoggingPayloadConverter? converter,
            uint? ignoreStateAttribute,
            LoggingPayloadMaskOptions? maskOptions)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyName));

            IncludeCondition = includeCondition;
            PropertyName = propertyName;
            Converter = converter;
            IgnoreStateByAttribute = ignoreStateAttribute;
            MaskOptions = maskOptions;
        }
    }

    public static void WriteFieldObject<T>(
        in T? value,
        ref LoggingPayloadWriter writer,
        WriteFieldOptions writeFieldOptions,
        LoggingPayloadSerializerOptions serializerOptions)
        where T : class
    {
        uint ignoreState = writeFieldOptions.IgnoreStateByAttribute ?? serializerOptions.DefaultIgnoreState;

        if (value is null)
        {
            if (ignoreState < 1u)
            {
                writer.BeginProperty(writeFieldOptions.PropertyName);
                writer.BeginValueInternal();
                writer.WriteNullValueInternal();
                writer.EndPropertyInternal();
            }
            return;
        }

        writer.BeginPropertyInternal(writeFieldOptions.PropertyName);

        LoggingPayloadConverter? converter = writeFieldOptions.Converter;
        if (converter != null)
        {
            if (converter is LoggingPayloadConverter<T> typedConverter)
            {
                typedConverter.Write(in value, ref writer);
            }
            else
            {
                converter.WritePolymorphic(value!, ref writer);
            }
        }
        else if (value is string stringValue)
        {
            writer.BeginValueInternal();

            LoggingPayloadMaskOptions? maskOptions = writeFieldOptions.MaskOptions;
            if (maskOptions != null)
            {
                writer.WriteValueInternal(stringValue.AsSpan(), maskOptions);
            }
            else
            {
                writer.WriteValueInternal(stringValue);
            }
        }
        else
        {
            LoggingPayloadSerializer.SerializeInternal(in value, ref writer);
        }

        writer.EndPropertyInternal();
    }

    public static void WriteFieldStruct<T>(
        in T value,
        ref LoggingPayloadWriter writer,
        WriteFieldOptions writeFieldOptions,
        LoggingPayloadSerializerOptions serializerOptions)
        where T : struct
    {
        uint ignoreState = writeFieldOptions.IgnoreStateByAttribute ?? serializerOptions.DefaultIgnoreState;

        if (ignoreState >= 2u
            && EqualityComparer<T>.Default.Equals(default, value))
        {
            return;
        }

        writer.BeginPropertyInternal(writeFieldOptions.PropertyName);

        LoggingPayloadConverter? converter = writeFieldOptions.Converter;
        if (converter != null)
        {
            if (converter is LoggingPayloadConverter<T> typedConverter)
            {
                typedConverter.Write(in value, ref writer);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
        else
        {
            LoggingPayloadSerializer.SerializeInternal(in value, ref writer);
        }

        writer.EndPropertyInternal();
    }

    public static void WriteFieldNullable<T>(
        in T? value,
        ref LoggingPayloadWriter writer,
        WriteFieldOptions writeFieldOptions,
        LoggingPayloadSerializerOptions serializerOptions)
        where T : struct
    {
        uint ignoreState = writeFieldOptions.IgnoreStateByAttribute ?? serializerOptions.DefaultIgnoreState;

        if (!value.HasValue)
        {
            if (ignoreState < 1u)
            {
                writer.BeginProperty(writeFieldOptions.PropertyName);
                writer.BeginValueInternal();
                writer.WriteNullValueInternal();
                writer.EndPropertyInternal();
            }
            return;
        }

#if NET7_0_OR_GREATER
        ref readonly T innerValue = ref Nullable.GetValueRefOrDefaultRef(in value);
#else
        T innerValue = value.Value;
#endif

        writer.BeginPropertyInternal(writeFieldOptions.PropertyName);

        LoggingPayloadConverter? converter = writeFieldOptions.Converter;
        if (converter != null)
        {
            if (converter is LoggingPayloadConverter<T> typedConverter)
            {
                typedConverter.Write(in innerValue, ref writer);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
        else
        {
            LoggingPayloadSerializer.SerializeInternal(in innerValue, ref writer);
        }

        writer.EndPropertyInternal();
    }
}
