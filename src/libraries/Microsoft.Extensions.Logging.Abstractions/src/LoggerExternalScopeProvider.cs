// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Default implementation of <see cref="IExternalScopeProvider"/>
    /// </summary>
    public class LoggerExternalScopeProvider : IExternalScopeProvider
    {
        private readonly AsyncLocal<Scope?> _currentScope = new AsyncLocal<Scope?>();
        private readonly IGlobalScopeProvider[]? _globalScopeProviders;

        /// <summary>
        /// Creates a new <see cref="LoggerExternalScopeProvider"/>.
        /// </summary>
        public LoggerExternalScopeProvider()
        { }

        /// <summary>
        /// Creates a new <see cref="LoggerExternalScopeProvider"/>.
        /// </summary>
        /// <param name="globalScopeProviders">The <see cref="IGlobalScopeProvider"/>s to use when emitting scopes.</param>
        public LoggerExternalScopeProvider(IGlobalScopeProvider[] globalScopeProviders)
        {
            _globalScopeProviders = globalScopeProviders;
        }

        /// <inheritdoc />
        public void ForEachScope<TState>(Action<object?, TState> callback, TState state)
        {
            void Report(Scope? current)
            {
                if (current == null)
                {
                    return;
                }
                Report(current.Parent);
                callback(current.State, state);
            }

            if (_globalScopeProviders != null)
            {
                foreach (IGlobalScopeProvider globalScopeProvider in _globalScopeProviders)
                {
                    globalScopeProvider.ForEachScope(callback, state);
                }
            }

            Report(_currentScope.Value);
        }

        /// <inheritdoc />
        public IDisposable Push(object? state)
        {
            Scope? parent = _currentScope.Value;
            var newScope = new Scope(this, state, parent);
            _currentScope.Value = newScope;

            return newScope;
        }

        private sealed class Scope : IDisposable
        {
            private readonly LoggerExternalScopeProvider _provider;
            private bool _isDisposed;

            internal Scope(LoggerExternalScopeProvider provider, object? state, Scope? parent)
            {
                _provider = provider;
                State = state;
                Parent = parent;
            }

            public Scope? Parent { get; }

            public object? State { get; }

            public override string? ToString()
            {
                return State?.ToString();
            }

            public void Dispose()
            {
                if (!_isDisposed)
                {
                    _provider._currentScope.Value = Parent;
                    _isDisposed = true;
                }
            }
        }
    }
}
