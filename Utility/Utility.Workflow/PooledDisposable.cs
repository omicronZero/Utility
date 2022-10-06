using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Utility
{

    public struct PooledDisposable : IDisposable
    {
        private readonly PooledDisposable<Action> _underlyingDisposable;

        private static Action<Action> HandleDelegate = Handle;

        public PooledDisposable(Action handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            //smartass (me) has spoken: we use PooledDisposable<Action> to avoid creating a new delegate, e.g. using EmptyType
            //Hence no instantiation of a reference type if there is a pool resource available, hence efficent, hence my rubber duck says \o/
            _underlyingDisposable = new PooledDisposable<Action>(handler, HandleDelegate);
        }

        private static void Handle(Action handler)
        {
            handler();
        }

        public void Dispose()
        {
            _underlyingDisposable.Dispose();
        }
    }

    public struct PooledDisposable<T> : IDisposable
    {
        internal static PooledDisposable<T> Empty => default;

        private readonly long _generation;
        private DisposableHelp _help;

        public PooledDisposable(T instance, Action<T> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            _help = DisposableHelp.Allocate();

            _help.SetData(handler, instance, out _generation);
        }

        public void Dispose()
        {
            _help?.Free(_generation);
            _help = null;
        }

        private sealed class DisposableHelp
        {
            private const int Limit = 64;

            private static readonly ConcurrentBag<DisposableHelp> FreeDisposables = new ConcurrentBag<DisposableHelp>();

            private Action<T> _handler;
            private T _instance;
            private long _generation;

            private DisposableHelp()
            { }

            public void SetData(Action<T> handler, T instance, out long generation)
            {
                if (handler == null)
                    throw new ArgumentNullException(nameof(handler));

                _instance = instance;
                _handler = handler;
                generation = _generation;
            }

            public void Free(long generation)
            {
                if (Interlocked.CompareExchange(ref _generation, generation + 1, generation) != generation)
                    return;

                Action<T> p = Interlocked.Exchange(ref _handler, null);

                if (p == null)
                    return;

                p(_instance);
                _instance = default;

                FreeDisposables.Add(this);

                if (FreeDisposables.Count > Limit)
                {
                    FreeDisposables.TryTake(out _);
                }
            }

            public static DisposableHelp Allocate()
            {
                DisposableHelp result;

                if (!FreeDisposables.TryTake(out result))
                {
                    result = new DisposableHelp();
                }

                return result;
            }
        }
    }
}
