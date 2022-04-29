// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;

namespace Microsoft.Extensions.Logging.Payloads;

internal sealed class ArrayLoggingPayloadConverter<T> : LoggingPayloadConverter<T[]>
{
    public override void Write(in T[] value, ref LoggingPayloadWriter writer)
    {
        Debug.Assert(value != null);

        LoggingPayloadWriterState state = writer.State;

        uint maxLength = (uint)writer.Options.EffectiveMaxArrayLength;

        TypeInspector<T> typeInspector = TypeInspector<T>.Inspector;

        writer.BeginArray(typeof(T).Name);

        for (int i = 0; i < value.Length; i++)
        {
            if (state.ChildItemCount >= maxLength)
                break;

            ref readonly T item = ref value[i];

            if (typeInspector.IsNull(in item!))
            {
                writer.BeginValueInternal();
                writer.WriteNullValueInternal();
            }
            else
            {
                LoggingPayloadSerializer.SerializeInternal(in item, ref writer);
            }
        }

        writer.EndArray();
    }
}
