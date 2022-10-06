using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Utility
{
    public struct SpinProtected
    {
        private readonly LockHelper _lockHelper;

        public static SpinProtected Create()
        {
            return new SpinProtected(new LockHelper());
        }

        private SpinProtected(LockHelper helper)
        {
            _lockHelper = helper;
        }

        public bool Locked => (_lockHelper ?? throw new InvalidOperationException("Instance has not been initialized.")).Locked != 0;

        public Disposable Lock()
        {
            if (_lockHelper == null)
                throw new InvalidOperationException("Instance has not been initialized.");

            _lockHelper.Lock();

            return new Disposable(_lockHelper);
        }

        public Disposable TryLock(out bool locked)
        {
            locked = _lockHelper.TryLock();

            return new Disposable(_lockHelper);
        }

        internal sealed class LockHelper
        {
            public int Locked;
            public long Generation;

            public void Lock()
            {
                //(*)
                do
                {
                    //skip until Locked is 0
                    while (Locked != 0)
                    {
                        Thread.Yield();
                    }
                    //if Locked is still 0, allocate the resource and continue, otherwise continue (*)
                } while (Interlocked.CompareExchange(ref Locked, 0, 1) != 0);
            }

            public bool TryLock()
            {
                return Interlocked.CompareExchange(ref Locked, 0, 1) == 0;
            }
        }

        public struct Disposable : IDisposable
        {
            private LockHelper _helper;
            private readonly long _generation;

            internal Disposable(LockHelper helper)
            {
                _helper = helper;
                _generation = helper.Generation;
            }

            public void Dispose()
            {
                if (_helper == null)
                    return;

                if (_helper.Generation == _generation)
                {
                    _helper.Generation++;
                    _helper.Locked = 0;
                    _helper = null;
                }
            }
        }
    }

    public struct SpinProtected<T>
    {
        private readonly T _value;
        private readonly SpinProtected _innerHandle;

        public SpinProtected(T value)
        {
            _value = value;
            _innerHandle = SpinProtected.Create();
        }

        public bool Locked => _innerHandle.Locked;

        public SpinProtected.Disposable Lock(out T value)
        {
            var d = _innerHandle.Lock();

            value = _value;

            return d;
        }

        public SpinProtected.Disposable TryLock(out T value, out bool locked)
        {
            var d = _innerHandle.TryLock(out locked);

            value = locked ? _value : default;

            return d;
        }
    }
}
