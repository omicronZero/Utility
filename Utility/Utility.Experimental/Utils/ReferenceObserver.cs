using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Utility
{
    public static class ReferenceObserver
    {
        private static readonly DisposableList<(WeakReference<object>, Action)> _entities;
        private static bool _isRegistered;

        static ReferenceObserver()
        {
            _entities = new DisposableList<(WeakReference<object>, Action)>(true);
        }

        public static IDisposable Observe(object entity, Action freeAction)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (freeAction == null)
                throw new ArgumentNullException(nameof(freeAction));

            lock (_entities.SyncObject)
            {
                if (!_isRegistered)
                {
                    _isRegistered = true;
                    GC.RegisterForFullGCNotification(GC.MaxGeneration, 1);

                    ThreadPool.QueueUserWorkItem((o) => HandleObservation());
                    Monitor.Wait(_entities.SyncObject);
                }

                return _entities.Add((new WeakReference<object>(entity), () =>
                    {
                        lock (_entities.SyncObject)
                        {
                            if (_entities.Count == 0)
                            {
                                GC.CancelFullGCNotification();
                                _isRegistered = false;
                            }
                        }
                        freeAction();
                    }
                ));
            }
        }

        private static void HandleObservation()
        {
            lock (_entities.SyncObject)
            {
                Monitor.PulseAll(_entities.SyncObject);
            }

            while (_isRegistered)
            {
                GCNotificationStatus result = GC.WaitForFullGCComplete();

                lock (_entities.SyncObject)
                {
                    if (!_isRegistered)
                    {
                        break;
                    }

                    if (result == GCNotificationStatus.Canceled)
                    {
                        _isRegistered = false;

                        if (_entities.Count > 0)
                            throw new InvalidOperationException("GC notifications have been canceled even though there are still observed objects.");

                        break;
                    }
                }

                if (result == GCNotificationStatus.Succeeded)
                {
                    new Thread(HandleFree).Start();
                }
            }
        }

        private static void HandleFree()
        {
            GC.WaitForPendingFinalizers();

            lock (_entities.SyncObject)
            {
                foreach (var v in _entities.GetHandleIterator())
                {
                    var (obj, action) = v.Key;

                    if (!obj.TryGetTarget(out _))
                    {
                        action();
                        v.Value.Dispose();
                    }
                }
            }
        }
    }
}
