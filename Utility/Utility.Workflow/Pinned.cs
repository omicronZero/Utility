using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Utility.Workflow
{
    public sealed class Pinned<T>
        where T : unmanaged
    {
        private readonly T _instance;
        private GCHandle _handle;

        private int _allocationCount;
        private readonly object _syncObject;

        public Pinned(T instance, bool synchronized = true)
        {
            _instance = instance;
            _syncObject = synchronized ? new object() : null;
        }

        public T Instance => _instance;

        public bool IsPinned => _allocationCount > 0;

        public PinnedReference<T> Allocate()
        {
            if (_allocationCount == 0)
            {
                if (_syncObject == null)
                {
                    _allocationCount++;
                    _handle = GCHandle.Alloc(_instance, GCHandleType.Pinned);
                }
                else
                {
                    lock (_syncObject)
                    {
                        if (_allocationCount++ == 0)
                            _handle = GCHandle.Alloc(_instance, GCHandleType.Pinned);
                    }
                }
            }

            return new PinnedReference<T>(this);
        }

        internal unsafe T* InternalPointer
        {
            get
            {
                if (_allocationCount == 0)
                    ThrowNotAllocated();

                return (T*)_handle.AddrOfPinnedObject();
            }
        }

        internal void InternalFreeOnce()
        {
            if (_syncObject == null)
            {
                if (--_allocationCount == 0)
                {
                    _handle.Free();
                    _handle = default;
                }
            }
            else
            {
                lock (_syncObject)
                {
                    if (--_allocationCount == 0)
                    {
                        _handle.Free();
                        _handle = default;
                    }
                }
            }
        }

        ~Pinned()
        {
            if (_allocationCount > 0)
                _handle.Free();
        }

        private static unsafe void ThrowNotAllocated()
        {
            throw new InvalidOperationException("The current instance is not allocated.");
        }
    }
}
