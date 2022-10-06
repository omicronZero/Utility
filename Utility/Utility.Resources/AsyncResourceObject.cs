using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Data.Resources
{
    public class AsyncResourceObject<T> : Resource, IObjectResource<T>
    {
        private readonly object _syncRoot;
        private Task<IDisposable> _resourceHandleTask;

        private readonly IObjectResource<T> _innerRessource;

        public AsyncResourceObject(IObjectResource<T> innerResource)
        {
            if (innerResource == null)
                throw new ArgumentNullException(nameof(innerResource));

            _innerRessource = innerResource;
            _syncRoot = new object();
        }
        
        public T Resource
        {
            get
            {
                if (!IsAllocated)
                    throw new InvalidOperationException("Resource has not been allocated.");

                if (_resourceHandleTask != null)
                {
                    lock (_syncRoot)
                    {
                        if (_resourceHandleTask != null)
                        {
                            _resourceHandleTask.Wait();
                            _resourceHandleTask.Dispose();
                            _resourceHandleTask = null;
                        }
                    }
                }

                return _innerRessource.Resource;
            }
        }

        protected override void Create()
        {
            _resourceHandleTask = Task.Run(() => _innerRessource.Allocate());
        }

        protected override void Free()
        {
            if (_resourceHandleTask != null)
            {
                _resourceHandleTask.Dispose();
                _resourceHandleTask = null;
            }
        }

        Type IObjectResource.ResourceType => typeof(T);

        object IObjectResource.Resource => Resource;
    }
}
