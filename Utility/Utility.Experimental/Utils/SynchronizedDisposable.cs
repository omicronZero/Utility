using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Utility
{
    public class SynchronizedDisposable : IDisposable
    {
        private int _actionCount;
        private readonly object _syncRoot;

        public bool IsDisposed { get; private set; }

        public string ObjectName { get; }

        public SynchronizedDisposable(string objectName)
        {
            ObjectName = objectName;
            _syncRoot = new object();
        }

        public void BeginAction()
        {
            lock (_syncRoot)
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(ObjectName);

                _actionCount++;
            }
        }

        public void EndAction()
        {
            lock (_syncRoot)
            {
                if (_actionCount == 0)
                    throw new InvalidOperationException("The object has no pending action.");

                if (--_actionCount == 0)
                    Monitor.PulseAll(_syncRoot);
            }
        }

        public IDisposable BeginHandledAction()
        {
            BeginAction();

            return Disposable.Create(EndAction, true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (_syncRoot)
                {
                    while (_actionCount > 0)
                    {
                        Monitor.Wait(_syncRoot);
                    }
                }
            }
            IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
