// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace Microsoft.Extensions.Logging.Payloads;

internal sealed class DictionaryLoggingPayloadConverter<TDictionary, TValue> : LoggingPayloadConverter<TDictionary>
    where TDictionary : IEnumerable<KeyValuePair<string, TValue>>
{
    public override void Write(in TDictionary value, ref LoggingPayloadWriter writer)
    {
        EnumerableLoggingPayloadConverterHelper.WriteDictionary<TDictionary, TValue>(in value, ref writer);
    }
}
