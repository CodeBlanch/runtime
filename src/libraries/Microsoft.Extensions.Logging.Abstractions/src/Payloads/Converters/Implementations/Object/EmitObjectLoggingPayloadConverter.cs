// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NETFRAMEWORK || NETCOREAPP
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;

namespace Microsoft.Extensions.Logging.Payloads;

internal sealed class EmitObjectLoggingPayloadConverter<
    [DynamicallyAccessedMembers(
        DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties
        | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)]
TObject> : ObjectLoggingPayloadConverter<TObject>
{
    protected override LoggingPayloadFieldWriter BuildPropertyWriter(
        Type rootType,
        MethodInfo? getMethod = null,
        FieldInfo? fieldInfo = null)
    {
        Debug.Assert((getMethod != null && fieldInfo == null) || (getMethod == null && fieldInfo != null));

        DynamicMethod dynamicMethod = new(
            $"{rootType.FullName}_fieldWriter_{getMethod?.Name ?? fieldInfo?.Name}",
            returnType: null,
            parameterTypes: new[]
            {
                rootType.MakeByRefType(),
                typeof(LoggingPayloadWriter).MakeByRefType(),
                typeof(ObjectLoggingPayloadConverterHelper.WriteFieldOptions),
                typeof(LoggingPayloadSerializerOptions)
            },
            typeof(EmitObjectLoggingPayloadConverter<TObject>).Module,
            skipVisibility: true);

        dynamicMethod.InitLocals = false;

        Type targetDataType = (getMethod?.ReturnType ?? fieldInfo?.FieldType)!;
        Type targetElementType = targetDataType.IsByRef ? targetDataType.GetElementType()! : targetDataType;

        ILGenerator generator = dynamicMethod.GetILGenerator();

        if (getMethod != null && !targetDataType.IsByRef)
        {
            generator.DeclareLocal(targetDataType);
        }

        generator.Emit(OpCodes.Ldarg_0);
        if (!rootType.IsValueType)
        {
            generator.Emit(OpCodes.Ldind_Ref);
        }
        if (getMethod != null)
        {
            if (getMethod.IsVirtual && !getMethod.IsFinal)
            {
                generator.Emit(OpCodes.Callvirt, getMethod);
            }
            else
            {
                generator.Emit(OpCodes.Call, getMethod);
            }
            if (!targetDataType.IsByRef)
            {
                generator.Emit(OpCodes.Stloc_0);
                generator.Emit(OpCodes.Ldloca_S, 0);
            }
        }
        else
        {
            generator.Emit(OpCodes.Ldflda, fieldInfo!);
        }
        generator.Emit(OpCodes.Ldarg_1);
        generator.Emit(OpCodes.Ldarg_2);
        generator.Emit(OpCodes.Ldarg_3);

        EmitCall(generator, targetElementType);

        generator.Emit(OpCodes.Ret);

        return (LoggingPayloadFieldWriter)dynamicMethod.CreateDelegate(typeof(LoggingPayloadFieldWriter));
    }

    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2060", Justification = "TODO")]
    private static void EmitCall(ILGenerator generator, Type targetElementType)
    {
        if (targetElementType.IsValueType)
        {
            Type? underlyingType = Nullable.GetUnderlyingType(targetElementType);
            if (underlyingType != null)
            {
                generator.Emit(OpCodes.Call, ObjectLoggingPayloadConverterHelper.WriteFieldNullableGenericMethodInfo.MakeGenericMethod(underlyingType));
            }
            else
            {
                generator.Emit(OpCodes.Call, ObjectLoggingPayloadConverterHelper.WriteFieldStructGenericMethodInfo.MakeGenericMethod(targetElementType));
            }
        }
        else
        {
            generator.Emit(OpCodes.Call, ObjectLoggingPayloadConverterHelper.WriteFieldObjectGenericMethodInfo.MakeGenericMethod(targetElementType));
        }
    }
}
#endif
