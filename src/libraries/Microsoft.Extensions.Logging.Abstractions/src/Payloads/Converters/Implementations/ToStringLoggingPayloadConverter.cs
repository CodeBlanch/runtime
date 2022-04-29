// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;

namespace Microsoft.Extensions.Logging.Payloads;

internal sealed class ToStringLoggingPayloadConverter<T> : LoggingPayloadConverter<T>
{
    public override void Write(in T value, ref LoggingPayloadWriter writer)
    {
        Debug.Assert(value != null);

        writer.WriteValue(value.ToString()!);
    }
}
