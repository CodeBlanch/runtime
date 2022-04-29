// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Extensions.Logging.Payloads;

internal sealed class UInt16LoggingPayloadConverter : LoggingPayloadConverter<ushort>
{
    public override void Write(in ushort value, ref LoggingPayloadWriter writer)
        => writer.WriteValue(value);
}
