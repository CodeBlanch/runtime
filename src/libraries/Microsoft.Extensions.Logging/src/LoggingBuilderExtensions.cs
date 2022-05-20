// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Extension methods for setting up logging services in an <see cref="ILoggingBuilder" />.
    /// </summary>
    public static class LoggingBuilderExtensions
    {
        /// <summary>
        /// Sets a minimum <see cref="LogLevel"/> requirement for log messages to be logged.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to set the minimum level on.</param>
        /// <param name="level">The <see cref="LogLevel"/> to set as the minimum.</param>
        /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
        public static ILoggingBuilder SetMinimumLevel(this ILoggingBuilder builder, LogLevel level)
        {
            builder.Services.Add(ServiceDescriptor.Singleton<IConfigureOptions<LoggerFilterOptions>>(
                new DefaultLoggerLevelConfigureOptions(level)));
            return builder;
        }

        /// <summary>
        /// Adds the given <see cref="ILoggerProvider"/> to the <see cref="ILoggingBuilder"/>
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to add the <paramref name="provider"/> to.</param>
        /// <param name="provider">The <see cref="ILoggerProvider"/> to add to the <paramref name="builder"/>.</param>
        /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
        public static ILoggingBuilder AddProvider(this ILoggingBuilder builder, ILoggerProvider provider)
        {
            builder.Services.AddSingleton(provider);
            return builder;
        }

        /// <summary>
        /// Removes all <see cref="ILoggerProvider"/>s from <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to remove <see cref="ILoggerProvider"/>s from.</param>
        /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
        public static ILoggingBuilder ClearProviders(this ILoggingBuilder builder)
        {
            builder.Services.RemoveAll<ILoggerProvider>();
            return builder;
        }

        /// <summary>
        /// Configure the <paramref name="builder"/> with the <see cref="LoggerFactoryOptions"/>.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to be configured with <see cref="LoggerFactoryOptions"/></param>
        /// <param name="action">The action used to configure the logger factory</param>
        /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
        public static ILoggingBuilder Configure(this ILoggingBuilder builder, Action<LoggerFactoryOptions> action)
        {
            builder.Services.Configure(action);
            return builder;
        }

        /// <summary>
        /// Adds the given scope values as a <see cref="IGlobalScopeProvider"/> to the <see cref="ILoggingBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to add the <see cref="IGlobalScopeProvider"/> to.</param>
        /// <param name="values">The values to return as a global scope.</param>
        /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
        public static ILoggingBuilder AddGlobalScopeValues(this ILoggingBuilder builder, IReadOnlyDictionary<string, object?> values)
        {
            ThrowHelper.ThrowIfNull(values);

            builder.Services.AddSingleton<IGlobalScopeProvider>(new LoggerGlobalScopeValuesProvider(values));
            return builder;
        }

        /// <summary>
        /// Adds the given scope value factories as a <see cref="IGlobalScopeProvider"/> to the <see cref="ILoggingBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to add the <see cref="IGlobalScopeProvider"/> to.</param>
        /// <param name="valueFactories">Factory functions used to populate the values returned as a global scope.</param>
        /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
        public static ILoggingBuilder AddGlobalScopeFactory(this ILoggingBuilder builder, params Func<KeyValuePair<string, object?>>[] valueFactories)
        {
            ThrowHelper.ThrowIfNull(valueFactories);

            builder.Services.AddSingleton<IGlobalScopeProvider>(new LoggerGlobalScopeValuesFactoryProvider(valueFactories));
            return builder;
        }

        /// <summary>
        /// Adds the given <see cref="IGlobalScopeProvider"/> to the <see cref="ILoggingBuilder"/>.
        /// </summary>
        /// <typeparam name="T">The global scope provider type.</typeparam>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to add the <typeparamref name="T"/> to.</param>
        /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
        public static ILoggingBuilder AddGlobalScopeProvider<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        T>(this ILoggingBuilder builder)
            where T : class, IGlobalScopeProvider
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IGlobalScopeProvider, T>());
            return builder;
        }

        private sealed class LoggerGlobalScopeValuesProvider : IGlobalScopeProvider
        {
            private readonly Scope _scope;

            public LoggerGlobalScopeValuesProvider(IReadOnlyDictionary<string, object?> values)
            {
                _scope = new Scope(values.ToList());
            }

            public void ForEachScope<TState>(Action<object?, TState> callback, TState state)
            {
                callback(_scope, state);
            }

            private sealed class Scope : IReadOnlyList<KeyValuePair<string, object?>>
            {
                private readonly IReadOnlyList<KeyValuePair<string, object?>> _values;
                private string? _cachedToString;

                public Scope(IReadOnlyList<KeyValuePair<string, object?>> values)
                {
                    _values = values;
                    Count = values.Count;
                }

                public KeyValuePair<string, object?> this[int index] => _values[index];

                public int Count { get; }

                public override string ToString()
                {
                    if (_cachedToString == null)
                    {
                        StringBuilder sb = new StringBuilder();

                        for (int i = 0; i < Count; i++)
                        {
                            if (i > 0)
                            {
                                sb.Append(", ");
                            }

                            KeyValuePair<string, object?> item = this[i];

                            sb.Append(item.Key);
                            sb.Append(':');
                            sb.Append(item.Value);
                        }

                        _cachedToString = sb.ToString();
                    }

                    return _cachedToString;
                }

                public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
                    => _values.GetEnumerator();

                IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            }
        }

        private sealed class LoggerGlobalScopeValuesFactoryProvider : IGlobalScopeProvider
        {
            private readonly Scope _scope;

            public LoggerGlobalScopeValuesFactoryProvider(Func<KeyValuePair<string, object?>>[] valueFactories)
            {
                _scope = new Scope(valueFactories);
            }

            public void ForEachScope<TState>(Action<object?, TState> callback, TState state)
            {
                callback(_scope, state);
            }

            private sealed class Scope : IReadOnlyList<KeyValuePair<string, object?>>
            {
                private readonly Func<KeyValuePair<string, object?>>[] _valueFactories;

                public Scope(Func<KeyValuePair<string, object?>>[] valueFactories)
                {
                    _valueFactories = valueFactories;
                    Count = valueFactories.Length;
                }

                public KeyValuePair<string, object?> this[int index]
                {
                    get
                    {
                        return index < 0 || index >= Count
                            ? throw new ArgumentOutOfRangeException(nameof(index))
                            : _valueFactories[index]();
                    }
                }

                public int Count { get; }

                public override string ToString()
                {
                    StringBuilder sb = new StringBuilder();

                    for (int i = 0; i < Count; i++)
                    {
                        if (i > 0)
                        {
                            sb.Append(", ");
                        }

                        KeyValuePair<string, object?> item = this[i];

                        sb.Append(item.Key);
                        sb.Append(':');
                        sb.Append(item.Value);
                    }

                    return sb.ToString();
                }

                public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
                {
                    for (int i = 0; i < Count; i++)
                    {
                        yield return this[i];
                    }
                }

                IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            }
        }
    }
}
