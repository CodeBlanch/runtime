// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Microsoft.Extensions.Logging.Payloads;

#pragma warning disable CA1812

internal sealed class EnumLoggingPayloadConverter<T> : LoggingPayloadConverter<T>
    where T : struct, Enum
{
    private StringEnumLoggingPayloadConverter<T>? _StringConverter;
    private NumericEnumLoggingPayloadConverter<T>? _NumericConverter;

    public override void Write(in T value, ref LoggingPayloadWriter writer)
    {
        if (writer.Options.EnumOptions.WriteEnumValuesAsStrings)
        {
            StringEnumLoggingPayloadConverter<T> stringConverter = _StringConverter ??= new StringEnumLoggingPayloadConverter<T>();

            stringConverter.Write(in value, ref writer);
        }
        else
        {
            NumericEnumLoggingPayloadConverter<T> numericConverter = _NumericConverter ??= new NumericEnumLoggingPayloadConverter<T>();

            numericConverter.Write(in value, ref writer);
        }
    }
}
