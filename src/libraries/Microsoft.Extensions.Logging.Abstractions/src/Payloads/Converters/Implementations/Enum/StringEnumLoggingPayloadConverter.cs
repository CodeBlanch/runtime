// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Microsoft.Extensions.Logging.Payloads;

internal sealed class StringEnumLoggingPayloadConverter<T> : LoggingPayloadConverter<T>
    where T : struct, Enum
{
    private const int NameCacheSizeSoftLimit = 64;

    private static readonly TypeCode s_EnumTypeCode = Type.GetTypeCode(typeof(T));

    // Odd type codes are conveniently signed types (for enum backing types).
    private static readonly string? s_NegativeSign = ((int)s_EnumTypeCode % 2) == 0 ? null : NumberFormatInfo.CurrentInfo.NegativeSign;

    private readonly bool? _WriteUnknownEnumValuesAsNumeric;
    private Dictionary<ulong, string> _NameCache;

    public StringEnumLoggingPayloadConverter(bool? writeUnknownEnumValuesAsNumeric = null)
    {
        _WriteUnknownEnumValuesAsNumeric = writeUnknownEnumValuesAsNumeric;

#if NET6_0_OR_GREATER
        string[] names = Enum.GetNames<T>();
        T[] values = Enum.GetValues<T>();
#else
        string[] names = Enum.GetNames(typeof(T));
        Array values = Enum.GetValues(typeof(T));
#endif

        Debug.Assert(names.Length == values.Length);

        _NameCache = new(Math.Min(names.Length, NameCacheSizeSoftLimit));
        for (int i = 0; i < names.Length; i++)
        {
            if (_NameCache.Count >= NameCacheSizeSoftLimit)
            {
                break;
            }

#if NET6_0_OR_GREATER
            T value = values[i];
#else
            T value = (T)values.GetValue(i)!;
#endif

            ulong key = ConvertToUInt64(value);
            string name = names[i];

#if NETCOREAPP3_1_OR_GREATER
            _NameCache.TryAdd(key, name);
#else
            if (!_NameCache.ContainsKey(key))
                _NameCache.Add(key, name);
#endif
        }
    }

    public override void Write(in T value, ref LoggingPayloadWriter writer)
    {
        ulong key = ConvertToUInt64(value);

        if (_NameCache.TryGetValue(key, out string? name))
        {
            writer.WriteValue(name);
            return;
        }

        name = value.ToString();
        if (IsValidIdentifier(name))
        {
            if (_NameCache.Count < NameCacheSizeSoftLimit)
            {
                Dictionary<ulong, string> cacheCopy = new(_NameCache);
                cacheCopy[key] = name;
                Interlocked.CompareExchange(ref _NameCache, cacheCopy, _NameCache);
            }

            writer.WriteValue(name);
            return;
        }

        if (!(_WriteUnknownEnumValuesAsNumeric ?? writer.Options.EnumOptions.WriteUnknownEnumValuesAsNumeric))
            throw new InvalidOperationException();

        EnumLoggingPayloadConverterHelper.WriteNumeric(s_EnumTypeCode, in value, ref writer);
    }

    private static bool IsValidIdentifier(string value)
    {
        // Trying to do this check efficiently. When an enum is converted to
        // string the underlying value is given if it can't find a matching
        // identifier (or identifiers in the case of flags).
        //
        // The underlying value will be given back with a digit (e.g. 0-9) possibly
        // preceded by a negative sign. Identifiers have to start with a letter
        // so we'll just pick the first valid one and check for a negative sign
        // if needed.
        return value[0] >= 'A' &&
            (s_NegativeSign == null || !value.StartsWith(s_NegativeSign, StringComparison.Ordinal));
    }

    private static ulong ConvertToUInt64(object value)
    {
        Debug.Assert(value is T);
        return s_EnumTypeCode switch
        {
            TypeCode.Int32 => (ulong)(int)value,
            TypeCode.UInt32 => (uint)value,
            TypeCode.UInt64 => (ulong)value,
            TypeCode.Int64 => (ulong)(long)value,
            TypeCode.SByte => (ulong)(sbyte)value,
            TypeCode.Byte => (byte)value,
            TypeCode.Int16 => (ulong)(short)value,
            TypeCode.UInt16 => (ushort)value,
            _ => throw new InvalidOperationException(),
        };
    }
}
