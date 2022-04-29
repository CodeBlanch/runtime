// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;

namespace Microsoft.Extensions.Logging.Payloads;

public class LoggingPayloadMaskOptions
{
    private static readonly string?[] s_MaskCache = new string?[64];

    private int _MaskLength = -1;
    private int _UnmaskedStartingCharacterCount;
    private int _UnmaskedEndingCharacterCount;

    public char MaskCharacter { get; set; } = '*';

    public int MaskLength
    {
        get => _MaskLength;
        set
        {
            if (value < -1)
                throw new ArgumentOutOfRangeException(nameof(value));
            _MaskLength = value;
        }
    }

    public int UnmaskedStartingCharacterCount
    {
        get => _UnmaskedStartingCharacterCount;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));
            _UnmaskedStartingCharacterCount = value;
        }
    }

    public int UnmaskedEndingCharacterCount
    {
        get => _UnmaskedEndingCharacterCount;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));
            _UnmaskedEndingCharacterCount = value;
        }
    }

    internal int UnmaskedCharacterCount => _UnmaskedStartingCharacterCount + _UnmaskedEndingCharacterCount;

    internal int ComputeMaskLength(int sourceLength)
    {
        return _MaskLength >= 0
            ? _MaskLength
            : Math.Max(0, sourceLength - _UnmaskedStartingCharacterCount - _UnmaskedEndingCharacterCount);
    }

    internal void MaskValueIntoSpan(
        int maskLength,
        ReadOnlySpan<char> source,
        Span<char> destination,
        out int charactersWritten)
    {
        charactersWritten = 0;

        Debug.Assert(destination.Length >= maskLength + UnmaskedCharacterCount);

        string? mask;
        if (MaskCharacter == '*' && maskLength < 64)
        {
            ref string? cachedMask = ref s_MaskCache[maskLength];
            if (cachedMask is null)
                cachedMask = new string('*', maskLength);
            mask = cachedMask;
        }
        else
        {
            mask = maskLength == 0 ? string.Empty : new string(MaskCharacter, maskLength);
        }

        if (_UnmaskedStartingCharacterCount > 0)
        {
            if (source.Length > _UnmaskedStartingCharacterCount)
            {
                source.Slice(0, _UnmaskedStartingCharacterCount).CopyTo(destination);
                charactersWritten = _UnmaskedStartingCharacterCount;
            }
            else
            {
                source.CopyTo(destination);
                charactersWritten = source.Length;
            }
        }

        if (maskLength > 0)
        {
            mask.AsSpan().CopyTo(destination.Slice(charactersWritten));
            charactersWritten += mask.Length;
        }

        if (_UnmaskedEndingCharacterCount > 0 && source.Length > _UnmaskedStartingCharacterCount)
        {
            int startPosition = source.Length - _UnmaskedEndingCharacterCount;
            if (startPosition < _UnmaskedStartingCharacterCount)
                startPosition = _UnmaskedStartingCharacterCount;

            source.Slice(startPosition).CopyTo(destination.Slice(charactersWritten));
            charactersWritten += source.Length - startPosition;
        }
    }
}
