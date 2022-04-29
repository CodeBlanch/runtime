// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Extensions.Logging.Payloads;

internal sealed class ObjectTypeInspector<T> : TypeInspector<T?>
    where T : class
{
    public override bool IsNull(in T? value) => value is null;
}
