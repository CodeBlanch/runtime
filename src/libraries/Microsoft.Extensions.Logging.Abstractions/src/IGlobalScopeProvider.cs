// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Represents a storage of global scope data.
    /// </summary>
    public interface IGlobalScopeProvider
    {
        /// <summary>
        /// Executes callback for each scope in the provider.
        /// All callbacks are guaranteed to be called inline from this method.
        /// </summary>
        /// <param name="callback">The callback to be executed for every scope in the provider.</param>
        /// <param name="state">The state object to be passed into the callback.</param>
        /// <typeparam name="TState">The type of state to accept.</typeparam>
        void ForEachScope<TState>(Action<object?, TState> callback, TState state);
    }
}
