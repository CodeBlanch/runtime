// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Microsoft.Extensions.Logging.Payloads;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public class LoggingPayloadConverterAttribute : Attribute
{
    private static readonly ConcurrentDictionary<Type, LoggingPayloadConverter> s_ConverterCache = new();

    public Type ConverterType { get; }

    public LoggingPayloadConverterAttribute(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
        Type converterType)
    {
        ConverterType = converterType ?? throw new ArgumentNullException(nameof(converterType));

        bool isValid = false;
        Type? inspectionType = converterType.BaseType;
        while (inspectionType != null)
        {
            if (inspectionType.IsGenericType && inspectionType.GetGenericTypeDefinition() == typeof(LoggingPayloadConverter<>))
            {
                Type typeBeingConverted = inspectionType.GetGenericArguments()[0];
                if (typeBeingConverted.IsValueType && Nullable.GetUnderlyingType(typeBeingConverted) != null)
                {
                    /* Note: We don't allow converts of Nullable<T> because that
                    is handled automatically. */
                    throw new NotSupportedException();
                }
                isValid = true;
                break;
            }
            inspectionType = inspectionType.BaseType;
        }
        if (!isValid)
            throw new InvalidOperationException();
        if (converterType.GetConstructor(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null,
            types: Array.Empty<Type>(),
            modifiers: null) == null)
        {
            throw new InvalidOperationException();
        }
    }

    internal LoggingPayloadConverter GetConverterInstance()
    {
        if (!s_ConverterCache.TryGetValue(ConverterType, out LoggingPayloadConverter? converter))
        {
            converter = (LoggingPayloadConverter)Activator.CreateInstance(ConverterType)!;
            s_ConverterCache.TryAdd(ConverterType, converter);
        }

        return converter;
    }
}
