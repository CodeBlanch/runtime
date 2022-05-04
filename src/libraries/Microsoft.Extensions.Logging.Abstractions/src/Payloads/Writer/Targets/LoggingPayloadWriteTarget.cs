// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Microsoft.Extensions.Logging.Payloads;

public abstract class LoggingPayloadWriteTarget
{
    internal LoggingPayloadWriterState WriterState { get; } = new();

    public bool InProperty => WriterState.Scope == LoggingPayloadWriter.ScopeType.Property;

    protected LoggingPayloadWriteTarget()
    {
    }

    public void Reset()
    {
        WriterState.Reset();
        OnReset();
    }

    protected virtual void OnReset()
    {
    }

    public abstract void AppendBeginObject();

    public abstract void AppendEndObject();

    public abstract void AppendBeginArray();

    public abstract void AppendEndArray();

    public abstract void AppendBeginProperty(string propertyName);

    public virtual void AppendEndProperty()
    {
    }

    public virtual void AppendSeparator()
    {
    }

    public abstract void AppendNullValue();

    public abstract void AppendValue(string value);

    public virtual void AppendValue(ReadOnlySpan<char> value)
        => AppendValue(value.ToString());

    public virtual void AppendValue(int value)
        => AppendValue((long)value);

    [CLSCompliant(false)]
    public virtual void AppendValue(uint value)
        => AppendValue((long)value);

    public abstract void AppendValue(long value);

    [CLSCompliant(false)]
    public virtual void AppendValue(ulong value)
        => AppendValue((long)value);

    public virtual void AppendValue(byte value)
        => AppendValue((long)value);

    [CLSCompliant(false)]
    public virtual void AppendValue(sbyte value)
        => AppendValue((long)value);

    public virtual void AppendValue(char value)
        => AppendValue((long)value);

    public virtual void AppendValue(short value)
        => AppendValue((long)value);

    [CLSCompliant(false)]
    public virtual void AppendValue(ushort value)
        => AppendValue((long)value);

    public abstract void AppendValue(double value);

    public virtual void AppendValue(float value)
        => AppendValue((double)value);

    public virtual void AppendValue(decimal value)
        => AppendValue((double)value);

    public abstract void AppendValue(byte[] value);

    public virtual void AppendValue(ReadOnlySpan<byte> value)
        => AppendValue(value.ToArray());

    public abstract void AppendValue(bool value);

    public virtual void Flush()
    {
    }
}
