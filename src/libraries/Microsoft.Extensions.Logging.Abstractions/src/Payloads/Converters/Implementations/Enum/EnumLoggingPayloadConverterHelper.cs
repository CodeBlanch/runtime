// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

using System.Runtime.CompilerServices;

namespace Microsoft.Extensions.Logging.Payloads;

internal static class EnumLoggingPayloadConverterHelper
{
    public static void WriteNumeric<T>(TypeCode enumTypeCode, in T value, ref LoggingPayloadWriter writer)
    {
        T valueCopy = value;
        switch (enumTypeCode)
        {
            case TypeCode.Int32:
                writer.WriteValue(Unsafe.As<T, int>(ref valueCopy));
                break;
            case TypeCode.UInt32:
                writer.WriteValue(Unsafe.As<T, uint>(ref valueCopy));
                break;
            case TypeCode.UInt64:
                writer.WriteValue(Unsafe.As<T, ulong>(ref valueCopy));
                break;
            case TypeCode.Int64:
                writer.WriteValue(Unsafe.As<T, long>(ref valueCopy));
                break;
            case TypeCode.Int16:
                writer.WriteValue(Unsafe.As<T, short>(ref valueCopy));
                break;
            case TypeCode.UInt16:
                writer.WriteValue(Unsafe.As<T, ushort>(ref valueCopy));
                break;
            case TypeCode.Byte:
                writer.WriteValue(Unsafe.As<T, byte>(ref valueCopy));
                break;
            case TypeCode.SByte:
                writer.WriteValue(Unsafe.As<T, sbyte>(ref valueCopy));
                break;
            default:
                throw new InvalidOperationException();
        }
    }
}
