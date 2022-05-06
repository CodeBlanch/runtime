// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Microsoft.Extensions.Logging.Payloads;

internal sealed class ReflectionObjectLoggingPayloadConverter<
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

        return getMethod != null
        ? (
            in TObject value,
            ref LoggingPayloadWriter writer,
            ObjectLoggingPayloadConverterHelper.WriteFieldOptions writeFieldOptions,
            LoggingPayloadSerializerOptions serializerOptions) =>
            {
                object? fieldValue = getMethod.Invoke(value, null);

                ObjectLoggingPayloadConverterHelper.WriteFieldObject(
                    in fieldValue,
                    ref writer,
                    writeFieldOptions,
                    serializerOptions);
            }
        : (
            in TObject value,
            ref LoggingPayloadWriter writer,
            ObjectLoggingPayloadConverterHelper.WriteFieldOptions writeFieldOptions,
            LoggingPayloadSerializerOptions serializerOptions) =>
            {
                object? fieldValue = fieldInfo!.GetValue(value);

                ObjectLoggingPayloadConverterHelper.WriteFieldObject(
                    in fieldValue,
                    ref writer,
                    writeFieldOptions,
                    serializerOptions);
            };
    }
}
