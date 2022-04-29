// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Microsoft.Extensions.Logging.Payloads;

internal sealed class LoggingPayloadWriterState
{
    private readonly HashSet<object> _VisitedRefs = new();
    private readonly Stack<State> _Stack = new();

    public int Depth => _Stack.Count;

    public LoggingPayloadWriter.ScopeType Scope { get; private set; } = LoggingPayloadWriter.ScopeType.Root;

    public string? TypeOrPropertyName { get; private set; }

    public bool HasValue { get; set; }

    public uint ChildItemCount { get; set; }

    public void Reset()
    {
        _VisitedRefs.Clear();
        _Stack.Clear();
        Scope = LoggingPayloadWriter.ScopeType.Root;
        TypeOrPropertyName = null;
        HasValue = false;
        ChildItemCount = 0;
    }

    public bool HasVisited(object value)
    {
        Debug.Assert(value != null);

        return !_VisitedRefs.Add(value);
    }

    public void Push(LoggingPayloadWriter.ScopeType newScope, bool clearChildItemCount, string? typeOrPropertyName = null)
    {
        uint childItemCount = ChildItemCount + 1;

        _Stack.Push(new()
        {
            Scope = Scope,
            TypeOrPropertyName = TypeOrPropertyName,
            ChildItemCount = childItemCount,
            HasValue = HasValue
        });

        Scope = newScope;
        TypeOrPropertyName = typeOrPropertyName;
        ChildItemCount = clearChildItemCount ? 0 : childItemCount;
        HasValue = false;
    }

    public void Pop()
    {
#if NETCOREAPP3_1_OR_GREATER
        if (_Stack.TryPop(out State state))
        {
            Scope = state.Scope;
            TypeOrPropertyName = state.TypeOrPropertyName;
            ChildItemCount = state.ChildItemCount;
            HasValue = state.HasValue;
        }
#else
        if (_Stack.Count > 0)
        {
            State state = _Stack.Pop();
            Scope = state.Scope;
            TypeOrPropertyName = state.TypeOrPropertyName;
            ChildItemCount = state.ChildItemCount;
            HasValue = state.HasValue;
        }
#endif
        else
        {
            Scope = LoggingPayloadWriter.ScopeType.Root;
            TypeOrPropertyName = null;
            ChildItemCount = 0;
            HasValue = false;
        }
    }

    public string ToPath()
    {
        StringBuilder builder = new(1024);

        Stack<State>.Enumerator enumerator = _Stack.GetEnumerator();

        if (ToPathRecursive(builder, ref enumerator))
            builder.Append(" > ");

        if (Scope != LoggingPayloadWriter.ScopeType.Root)
        {
            WriteStateToPath(
                builder,
                Scope,
                ChildItemCount,
                TypeOrPropertyName);
        }

        return builder.ToString();
    }

    private static bool ToPathRecursive(StringBuilder builder, ref Stack<State>.Enumerator enumerator)
    {
        if (!enumerator.MoveNext())
            return false;

        State state = enumerator.Current;

        if (ToPathRecursive(builder, ref enumerator))
            builder.Append(" > ");

        return WriteStateToPath(
            builder,
            state.Scope,
            state.ChildItemCount,
            state.TypeOrPropertyName);
    }

    private static bool WriteStateToPath(
        StringBuilder builder,
        LoggingPayloadWriter.ScopeType scope,
        uint childItemCount,
        string? typeOrPropertyName)
    {
        switch (scope)
        {
            case LoggingPayloadWriter.ScopeType.Root:
                return false;
            case LoggingPayloadWriter.ScopeType.Object:
                builder.Append("Object");
                break;
            case LoggingPayloadWriter.ScopeType.Array:
                builder.Append("Array");
                builder.Append('[');
                builder.Append(childItemCount);
                builder.Append(']');
                break;
            case LoggingPayloadWriter.ScopeType.Property:
                builder.Append(typeOrPropertyName);
                return true;
            default:
                Debug.Fail("This should be unreachable.");
                return false;
        }

        if (typeOrPropertyName != null)
        {
            builder.Append('{');
            builder.Append(typeOrPropertyName);
            builder.Append('}');
        }

        return true;
    }

    private struct State
    {
        public LoggingPayloadWriter.ScopeType Scope;
        public string? TypeOrPropertyName;
        public uint ChildItemCount;
        public bool HasValue;
    }
}
