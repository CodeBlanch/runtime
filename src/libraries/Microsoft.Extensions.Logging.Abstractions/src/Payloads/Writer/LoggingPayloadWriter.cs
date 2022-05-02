// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
#if NETCOREAPP3_1_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Microsoft.Extensions.Logging.Payloads;

public readonly ref partial struct LoggingPayloadWriter
{
    internal static uint IgnoreConditionToState(LoggingPayloadIgnoreCondition ignoreCondition)
    {
        return ignoreCondition switch
        {
            LoggingPayloadIgnoreCondition.WhenWritingNullOrDefault => 2u,
            LoggingPayloadIgnoreCondition.WhenWritingNull => 1u,
            LoggingPayloadIgnoreCondition.Never => 0u,
            _ => throw new NotSupportedException()
        };
    }

    internal static LoggingPayloadIgnoreCondition IgnoreStateToCondition(uint ignoreState)
    {
        return ignoreState switch
        {
            2u => LoggingPayloadIgnoreCondition.WhenWritingNullOrDefault,
            1u => LoggingPayloadIgnoreCondition.WhenWritingNull,
            0u => LoggingPayloadIgnoreCondition.Never,
            _ => throw new NotSupportedException()
        };
    }

    private readonly LoggingPayloadWriteTarget _Target;
    private readonly uint _IgnoreState;

    internal LoggingPayloadWriterState State { get; }

    internal LoggingPayloadSerializerOptions Options { get; }

    private ScopeType CurrentScope => State.Scope;

    internal int CurrentDepth => State.Depth;

    public LoggingPayloadWriter(
        LoggingPayloadWriteTarget target,
        LoggingPayloadSerializerOptions? options = null)
    {
        _Target = target ?? throw new ArgumentNullException(nameof(target));
        State = target.WriterState;
        Options = options ?? LoggingPayloadSerializerOptions.Default;

        _IgnoreState = Options.DefaultIgnoreState;
    }

    public void BeginObject(string? typeName = null)
    {
        EnsureCanBeginNestedType();

        State.Push(ScopeType.Object, clearChildItemCount: true, typeOrPropertyName: typeName);
        _Target.OnBeginObject();
    }

    public void EndObject()
    {
        if (CurrentScope != ScopeType.Object)
            throw new InvalidOperationException();

        State.Pop();
        _Target.OnEndObject();
    }

    public void BeginArray(string? typeName = null)
    {
        EnsureCanBeginNestedType();

        State.Push(ScopeType.Array, clearChildItemCount: true, typeOrPropertyName: typeName);
        _Target.OnBeginArray();
    }

    public void EndArray()
    {
        if (CurrentScope != ScopeType.Array)
            throw new InvalidOperationException();

        State.Pop();
        _Target.OnEndArray();
    }

    public void BeginProperty(string propertyName)
    {
        if (CurrentScope != ScopeType.Object)
            throw new InvalidOperationException();

        if (string.IsNullOrEmpty(propertyName))
            throw new ArgumentNullException(nameof(propertyName));

        BeginPropertyInternal(propertyName);
    }

    public void EndProperty()
    {
        if (CurrentScope != ScopeType.Property)
            throw new InvalidOperationException();

        if (!State.HasValue)
            throw new InvalidOperationException();

        EndPropertyInternal();
    }

    internal void BeginValue()
    {
        if (CurrentScope == ScopeType.Object)
        {
            throw new InvalidOperationException();
        }

        BeginValueInternal();
    }

    public void Flush()
        => _Target.Flush();

    internal void BeginPropertyInternal(string propertyName)
    {
        Debug.Assert(CurrentScope == ScopeType.Object);
        Debug.Assert(!string.IsNullOrEmpty(propertyName));

        if (State.ChildItemCount > 0)
            _Target.OnWriteSeparator();

        State.Push(ScopeType.Property, clearChildItemCount: false, typeOrPropertyName: propertyName);
        _Target.OnBeginProperty(propertyName);
    }

    internal void EndPropertyInternal()
    {
        Debug.Assert(CurrentScope == ScopeType.Property);

        State.Pop();
        _Target.OnEndProperty();
    }

    internal void BeginValueInternal()
    {
        Debug.Assert(CurrentScope != ScopeType.Object);

        if (CurrentScope == ScopeType.Array)
        {
            if (State.ChildItemCount++ > 0)
            {
                Debug.Assert(State.HasValue);
                _Target.OnWriteSeparator();
            }
            else
            {
                State.HasValue = true;
            }
        }
        else
        {
            if (State.HasValue)
                throw new InvalidOperationException();
            State.HasValue = true;
        }
    }

    internal void WriteNullValueInternal()
        => _Target.OnWriteNullValue();

    internal void WriteValueInternal(string value)
    {
        Debug.Assert(value != null);

        _Target.OnWriteValue(value);
    }

    internal void WriteValueInternal(int value)
        => _Target.OnWriteValue(value);

    internal void WriteValueInternal(uint value)
        => _Target.OnWriteValue(value);

    internal void WriteValueInternal(long value)
        => _Target.OnWriteValue(value);

    internal void WriteValueInternal(ulong value)
        => _Target.OnWriteValue(value);

    internal void WriteValueInternal(short value)
        => _Target.OnWriteValue(value);

    internal void WriteValueInternal(ushort value)
        => _Target.OnWriteValue(value);

    internal void WriteValueInternal(byte value)
        => _Target.OnWriteValue(value);

    internal void WriteValueInternal(sbyte value)
        => _Target.OnWriteValue(value);

    internal void WriteValueInternal(char value)
        => _Target.OnWriteValue(value);

    internal void WriteValueInternal(double value)
        => _Target.OnWriteValue(value);

    internal void WriteValueInternal(float value)
        => _Target.OnWriteValue(value);

    internal void WriteValueInternal(decimal value)
        => _Target.OnWriteValue(value);

    internal void WriteValueInternal(byte[] value)
    {
        Debug.Assert(value != null);

        _Target.OnWriteValue(value);
    }

    internal void WriteValueInternal(bool value)
        => _Target.OnWriteValue(value);

    internal void WriteValueInternal(ReadOnlySpan<char> value)
        => _Target.OnWriteValue(value);

    internal void WriteValueInternal(ReadOnlySpan<byte> value)
        => _Target.OnWriteValue(value);

    internal void WriteValueInternal(ReadOnlySpan<char> value, LoggingPayloadMaskOptions maskOptions)
    {
        int maskLength = maskOptions.ComputeMaskLength(value.Length);
        int maxTotalLength = maskLength + maskOptions.UnmaskedCharacterCount;
        if (maxTotalLength <= 0)
            return;

        if (maxTotalLength <= 128)
        {
            Span<char> destination = stackalloc char[128];
            maskOptions.MaskValueIntoSpan(maskLength, value, destination, out int charsWritten);
            WriteValueInternal(destination.Slice(0, charsWritten));
        }
        else
        {
            char[] data = ArrayPool<char>.Shared.Rent(maxTotalLength);
            Span<char> destination = data;
            try
            {
                maskOptions.MaskValueIntoSpan(maskLength, value, destination, out int charsWritten);
                WriteValueInternal(destination.Slice(0, charsWritten));
            }
            finally
            {
                ArrayPool<char>.Shared.Return(data);
            }
        }
    }

    internal bool HandledAsNullOrIgnoredInternal<T>(
        TypeInspector<T> typeInspector,
#if NETCOREAPP3_1_OR_GREATER
        [NotNullWhen(false)]
#endif
        in T? value,
        string propertyName)
    {
        if (typeInspector.IsNull(in value!))
        {
            if (_IgnoreState < 1u)
            {
                BeginPropertyInternal(propertyName);
                BeginValueInternal();
                WriteNullValueInternal();
                EndPropertyInternal();
            }

            return true;
        }

        return typeof(T).IsValueType
            && _IgnoreState == 2u
            && EqualityComparer<T>.Default.Equals(default, value);
    }

    internal void HandleNullProperty(string propertyName)
    {
        if (_IgnoreState < 1u)
        {
            BeginProperty(propertyName);
            BeginValueInternal();
            WriteNullValueInternal();
            EndPropertyInternal();
        }
    }

    private void EnsureCanBeginNestedType()
    {
        ScopeType currentScope = CurrentScope;
        if (currentScope == ScopeType.Array)
        {
            if (State.ChildItemCount++ > 0)
            {
                Debug.Assert(State.HasValue);
                _Target.OnWriteSeparator();
            }
            else
            {
                State.HasValue = true;
            }
        }
        else
        {
            if (currentScope == ScopeType.Object)
                throw new InvalidOperationException();

            if (State.HasValue)
                throw new InvalidOperationException();
            State.HasValue = true;
        }
    }

    internal enum ScopeType
    {
        Root,
        Object,
        Array,
        Property
    }
}
