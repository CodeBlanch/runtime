// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;

namespace Microsoft.Extensions.Logging.Payloads;

public static class LoggingPayloadSerializer
{
    public static void Serialize<TPayload>(
        LoggingPayloadWriteTarget target,
        in TPayload? payload,
        LoggingPayloadSerializerOptions options)
        => Serialize(target, in payload, converter: null, options);

    public static void Serialize<TPayload>(
        LoggingPayloadWriteTarget target,
        in TPayload? payload,
        LoggingPayloadConverter<TPayload> converter)
        => Serialize(target, in payload, converter, options: null);

    public static void Serialize<TPayload>(
        LoggingPayloadWriteTarget target,
        in TPayload? payload,
        LoggingPayloadConverter<TPayload>? converter = null,
        LoggingPayloadSerializerOptions? options = null)
    {
        if (target is null)
            throw new ArgumentNullException(nameof(target));

        LoggingPayloadWriter writer = new(target, options ?? LoggingPayloadSerializerOptions.Default);
        try
        {
            if (TypeInspector<TPayload>.Inspector.IsNull(in payload!))
            {
                writer.WriteNullValue();
            }
            else if (converter is not null)
            {
                converter.Write(in payload!, ref writer);

                if (!writer.State.HasValue)
                    throw new InvalidOperationException();
            }
            else
            {
                SerializeInternal(in payload, ref writer);
            }
        }
        catch (LoggingPayloadSerializationException lpEx)
        {
            lpEx.TryUpdateExceptionMessage(ref writer);
            throw;
        }
        catch (Exception ex)
        {
            LoggingPayloadSerializationException lpEx = new(message: null, ex);
            lpEx.TryUpdateExceptionMessage(ref writer);
            throw lpEx;
        }
        finally
        {
            writer.Flush();
        }
    }

    internal static void SerializeInternal<TPayload>(in TPayload value, ref LoggingPayloadWriter writer)
    {
        Debug.Assert(value != null);

        bool isValueType = typeof(TPayload).IsValueType;

        if (!isValueType
            && writer.State.HasVisited(value!))
        {
            writer.BeginValueInternal();
            writer.WriteNullValueInternal();
            return;
        }

        Type concreteType = value.GetType();

        LoggingPayloadConverter loggingPayloadConverter = writer.Options.GetConverter(
            concreteType,
            isValueType ? typeof(TPayload) : null);

        if (loggingPayloadConverter is LoggingPayloadConverter<TPayload> typedLoggingPayloadConverter)
        {
            typedLoggingPayloadConverter.Write(in value, ref writer);
        }
        else
        {
            loggingPayloadConverter.WritePolymorphic(value, ref writer);
        }

        if (!writer.State.HasValue)
            throw new InvalidOperationException();
    }
}
