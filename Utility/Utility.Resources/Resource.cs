using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Data.Resources
{
    public abstract class Resource : IResource
    {
        //TODO: make all resources synchronized
        protected int AllocationCount { get; private set; }

        protected abstract void Free();
        protected abstract void Create();

        public bool IsAllocated => AllocationCount > 0;

        protected virtual void Releasing()
        { }

        protected virtual void OnAllocating()
        { }

        protected virtual void OnAllocated()
        { }

        protected virtual void Released()
        { }

        public IDisposable Allocate()
        {
            int c = AllocationCount++;

            OnAllocating();

            if (c == 0)
            {
                Create();
            }

            OnAllocated();

            return Disposable.Create(() =>
                {
                    int rc = --AllocationCount;

                    Releasing();

                    if (rc == 0)
                    {
                        Free();
                    }

                    Released();
                }, true);
        }
    }
}
