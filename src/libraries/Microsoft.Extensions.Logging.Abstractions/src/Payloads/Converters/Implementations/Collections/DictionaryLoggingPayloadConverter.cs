// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Extensions.Logging.Payloads;

internal sealed class DictionaryLoggingPayloadConverter<TDictionary, TValue> : LoggingPayloadConverter<TDictionary>
    where TDictionary : IEnumerable<KeyValuePair<string, TValue>>
{
    public override void Write(in TDictionary value, ref LoggingPayloadWriter writer)
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
                LoggingPayloadSerializer.SerializeInternal(in itemValue, ref writer);
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
                LoggingPayloadSerializer.SerializeInternal(in itemValue, ref writer);
                writer.EndPropertyInternal();
            }
        }

        writer.EndObject();
    }
}
