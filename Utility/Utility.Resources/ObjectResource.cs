using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Data.Resources
{
    public abstract class ObjectResource<T> : Resource, IObjectResource<T>
    {
        private T _resource;

        protected abstract T CreateObject();

        public T Resource
        {
            get
            {
                if (!IsAllocated)
                    throw new InvalidOperationException("The resource has not been allocated.");

                return _resource;
            }
        }

        Type IObjectResource.ResourceType => typeof(T);

        object IObjectResource.Resource => Resource;

        protected T GetCurrentValue() => _resource;

        protected sealed override void Create()
        {
            _resource = CreateObject();
        }

        protected override void Free()
        {
            T res = _resource;
            _resource = default;

            if (res is IDisposable d)
            {
                d.Dispose();
            }
        }

        public static ObjectResource<T> FromDelegate(Func<T> factory)
        {
            return FromDelegate(factory, true);
        }

        public static ObjectResource<T> FromDelegate(Func<T> factory, bool deleteOnFree)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            return new FactoryObjectResource(factory, deleteOnFree);
        }

        private sealed class FactoryObjectResource : ObjectResource<T>
        {
            private readonly Func<T> _factory;
            private readonly bool _deleteOnFree;
            private bool _allocated;

            public FactoryObjectResource(Func<T> factory, bool deleteOnFree)
            {
                if (factory == null)
                    throw new ArgumentNullException(nameof(factory));

                _factory = factory;
                _deleteOnFree = deleteOnFree;
            }

            protected override void OnAllocated()
            {
                base.OnAllocated();
                _allocated = true;
            }

            protected override void Free()
            {
                if (_deleteOnFree)
                {
                    base.Free();
                    _allocated = false;
                }
            }

            protected override T CreateObject()
            {
                return _allocated ? GetCurrentValue() : _factory();
            }
        }
    }
}
