using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Data.Resources
{
    public class BufferedObjectResource<T> : Resource, IObjectResource<T>
    {
        private readonly IObjectResource<T> _resource;
        private WeakReference<LazyResource> _weakResourceReference;
        private LazyResource _resourceReference;

        public BufferedObjectResource(IObjectResource<T> resource)
        {
            if (resource == null)
                throw new ArgumentNullException(nameof(resource));

            _resource = resource;
        }

        public T Resource
        {
            get
            {
                if (!IsAllocated)
                    throw new InvalidOperationException("The resource has not been allocated.");

                return _resourceReference.Resource;
            }
        }

        Type IObjectResource.ResourceType => typeof(T);

        object IObjectResource.Resource => Resource;

        protected override void Create()
        {
            if (!_weakResourceReference.TryGetTarget(out _resourceReference))
            {
                _resourceReference = new LazyResource(this);
                _weakResourceReference = new WeakReference<LazyResource>(_resourceReference);
            }
        }

        protected override void Free()
        {
            _resourceReference = null;
        }

        public static BufferedObjectResource<T> FromDelegate(Func<T> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            return new BufferedObjectResource<T>(ObjectResource<T>.FromDelegate(factory));
        }

        private sealed class LazyResource
        {
            private BufferedObjectResource<T> _instance;
            private IDisposable _handle;

            public T Resource { get; }

            public LazyResource(BufferedObjectResource<T> instance)
            {
                _instance = instance;
                _handle = instance._resource.Allocate();
                Resource = instance._resource.Resource;
            }

            ~LazyResource()
            {
                _handle.Dispose();
            }
        }
    }
}
