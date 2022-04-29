// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Microsoft.Extensions.Logging.Payloads;

internal sealed class ReflectionObjectLoggingPayloadConverter<TObject> : LoggingPayloadConverter<TObject>
{
    public override void Write(in TObject value, ref LoggingPayloadWriter writer)
    {
        // TODO: Implement.
        throw new NotImplementedException();
    }
}
