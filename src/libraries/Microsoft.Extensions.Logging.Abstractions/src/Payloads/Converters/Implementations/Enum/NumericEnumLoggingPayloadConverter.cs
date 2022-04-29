// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Microsoft.Extensions.Logging.Payloads;

internal sealed class NumericEnumLoggingPayloadConverter<T> : LoggingPayloadConverter<T>
    where T : struct, Enum
{
    private static readonly TypeCode s_EnumTypeCode = Type.GetTypeCode(typeof(T));

    public override void Write(in T value, ref LoggingPayloadWriter writer)
        => EnumLoggingPayloadConverterHelper.WriteNumeric(s_EnumTypeCode, in value, ref writer);
}
