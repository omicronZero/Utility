using System;
using System.Collections.Generic;
using System.Linq;

namespace Utility
{
    public static partial class Disposable
    {
        public static IDisposable Empty { get; } = new EmptyDisposable();

        public static IDisposable Create(Action handler)
        {
            return Create(handler, false);
        }

        public static IDisposable Create(Action handler, bool callOnce)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return new DisposableHandle(handler, callOnce);
        }

        public static IDisposable Join(IEnumerable<IDisposable> disposables)
        {
            return Join(disposables, false, true);
        }

        public static IDisposable Join(IEnumerable<IDisposable> disposables, bool callOnce)
        {
            return Join(disposables, callOnce, true);
        }

        public static IDisposable Join(IEnumerable<IDisposable> disposables, bool callOnce, bool rethrowExceptions)
        {
            if (disposables == null)
                throw new ArgumentNullException(nameof(disposables));

            return Create(() =>
            {
                List<Exception> e = null;
                foreach (IDisposable d in disposables)
                {
                    try
                    {
                        d?.Dispose();
                    }
                    catch (Exception ex)
                    {
                        if (rethrowExceptions)
                        {
                            if (e == null)
                                e = new List<Exception>();

                            e.Add(ex);
                        }
                    }
                }

                if (e != null)
                    throw new AggregateException(e);
            }, callOnce);
        }

        public static IDisposable Weak(IDisposable disposable, bool callOnce)
        {
            if (disposable == null)
                throw new ArgumentNullException(nameof(disposable));

            WeakReference<IDisposable> r = new WeakReference<IDisposable>(disposable);

            disposable = null;

            return Create(() =>
            {
                IDisposable d;

                if (r.TryGetTarget(out d))
                {
                    d.Dispose();
                }
            }, callOnce);
        }

        private sealed class DisposableHandle : IDisposable
        {
            private Action _handler;

            public DisposableHandle(Action handler, bool callOnce)
            {
                if (handler == null)
                    throw new ArgumentNullException(nameof(handler));

                if (callOnce)
                {
                    Action h = handler;

                    handler = () =>
                    {
                        h();
                        _handler = null;
                    };
                }

                _handler = handler;
            }

            public void Dispose()
            {
                _handler?.Invoke();
            }
        }

        private sealed class EmptyDisposable : IDisposable
        {
            public void Dispose()
            { }
        }
    }
}
