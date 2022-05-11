// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace Microsoft.Extensions.Logging.Payloads;

internal sealed class EnumerableLoggingPayloadConverter<TEnumerable, TItem> : LoggingPayloadConverter<TEnumerable>
    where TEnumerable : IEnumerable<TItem>
{
    public override void Write(in TEnumerable value, ref LoggingPayloadWriter writer)
    {
        EnumerableLoggingPayloadConverterHelper.WriteEnumerable<TEnumerable, TItem>(in value, ref writer);
    }
}
