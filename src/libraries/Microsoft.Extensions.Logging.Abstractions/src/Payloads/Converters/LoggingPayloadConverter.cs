// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Extensions.Logging.Payloads;

public abstract class LoggingPayloadConverter
{
    internal LoggingPayloadConverter()
    {
    }

    internal abstract void WritePolymorphic(object value, ref LoggingPayloadWriter writer);
}
