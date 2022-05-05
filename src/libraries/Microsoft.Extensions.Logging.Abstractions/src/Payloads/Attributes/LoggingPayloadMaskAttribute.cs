// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Microsoft.Extensions.Logging.Payloads;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class LoggingPayloadMaskAttribute : Attribute
{
    internal LoggingPayloadMaskOptions Options { get; } = new();

    public char MaskCharacter
    {
        get => Options.MaskCharacter;
        set => Options.MaskCharacter = value;
    }

    public int MaskLength
    {
        get => Options.MaskLength;
        set => Options.MaskLength = value;
    }

    public int UnmaskedStartingCharacterCount
    {
        get => Options.UnmaskedStartingCharacterCount;
        set => Options.UnmaskedStartingCharacterCount = value;
    }

    public int UnmaskedEndingCharacterCount
    {
        get => Options.UnmaskedEndingCharacterCount;
        set => Options.UnmaskedEndingCharacterCount = value;
    }
}
