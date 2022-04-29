// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Microsoft.Extensions.Logging.Payloads;

internal abstract class TypeInspector<T>
{
    public static TypeInspector<T> Inspector { get; } = BuildTypeInspector();

    public abstract bool IsNull(in T value);

    private static TypeInspector<T> BuildTypeInspector()
    {
        Type type = typeof(T);

        if (type.IsValueType)
        {
            Type? underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType != null
                ? (TypeInspector<T>)Activator.CreateInstance(typeof(NullableTypeInspector<>).MakeGenericType(underlyingType))!
                : (TypeInspector<T>)Activator.CreateInstance(typeof(ValueTypeInspector<>).MakeGenericType(type))!;
        }

        return (TypeInspector<T>)Activator.CreateInstance(typeof(ObjectTypeInspector<>).MakeGenericType(type))!;
    }
}
