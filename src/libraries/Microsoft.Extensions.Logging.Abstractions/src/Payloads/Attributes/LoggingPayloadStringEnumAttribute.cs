// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Microsoft.Extensions.Logging.Payloads;

[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class LoggingPayloadStringEnumAttribute : Attribute
{
    private readonly LoggingPayloadEnumOptions _Options = new()
    {
        WriteEnumValuesAsStrings = true
    };

    public bool WriteUnknownEnumValuesAsNumeric
    {
        get => _Options.WriteUnknownEnumValuesAsNumeric;
        set => _Options.WriteUnknownEnumValuesAsNumeric = value;
    }
}
