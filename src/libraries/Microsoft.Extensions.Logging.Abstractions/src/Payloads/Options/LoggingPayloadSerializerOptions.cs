// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Microsoft.Extensions.Logging.Payloads;

public class LoggingPayloadSerializerOptions
{
    public static LoggingPayloadSerializerOptions Default { get; } = new(isDefaultInstance: true)
    {
        DefaultWriteTargetFactory = () => new LoggingPayloadStringWriteTarget()
    };

    private const int DefaultMaxDepth = 64;
    private const int DefaultMaxArrayLength = 64;
    private const int DefaultMaxPropertyCount = 64;

    private readonly ConcurrentDictionary<Type, LoggingPayloadConverter> _RegisteredLoggingPayloadConverters;

    private Func<LoggingPayloadWriteTarget>? _DefaultWriteTargetFactory;
    private int _MaxDepth;
    private int _MaxArrayLength;
    private int _MaxPropertyCount;
    private LoggingPayloadEnumOptions _EnumOptions = new();

    public LoggingPayloadSerializerOptions()
        : this(isDefaultInstance: false)
    {
    }

    private LoggingPayloadSerializerOptions(bool isDefaultInstance)
    {
        IsDefaultInstance = isDefaultInstance;

        if (isDefaultInstance)
        {
            _RegisteredLoggingPayloadConverters = new()
            {
                [typeof(string)] = new StringLoggingPayloadConverter(),
                [typeof(int)] = new Int32LoggingPayloadConverter(),
                [typeof(uint)] = new UInt32LoggingPayloadConverter(),
                [typeof(long)] = new Int64LoggingPayloadConverter(),
                [typeof(ulong)] = new UInt64LoggingPayloadConverter(),
                [typeof(short)] = new Int16LoggingPayloadConverter(),
                [typeof(ushort)] = new UInt16LoggingPayloadConverter(),
                [typeof(byte)] = new ByteLoggingPayloadConverter(),
                [typeof(sbyte)] = new SByteLoggingPayloadConverter(),
                [typeof(char)] = new CharLoggingPayloadConverter(),
                [typeof(float)] = new SingleLoggingPayloadConverter(),
                [typeof(double)] = new DoubleLoggingPayloadConverter(),
                [typeof(decimal)] = new DecimalLoggingPayloadConverter(),
                [typeof(byte[])] = new ByteArrayLoggingPayloadConverter(),
                [typeof(bool)] = new BooleanLoggingPayloadConverter(),

#if NET6_0_OR_GREATER
                [typeof(DateOnly)] = LoggingPayloadMetadataServices.DateOnlyConverter,
#endif
                [typeof(DateTime)] = LoggingPayloadMetadataServices.DateTimeConverter,
                [typeof(DateTimeOffset)] = LoggingPayloadMetadataServices.DateTimeOffsetConverter,
                [typeof(Guid)] = LoggingPayloadMetadataServices.GuidConverter,
#if NET6_0_OR_GREATER
                [typeof(TimeOnly)] = LoggingPayloadMetadataServices.TimeOnlyConverter,
#endif
                [typeof(TimeSpan)] = LoggingPayloadMetadataServices.TimeSpanConverter,
                [typeof(Version)] = LoggingPayloadMetadataServices.VersionConverter,
                [typeof(Uri)] = LoggingPayloadMetadataServices.UriConverter,
            };
        }
        else
        {
            _RegisteredLoggingPayloadConverters = new();
        }
    }

    public bool IsDefaultInstance { get; }

    public Func<LoggingPayloadWriteTarget>? DefaultWriteTargetFactory
    {
        get => _DefaultWriteTargetFactory;
        set
        {
            if (value == null && IsDefaultInstance)
                throw new ArgumentNullException(nameof(value));
            _DefaultWriteTargetFactory = value;
        }
    }

    public IDictionary<Type, LoggingPayloadConverter> Converters => _RegisteredLoggingPayloadConverters;

    internal uint DefaultIgnoreState { get; private set; } = 1u;

    public LoggingPayloadIgnoreCondition DefaultIgnoreCondition
    {
        get => LoggingPayloadWriter.IgnoreStateToCondition(DefaultIgnoreState);
        set
        {
            if (value == LoggingPayloadIgnoreCondition.Always)
                throw new ArgumentException("TODO", nameof(value));

            DefaultIgnoreState = LoggingPayloadWriter.IgnoreConditionToState(value);
        }
    }

    public int MaxDepth
    {
        get => _MaxDepth;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            _MaxDepth = value;
            EffectiveMaxDepth = value == 0 ? DefaultMaxDepth : value;
        }
    }

    internal int EffectiveMaxDepth { get; private set; } = DefaultMaxDepth;

    public int MaxArrayLength
    {
        get => _MaxArrayLength;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            _MaxArrayLength = value;
            EffectiveMaxArrayLength = value == 0 ? DefaultMaxArrayLength : value;
        }
    }

    internal int EffectiveMaxArrayLength { get; private set; } = DefaultMaxArrayLength;

    public int MaxPropertyCount
    {
        get => _MaxPropertyCount;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            _MaxPropertyCount = value;
            EffectiveMaxPropertyCount = value == 0 ? DefaultMaxPropertyCount : value;
        }
    }

    internal int EffectiveMaxPropertyCount { get; private set; } = DefaultMaxPropertyCount;

    public bool IncludePublicFields { get; set; }

    public LoggingPayloadEnumOptions EnumOptions
    {
        get => _EnumOptions;
        set => _EnumOptions = value ?? throw new ArgumentNullException(nameof(value));
    }

    internal LoggingPayloadConverter GetConverter(
        [DynamicallyAccessedMembers(
            DynamicallyAccessedMemberTypes.Interfaces
            | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties
            | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)]
        Type concreteType,
        Type? runtimeValueType)
    {
        if (!_RegisteredLoggingPayloadConverters.TryGetValue(runtimeValueType ?? concreteType, out LoggingPayloadConverter? loggingPayloadConverter))
        {
            loggingPayloadConverter = BuildAndRegisterLoggingPayloadConverter(concreteType, runtimeValueType);
        }

        return loggingPayloadConverter;
    }

    private LoggingPayloadConverter BuildAndRegisterLoggingPayloadConverter(
        [DynamicallyAccessedMembers(
            DynamicallyAccessedMemberTypes.Interfaces
            | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties
            | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)]
        Type concreteType,
        Type? runtimeValueType)
    {
        if (!IsDefaultInstance)
        {
            return Default.GetConverter(concreteType, runtimeValueType);
        }

        if (runtimeValueType != null && runtimeValueType != concreteType)
        {
            /* Note: Nullable<T> gets special handling. Converter is
            registered for the concrete type and a wrapper
            NullableLoggingPayloadConverter<T> is registered automatically.
            */

            Type? underlyingType = Nullable.GetUnderlyingType(runtimeValueType);

            Debug.Assert(underlyingType != null);

            if (!_RegisteredLoggingPayloadConverters.TryGetValue(concreteType, out LoggingPayloadConverter? loggingPayloadConverter))
            {
                // If a converter for the concrete type is not found, create it.
                loggingPayloadConverter = CreateConverter(concreteType);
            }

            LoggingPayloadConverter? nullableLoggingPayloadConverter = (LoggingPayloadConverter?)Activator.CreateInstance(
                typeof(NullableLoggingPayloadConverter<>).MakeGenericType(concreteType),
                new object[] { loggingPayloadConverter });

            Debug.Assert(nullableLoggingPayloadConverter != null);

            _RegisteredLoggingPayloadConverters.TryAdd(runtimeValueType, nullableLoggingPayloadConverter);

            return nullableLoggingPayloadConverter;
        }

        return CreateConverter(concreteType);
    }

    private LoggingPayloadConverter CreateConverter(
        [DynamicallyAccessedMembers(
            DynamicallyAccessedMemberTypes.Interfaces
            | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties
            | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)]
        Type concreteType)
    {
        LoggingPayloadConverterAttribute? converterAttribute = concreteType.GetCustomAttribute<LoggingPayloadConverterAttribute>();

        LoggingPayloadConverter loggingPayloadConverter = converterAttribute != null
            ? converterAttribute.GetConverterInstance()
            : LoggingPayloadConverterFactory.Create(concreteType);

        Debug.Assert(loggingPayloadConverter != null);

        bool result = _RegisteredLoggingPayloadConverters.TryAdd(concreteType, loggingPayloadConverter);

        Debug.Assert(result);

        return loggingPayloadConverter;
    }
}
