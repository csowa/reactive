﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace System.Linq
{
    public static partial class AsyncEnumerable
    {
        public static ValueTask<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, CancellationToken cancellationToken = default) =>
            ToDictionaryAsync(source, keySelector, comparer: null, cancellationToken);

        public static ValueTask<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw Error.ArgumentNull(nameof(source));
            if (keySelector == null)
                throw Error.ArgumentNull(nameof(keySelector));

            return Core(source, keySelector, comparer, cancellationToken);

            static async ValueTask<Dictionary<TKey, TSource>> Core(IAsyncEnumerable<TSource> _source, Func<TSource, TKey> _keySelector, IEqualityComparer<TKey> _comparer, CancellationToken _cancellationToken)
            {
                var d = new Dictionary<TKey, TSource>(_comparer);

                await foreach (var item in AsyncEnumerableExtensions.WithCancellation(_source, _cancellationToken).ConfigureAwait(false))
                {
                    var key = _keySelector(item);

                    d.Add(key, item);
                }

                return d;
            }
        }

        internal static ValueTask<Dictionary<TKey, TSource>> ToDictionaryAwaitAsyncCore<TSource, TKey>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<TKey>> keySelector, CancellationToken cancellationToken = default) =>
            ToDictionaryAwaitAsyncCore<TSource, TKey>(source, keySelector, comparer: null, cancellationToken);

        internal static ValueTask<Dictionary<TKey, TSource>> ToDictionaryAwaitAsyncCore<TSource, TKey>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<TKey>> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw Error.ArgumentNull(nameof(source));
            if (keySelector == null)
                throw Error.ArgumentNull(nameof(keySelector));

            return Core(source, keySelector, comparer, cancellationToken);

            static async ValueTask<Dictionary<TKey, TSource>> Core(IAsyncEnumerable<TSource> _source, Func<TSource, ValueTask<TKey>> _keySelector, IEqualityComparer<TKey> _comparer, CancellationToken _cancellationToken)
            {
                var d = new Dictionary<TKey, TSource>(_comparer);

                await foreach (var item in AsyncEnumerableExtensions.WithCancellation(_source, _cancellationToken).ConfigureAwait(false))
                {
                    var key = await _keySelector(item).ConfigureAwait(false);

                    d.Add(key, item);
                }

                return d;
            }
        }

#if !NO_DEEP_CANCELLATION
        internal static ValueTask<Dictionary<TKey, TSource>> ToDictionaryAwaitWithCancellationAsyncCore<TSource, TKey>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector, CancellationToken cancellationToken = default) =>
            ToDictionaryAwaitWithCancellationAsyncCore(source, keySelector, comparer: null, cancellationToken);

        internal static ValueTask<Dictionary<TKey, TSource>> ToDictionaryAwaitWithCancellationAsyncCore<TSource, TKey>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw Error.ArgumentNull(nameof(source));
            if (keySelector == null)
                throw Error.ArgumentNull(nameof(keySelector));

            return Core(source, keySelector, comparer, cancellationToken);

            static async ValueTask<Dictionary<TKey, TSource>> Core(IAsyncEnumerable<TSource> _source, Func<TSource, CancellationToken, ValueTask<TKey>> _keySelector, IEqualityComparer<TKey> _comparer, CancellationToken _cancellationToken)
            {
                var d = new Dictionary<TKey, TSource>(_comparer);

                await foreach (var item in AsyncEnumerableExtensions.WithCancellation(_source, _cancellationToken).ConfigureAwait(false))
                {
                    var key = await _keySelector(item, _cancellationToken).ConfigureAwait(false);

                    d.Add(key, item);
                }

                return d;
            }
        }
#endif

        public static ValueTask<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, CancellationToken cancellationToken = default) =>
            ToDictionaryAsync(source, keySelector, elementSelector, comparer: null, cancellationToken);

        public static ValueTask<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw Error.ArgumentNull(nameof(source));
            if (keySelector == null)
                throw Error.ArgumentNull(nameof(keySelector));
            if (elementSelector == null)
                throw Error.ArgumentNull(nameof(elementSelector));

            return Core(source, keySelector, elementSelector, comparer, cancellationToken);

            static async ValueTask<Dictionary<TKey, TElement>> Core(IAsyncEnumerable<TSource> _source, Func<TSource, TKey> _keySelector, Func<TSource, TElement> _elementSelector, IEqualityComparer<TKey> _comparer, CancellationToken _cancellationToken)
            {
                var d = new Dictionary<TKey, TElement>(_comparer);

                await foreach (var item in AsyncEnumerableExtensions.WithCancellation(_source, _cancellationToken).ConfigureAwait(false))
                {
                    var key = _keySelector(item);
                    var value = _elementSelector(item);

                    d.Add(key, value);
                }

                return d;
            }
        }

        internal static ValueTask<Dictionary<TKey, TElement>> ToDictionaryAwaitAsyncCore<TSource, TKey, TElement>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<TKey>> keySelector, Func<TSource, ValueTask<TElement>> elementSelector, CancellationToken cancellationToken = default) =>
            ToDictionaryAwaitAsyncCore<TSource, TKey, TElement>(source, keySelector, elementSelector, comparer: null, cancellationToken);

        internal static ValueTask<Dictionary<TKey, TElement>> ToDictionaryAwaitAsyncCore<TSource, TKey, TElement>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<TKey>> keySelector, Func<TSource, ValueTask<TElement>> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw Error.ArgumentNull(nameof(source));
            if (keySelector == null)
                throw Error.ArgumentNull(nameof(keySelector));
            if (elementSelector == null)
                throw Error.ArgumentNull(nameof(elementSelector));

            return Core(source, keySelector, elementSelector, comparer, cancellationToken);

            static async ValueTask<Dictionary<TKey, TElement>> Core(IAsyncEnumerable<TSource> _source, Func<TSource, ValueTask<TKey>> _keySelector, Func<TSource, ValueTask<TElement>> _elementSelector, IEqualityComparer<TKey> _comparer, CancellationToken _cancellationToken)
            {
                var d = new Dictionary<TKey, TElement>(_comparer);

                await foreach (var item in AsyncEnumerableExtensions.WithCancellation(_source, _cancellationToken).ConfigureAwait(false))
                {
                    var key = await _keySelector(item).ConfigureAwait(false);
                    var value = await _elementSelector(item).ConfigureAwait(false);

                    d.Add(key, value);
                }

                return d;
            }
        }

#if !NO_DEEP_CANCELLATION
        internal static ValueTask<Dictionary<TKey, TElement>> ToDictionaryAwaitWithCancellationAsyncCore<TSource, TKey, TElement>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector, Func<TSource, CancellationToken, ValueTask<TElement>> elementSelector, CancellationToken cancellationToken = default) =>
            ToDictionaryAwaitWithCancellationAsyncCore(source, keySelector, elementSelector, comparer: null, cancellationToken);

        internal static ValueTask<Dictionary<TKey, TElement>> ToDictionaryAwaitWithCancellationAsyncCore<TSource, TKey, TElement>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector, Func<TSource, CancellationToken, ValueTask<TElement>> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw Error.ArgumentNull(nameof(source));
            if (keySelector == null)
                throw Error.ArgumentNull(nameof(keySelector));
            if (elementSelector == null)
                throw Error.ArgumentNull(nameof(elementSelector));

            return Core(source, keySelector, elementSelector, comparer, cancellationToken);

            static async ValueTask<Dictionary<TKey, TElement>> Core(IAsyncEnumerable<TSource> _source, Func<TSource, CancellationToken, ValueTask<TKey>> _keySelector, Func<TSource, CancellationToken, ValueTask<TElement>> _elementSelector, IEqualityComparer<TKey> _comparer, CancellationToken _cancellationToken)
            {
                var d = new Dictionary<TKey, TElement>(_comparer);

                await foreach (var item in AsyncEnumerableExtensions.WithCancellation(_source, _cancellationToken).ConfigureAwait(false))
                {
                    var key = await _keySelector(item, _cancellationToken).ConfigureAwait(false);
                    var value = await _elementSelector(item, _cancellationToken).ConfigureAwait(false);

                    d.Add(key, value);
                }

                return d;
            }
        }
#endif
    }
}
