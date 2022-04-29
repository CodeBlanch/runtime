// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;

namespace Microsoft.Extensions.Logging.Payloads;

#pragma warning disable CA1812

internal sealed class NullableLoggingPayloadConverter<T> : LoggingPayloadConverter<T?>
    where T : struct
{
    private readonly LoggingPayloadConverter<T> _InnerConverter;

    public NullableLoggingPayloadConverter(LoggingPayloadConverter<T> innerConverter)
        : base(allowNullable: true)
    {
        Debug.Assert(innerConverter != null);

        _InnerConverter = innerConverter;
    }

    public override void Write(in T? value, ref LoggingPayloadWriter writer)
    {
        if (!value.HasValue)
            throw new InvalidOperationException();

#if NET7_0_OR_GREATER
        ref readonly T innerValue = ref Nullable.GetValueRefOrDefaultRef(in value);
#else
        T innerValue = value.Value;
#endif

        _InnerConverter.Write(in innerValue, ref writer);
    }
}
