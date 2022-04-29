// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Microsoft.Extensions.Logging.Payloads;

public class LoggingPayloadWriteTarget
{
    internal LoggingPayloadWriterState WriterState { get; } = new();

    public virtual void Reset()
        => WriterState.Reset();

    public virtual void OnBeginObject()
    {
    }

    public virtual void OnEndObject()
    {
    }

    public virtual void OnBeginArray()
    {
    }

    public virtual void OnEndArray()
    {
    }

    public virtual void OnBeginProperty(string propertyName)
    {
    }

    public virtual void OnEndProperty()
    {
    }

    public virtual void OnWriteSeparator()
    {
    }

    public virtual void OnWriteNullValue()
    {
    }

    public virtual void OnWriteValue(string value)
    {
    }

    public virtual void OnWriteValue(ReadOnlySpan<char> value)
    {
    }

    public virtual void OnWriteValue(int value)
    {
    }

    [CLSCompliant(false)]
    public virtual void OnWriteValue(uint value)
    {
    }

    public virtual void OnWriteValue(long value)
    {
    }

    [CLSCompliant(false)]
    public virtual void OnWriteValue(ulong value)
    {
    }

    public virtual void OnWriteValue(byte value)
    {
    }

    [CLSCompliant(false)]
    public virtual void OnWriteValue(sbyte value)
    {
    }

    public virtual void OnWriteValue(char value)
    {
    }

    public virtual void OnWriteValue(short value)
    {
    }

    [CLSCompliant(false)]
    public virtual void OnWriteValue(ushort value)
    {
    }

    public virtual void OnWriteValue(double value)
    {
    }

    public virtual void OnWriteValue(float value)
    {
    }

    public virtual void OnWriteValue(decimal value)
    {
    }

    public virtual void OnWriteValue(byte[] value)
    {
    }

    public virtual void OnWriteValue(ReadOnlySpan<byte> value)
    {
    }

    public virtual void OnWriteValue(bool value)
    {
    }

    public virtual void Flush()
    {
    }
}
