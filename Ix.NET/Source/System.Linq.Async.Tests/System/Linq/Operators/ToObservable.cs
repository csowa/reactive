﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class ToObservable : AsyncEnumerableTests
    {
        [Fact]
        public void ToObservable_Null()
        {
            Assert.Throws<ArgumentNullException>(() => AsyncEnumerable.ToObservable<int>(null));
        }

        [Fact]
        public void ToObservable1()
        {
            var fail = false;
            var evt = new ManualResetEvent(false);

            var xs = AsyncEnumerable.Empty<int>().ToObservable();
            xs.Subscribe(new MyObserver<int>(
                x =>
                {
                    fail = true;
                },
                ex =>
                {
                    fail = true;
                    evt.Set();
                },
                () =>
                {
                    evt.Set();
                }
            ));

            evt.WaitOne();
            Assert.False(fail);
        }

        [Fact]
        public void ToObservable2()
        {
            var lst = new List<int>();
            var fail = false;
            var evt = new ManualResetEvent(false);

            var xs = Return42.ToObservable();
            xs.Subscribe(new MyObserver<int>(
                x =>
                {
                    lst.Add(x);
                },
                ex =>
                {
                    fail = true;
                    evt.Set();
                },
                () =>
                {
                    evt.Set();
                }
            ));

            evt.WaitOne();
            Assert.False(fail);
            Assert.True(lst.SequenceEqual(new[] { 42 }));
        }

        [Fact]
        public void ToObservable3()
        {
            var lst = new List<int>();
            var fail = false;
            var evt = new ManualResetEvent(false);

            var xs = AsyncEnumerable.Range(0, 10).ToObservable();
            xs.Subscribe(new MyObserver<int>(
                x =>
                {
                    lst.Add(x);
                },
                ex =>
                {
                    fail = true;
                    evt.Set();
                },
                () =>
                {
                    evt.Set();
                }
            ));

            evt.WaitOne();
            Assert.False(fail);
            Assert.True(lst.SequenceEqual(Enumerable.Range(0, 10)));
        }

        [Fact]
        public void ToObservable4()
        {
            var ex1 = new Exception("Bang!");
            var ex_ = default(Exception);
            var fail = false;
            var evt = new ManualResetEvent(false);

            var xs = Throw<int>(ex1).ToObservable();
            xs.Subscribe(new MyObserver<int>(
                x =>
                {
                    fail = true;
                },
                ex =>
                {
                    ex_ = ex;
                    evt.Set();
                },
                () =>
                {
                    fail = true;
                    evt.Set();
                }
            ));

            evt.WaitOne();
            Assert.False(fail);
            Assert.Equal(ex1, ex_);
        }

        [Fact]
        public void ToObservable_DisposesEnumeratorOnCompletion()
        {
            var fail = false;
            var evt = new ManualResetEvent(false);

            var ae = AsyncEnumerable.Create(
                _ => AsyncEnumerator.Create<int>(
                    () => new ValueTask<bool>(false),
                    () => { throw new InvalidOperationException(); },
                    () => { evt.Set(); return default; }));

            ae
                .ToObservable()
                .Subscribe(new MyObserver<int>(
                    x =>
                    {
                        fail = true;
                    },
                    ex =>
                    {
                        fail = true;
                    },
                    () =>
                    {
                    }
                ));

            evt.WaitOne();
            Assert.False(fail);
        }

        [Fact]
        public void ToObservable_DisposesEnumeratorWhenSubscriptionIsDisposed()
        {
            var fail = false;
            var evt = new ManualResetEvent(false);
            var subscription = default(IDisposable);
            var subscriptionAssignedTcs = new TaskCompletionSource<object>();

            var ae = AsyncEnumerable.Create(
                _ => AsyncEnumerator.Create(
                    async () =>
                    {
                        await subscriptionAssignedTcs.Task;
                        return true;
                    },
                    () => 1,
                    () =>
                    {
                        evt.Set();
                        return default;
                    }));

            subscription = ae
                .ToObservable()
                .Subscribe(new MyObserver<int>(
                    x =>
                    {
                        subscription.Dispose();
                    },
                    ex =>
                    {
                        fail = true;
                    },
                    () =>
                    {
                        fail = true;
                    }
                ));

            subscriptionAssignedTcs.SetResult(null);
            evt.WaitOne();

            Assert.False(fail);
        }

        [Fact]
        public void ToObservable_DesNotCallMoveNextAgainWhenSubscriptionIsDisposed()
        {
            var fail = false;
            var moveNextCount = 0;
            var evt = new ManualResetEvent(false);
            var subscription = default(IDisposable);
            var subscriptionAssignedTcs = new TaskCompletionSource<object>();

            var ae = AsyncEnumerable.Create(
                _ => AsyncEnumerator.Create(
                    async () =>
                    {
                        await subscriptionAssignedTcs.Task;

                        moveNextCount++;
                        return true;
                    },
                    () => 1,
                    () =>
                    {
                        evt.Set();
                        return default;
                    }));

            subscription = ae
                .ToObservable()
                .Subscribe(new MyObserver<int>(
                    x =>
                    {
                        subscription.Dispose();
                    },
                    ex =>
                    {
                        fail = true;
                    },
                    () =>
                    {
                        fail = true;
                    }
                ));

            subscriptionAssignedTcs.SetResult(null);
            evt.WaitOne();

            Assert.Equal(1, moveNextCount);
            Assert.False(fail);
        }

        private sealed class MyObserver<T> : IObserver<T>
        {
            private readonly Action<T> _onNext;
            private readonly Action<Exception> _onError;
            private readonly Action _onCompleted;

            public MyObserver(Action<T> onNext, Action<Exception> onError, Action onCompleted)
            {
                _onNext = onNext;
                _onError = onError;
                _onCompleted = onCompleted;
            }

            public void OnCompleted() => _onCompleted();

            public void OnError(Exception error) => _onError(error);

            public void OnNext(T value) => _onNext(value);
        }
    }
}
