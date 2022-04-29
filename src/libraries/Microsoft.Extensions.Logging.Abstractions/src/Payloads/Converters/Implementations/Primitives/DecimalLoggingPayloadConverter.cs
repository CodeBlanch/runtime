// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Extensions.Logging.Payloads;

internal sealed class DecimalLoggingPayloadConverter : LoggingPayloadConverter<decimal>
{
    public override void Write(in decimal value, ref LoggingPayloadWriter writer)
        => writer.WriteValue(value);
}
