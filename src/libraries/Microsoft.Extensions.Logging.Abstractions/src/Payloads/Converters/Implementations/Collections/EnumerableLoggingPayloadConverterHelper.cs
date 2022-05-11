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

internal static class EnumerableLoggingPayloadConverterHelper
{
    public static void WriteArray<T>(T[] value, ref LoggingPayloadWriter writer, LoggingPayloadConverter<T>? converter = null)
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

    public static void WriteDictionary<TDictionary, TValue>(in TDictionary value, ref LoggingPayloadWriter writer, LoggingPayloadConverter<TValue>? converter = null)
        where TDictionary : IEnumerable<KeyValuePair<string, TValue>>
    {
        Debug.Assert(value != null);

        LoggingPayloadWriterState state = writer.State;

        uint maxCount = (uint)writer.Options.EffectiveMaxPropertyCount;

        TypeInspector<TValue> typeInspector = TypeInspector<TValue>.Inspector;

        writer.BeginObject();

        /* Note: Some unavoidable copying of value types going on in here.
        Something like this would help:
        https://github.com/dotnet/runtime/issues/58333#issuecomment-907877948 */

        if (value is Dictionary<string, TValue> dictionary)
        {
            // Note: This is to utilize the struct enumerator on concrete Dictionary.
            foreach (KeyValuePair<string, TValue> item in dictionary)
            {
                if (string.IsNullOrEmpty(item.Key))
                    continue;

                if (state.ChildItemCount >= maxCount)
                    break;

                TValue itemValue = item.Value;

                if (writer.HandledAsNullOrIgnoredInternal(typeInspector, in itemValue, item.Key))
                    continue;

                writer.BeginPropertyInternal(item.Key);
                if (converter != null)
                {
                    converter.Write(in itemValue, ref writer);
                }
                else
                {
                    LoggingPayloadSerializer.SerializeInternal(in itemValue, ref writer);
                }
                writer.EndPropertyInternal();
            }
        }
        else
        {
            foreach (KeyValuePair<string, TValue> item in value)
            {
                if (string.IsNullOrEmpty(item.Key))
                    continue;

                if (state.ChildItemCount >= maxCount)
                    break;

                TValue itemValue = item.Value;

                if (writer.HandledAsNullOrIgnoredInternal(typeInspector, in itemValue, item.Key))
                    continue;

                writer.BeginPropertyInternal(item.Key);
                if (converter != null)
                {
                    converter.Write(in itemValue, ref writer);
                }
                else
                {
                    LoggingPayloadSerializer.SerializeInternal(in itemValue, ref writer);
                }
                writer.EndPropertyInternal();
            }
        }

        writer.EndObject();
    }

    public static void WriteEnumerable<TEnumerable, TItem>(in TEnumerable value, ref LoggingPayloadWriter writer, LoggingPayloadConverter<TItem>? converter = null)
        where TEnumerable : IEnumerable<TItem>
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
                else if (converter != null)
                {
                    converter.Write(in item, ref writer);
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
                else if (converter != null)
                {
                    converter.Write(in item, ref writer);
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
                else if (converter != null)
                {
                    converter.Write(in item, ref writer);
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
                else if (converter != null)
                {
                    converter.Write(in item, ref writer);
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
