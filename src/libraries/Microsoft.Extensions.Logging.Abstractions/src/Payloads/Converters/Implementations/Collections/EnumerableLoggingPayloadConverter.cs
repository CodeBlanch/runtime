// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET6_0_OR_GREATER
using System;
#endif
using System.Collections.Generic;
using System.Diagnostics;
#if NET6_0_OR_GREATER
using System.Runtime.InteropServices;
#endif

namespace Microsoft.Extensions.Logging.Payloads;

internal sealed class EnumerableLoggingPayloadConverter<TEnumerable, TItem> : LoggingPayloadConverter<TEnumerable>
    where TEnumerable : IEnumerable<TItem>
{
    public override void Write(in TEnumerable value, ref LoggingPayloadWriter writer)
    {
        Debug.Assert(value != null);

        LoggingPayloadWriterState state = writer.State;

        uint maxLength = (uint)writer.Options.EffectiveMaxArrayLength;

        TypeInspector<TItem> typeInspector = TypeInspector<TItem>.Inspector;

        writer.BeginArray(typeof(TItem).Name);

        if (value is TItem[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (state.ChildItemCount >= maxLength)
                    break;

                ref readonly TItem item = ref array[i];

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
        }
#if NET6_0_OR_GREATER
        else if (value is List<TItem> list)
        {
            Span<TItem> data = CollectionsMarshal.AsSpan(list);
            foreach (ref readonly TItem item in data)
            {
                if (state.ChildItemCount >= maxLength)
                    break;

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
        }
#endif
        else if (value is IReadOnlyList<TItem> readOnlyList)
        {
            for (int i = 0; i < readOnlyList.Count; i++)
            {
                if (state.ChildItemCount >= maxLength)
                    break;

                // Note: Currently unavoidable copy of value types.
                TItem item = readOnlyList[i];

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
        }
        else
        {
            // Note: Currently unavoidable copy of value types.
            foreach (TItem item in value)
            {
                if (state.ChildItemCount >= maxLength)
                    break;

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
        }

        writer.EndArray();
    }
}
