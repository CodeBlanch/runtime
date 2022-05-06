// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Microsoft.Extensions.Logging.Payloads;

internal abstract class ObjectLoggingPayloadConverter<
    [DynamicallyAccessedMembers(
        DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties
        | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)]
TObject> : LoggingPayloadConverter<TObject>
{
    public delegate void LoggingPayloadFieldWriter(
        in TObject value,
        ref LoggingPayloadWriter writer,
        ObjectLoggingPayloadConverterHelper.WriteFieldOptions writeFieldOptions,
        LoggingPayloadSerializerOptions serializerOptions);

    private readonly List<Tuple<ObjectLoggingPayloadConverterHelper.WriteFieldOptions, LoggingPayloadFieldWriter>> _FieldWriters = new();

    protected ObjectLoggingPayloadConverter()
    {
        Type type = typeof(TObject);

        foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (!property.CanRead)
                continue;

            LoggingPayloadIncludeAttribute? includeAttribute = property.GetCustomAttribute<LoggingPayloadIncludeAttribute>();
            if (!property.GetMethod!.IsPublic && includeAttribute == null)
                continue;

            ObjectLoggingPayloadConverterHelper.WriteFieldOptions? writeFieldOptions = BuildWriteFieldOptions(
                ObjectLoggingPayloadConverterHelper.FieldIncludeCondition.Always,
                property.PropertyType,
                property);
            if (writeFieldOptions == null)
                continue;

            _FieldWriters.Add(new(writeFieldOptions, BuildPropertyWriter(type, getMethod: property.GetMethod)));
        }

        foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            LoggingPayloadIncludeAttribute? includeAttribute = field.GetCustomAttribute<LoggingPayloadIncludeAttribute>();
            if (!field.IsPublic && includeAttribute == null)
                continue;

            ObjectLoggingPayloadConverterHelper.FieldIncludeCondition includeCondition =
                includeAttribute != null
                ? ObjectLoggingPayloadConverterHelper.FieldIncludeCondition.Always
                : ObjectLoggingPayloadConverterHelper.FieldIncludeCondition.IfPublicFieldsIncluded;

            ObjectLoggingPayloadConverterHelper.WriteFieldOptions? writeFieldOptions = BuildWriteFieldOptions(
                includeCondition,
                field.FieldType,
                field);
            if (writeFieldOptions == null)
                continue;

            _FieldWriters.Add(new(writeFieldOptions, BuildPropertyWriter(type, fieldInfo: field)));
        }
    }

    private static ObjectLoggingPayloadConverterHelper.WriteFieldOptions? BuildWriteFieldOptions(
        ObjectLoggingPayloadConverterHelper.FieldIncludeCondition includeCondition,
        Type fieldType,
        MemberInfo memberInfo)
    {
        LoggingPayloadIgnoreCondition? ignoreCondition = memberInfo.GetCustomAttribute<LoggingPayloadIgnoreAttribute>()?.Condition;
        if (ignoreCondition == LoggingPayloadIgnoreCondition.Always)
            return null;

        LoggingPayloadPropertyNameAttribute? propertyNameAttribute = memberInfo.GetCustomAttribute<LoggingPayloadPropertyNameAttribute>();

        LoggingPayloadConverter? converter = memberInfo.GetCustomAttribute<LoggingPayloadConverterAttribute>()?.GetConverterInstance();

        if (converter == null && fieldType.IsValueType)
        {
            fieldType = Nullable.GetUnderlyingType(fieldType) ?? fieldType;
            if (fieldType.IsEnum)
            {
                LoggingPayloadStringEnumAttribute? stringEnumAttribute = memberInfo.GetCustomAttribute<LoggingPayloadStringEnumAttribute>();
                if (stringEnumAttribute != null)
                {
                    converter = (LoggingPayloadConverter)Activator.CreateInstance(
                        typeof(StringEnumLoggingPayloadConverter<>).MakeGenericType(fieldType),
                        new object[] { stringEnumAttribute.WriteUnknownEnumValuesAsNumeric })!;
                }
                else
                {
                    LoggingPayloadNumericEnumAttribute? numericEnumAttribute = memberInfo.GetCustomAttribute<LoggingPayloadNumericEnumAttribute>();
                    if (numericEnumAttribute != null)
                    {
                        converter = (LoggingPayloadConverter)Activator.CreateInstance(
                            typeof(NumericEnumLoggingPayloadConverter<>).MakeGenericType(fieldType))!;
                    }
                }
            }
        }

        return new(
            includeCondition,
            propertyNameAttribute?.PropertyName ?? memberInfo.Name,
            converter,
            ignoreCondition.HasValue ? LoggingPayloadWriter.IgnoreConditionToState(ignoreCondition.Value) : null,
            fieldType != typeof(string) ? null : memberInfo.GetCustomAttribute<LoggingPayloadMaskAttribute>()?.Options);
    }

    public override void Write(in TObject value, ref LoggingPayloadWriter writer)
    {
        Debug.Assert(value != null);

        if (writer.CurrentDepth >= writer.Options.EffectiveMaxDepth)
            return;

        writer.BeginObject(typeof(TObject).Name);
        foreach ((ObjectLoggingPayloadConverterHelper.WriteFieldOptions writeFieldOptions, LoggingPayloadFieldWriter? fieldWriter) in _FieldWriters)
        {
            if (writeFieldOptions.IncludeCondition == ObjectLoggingPayloadConverterHelper.FieldIncludeCondition.Always
                || writer.Options.IncludePublicFields)
            {
                fieldWriter(
                    in value,
                    ref writer,
                    writeFieldOptions,
                    writer.Options);
            }
        }
        writer.EndObject();
    }

    protected abstract LoggingPayloadFieldWriter BuildPropertyWriter(
        Type rootType,
        MethodInfo? getMethod = null,
        FieldInfo? fieldInfo = null);
}
