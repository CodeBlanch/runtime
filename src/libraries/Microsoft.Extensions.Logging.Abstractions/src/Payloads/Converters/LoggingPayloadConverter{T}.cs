// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;

namespace Microsoft.Extensions.Logging.Payloads;

public abstract class LoggingPayloadConverter<T> : LoggingPayloadConverter
{
    protected LoggingPayloadConverter()
    {
        if (typeof(T).IsValueType)
        {
            Type? underlyingType = Nullable.GetUnderlyingType(typeof(T));
            if (underlyingType != null)
            {
                /* Note: We don't allow converts of Nullable<T> because that is
                handled automatically. */
                throw new NotSupportedException();
            }
        }
    }

    internal LoggingPayloadConverter(bool allowNullable)
    {
        /* Note: This ctor is so that the NullableLoggingPayloadConverter can
        bypass the Nullable<T> check in the standard ctor. */

        Debug.Assert(allowNullable);
    }

    public abstract void Write(in T value, ref LoggingPayloadWriter writer);

    internal override void WritePolymorphic(object value, ref LoggingPayloadWriter writer)
    {
#if DEBUG
        if (typeof(T).IsValueType)
        {
            Debug.Write("Boxed value type detected");
        }
#endif

        if (value is not T typedValue)
        {
            throw new InvalidCastException();
        }

        Write(in typedValue, ref writer);
    }
}
