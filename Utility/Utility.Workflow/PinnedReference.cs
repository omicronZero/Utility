using System;
using System.Threading;

namespace Utility
{
    public sealed class PinnedReference<T> : IDisposable
        where T : unmanaged
    {
        private readonly Pinned<T> _handle;
        private int _disposed;

        internal PinnedReference(Pinned<T> handle)
        {
            _handle = handle;
        }

        public T Instance => _handle.Instance;

        public unsafe T* Pointer
        {
            get
            {
                ThrowDisposed();

                return _handle.InternalPointer;
            }
        }

        private void Dispose(bool disposing)
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
                _handle.InternalFreeOnce();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void ThrowDisposed()
        {
            if (_disposed == 1)
                throw new ObjectDisposedException(GetType().Name);
        }

        ~PinnedReference()
        {
            Dispose(false);
        }

        public unsafe static explicit operator IntPtr(PinnedReference<T> pinned) => (IntPtr)pinned.Pointer;
    }
}