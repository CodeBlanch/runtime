// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
#if NETCOREAPP
using System.Runtime.CompilerServices;
#endif

namespace Microsoft.Extensions.Logging.Payloads;

internal static class LoggingPayloadConverterFactory
{
    public static LoggingPayloadConverter Create(
        [DynamicallyAccessedMembers(
            DynamicallyAccessedMemberTypes.Interfaces
            | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties
            | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)]
        Type concreteType)
    {
        if (concreteType.IsArray)
        {
            return (LoggingPayloadConverter)Activator.CreateInstance(
                typeof(ArrayLoggingPayloadConverter<>).MakeGenericType(concreteType.GetElementType()!))!;
        }

        Type[] typeInterfaces = concreteType.GetInterfaces();
        Type? ienumerableTypeDefinition = null;

        foreach (Type typeInterface in typeInterfaces)
        {
            if (typeInterface.IsGenericType
                && typeInterface.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                ienumerableTypeDefinition = typeInterface;
                break;
            }
        }

        if (ienumerableTypeDefinition != null)
        {
            Type typeArgument = ienumerableTypeDefinition.GetGenericArguments()[0];
            if (typeArgument.IsGenericType && typeArgument.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                Type[] kvpTypeArguments = typeArgument.GetGenericArguments();
                if (kvpTypeArguments[0] == typeof(string))
                {
                    return (LoggingPayloadConverter)Activator.CreateInstance(
                            typeof(DictionaryLoggingPayloadConverter<,>).MakeGenericType(concreteType, kvpTypeArguments[1]))!;
                }
            }

            return (LoggingPayloadConverter)Activator.CreateInstance(
                    typeof(EnumerableLoggingPayloadConverter<,>).MakeGenericType(concreteType, typeArgument))!;
        }

        if (concreteType.IsEnum)
        {
            LoggingPayloadStringEnumAttribute? stringEnumAttribute = concreteType.GetCustomAttribute<LoggingPayloadStringEnumAttribute>();
            if (stringEnumAttribute != null)
            {
                return (LoggingPayloadConverter)Activator.CreateInstance(
                    typeof(StringEnumLoggingPayloadConverter<>).MakeGenericType(concreteType),
                    new object[] { stringEnumAttribute.WriteUnknownEnumValuesAsNumeric })!;
            }

            LoggingPayloadNumericEnumAttribute? numericEnumAttribute = concreteType.GetCustomAttribute<LoggingPayloadNumericEnumAttribute>();
            return numericEnumAttribute != null
                ? (LoggingPayloadConverter)Activator.CreateInstance(
                    typeof(NumericEnumLoggingPayloadConverter<>).MakeGenericType(concreteType))!
                : (LoggingPayloadConverter)Activator.CreateInstance(
                    typeof(EnumLoggingPayloadConverter<>).MakeGenericType(concreteType))!;
        }

#if NETCOREAPP
        return (LoggingPayloadConverter)Activator.CreateInstance(RuntimeFeature.IsDynamicCodeSupported
            ? typeof(EmitObjectLoggingPayloadConverter<>).MakeGenericType(concreteType)
            : typeof(ReflectionObjectLoggingPayloadConverter<>).MakeGenericType(concreteType))!;
#elif NETFRAMEWORK
        return (LoggingPayloadConverter)Activator.CreateInstance(
            typeof(EmitObjectLoggingPayloadConverter<>).MakeGenericType(concreteType))!;
#else
        return (LoggingPayloadConverter)Activator.CreateInstance(
            typeof(ReflectionObjectLoggingPayloadConverter<>).MakeGenericType(concreteType))!;
#endif
    }
}
