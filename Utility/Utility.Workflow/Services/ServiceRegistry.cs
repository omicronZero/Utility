using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Utility.Workflow.Services
{
    public class ServiceRegistry : IServiceRegistry
    {
        private readonly Dictionary<Type, object> _services;

        private readonly ReaderWriterLockSlim _lockHandle;

        public ServiceRegistry(bool synchronized = true)
        {
            _services = new Dictionary<Type, object>();

            //we may have recursive calls to the registry during factory-based initialization
            _lockHandle = synchronized ? null : new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        public object GetService(Type serviceType)
        {
            if (_lockHandle == null)
                return _services.TryGetValue(serviceType, out var v) ? v : null;

            _lockHandle.EnterReadLock();

            object service;

            try
            {
                if (!_services.TryGetValue(serviceType, out service))
                    service = null;
            }
            finally
            {
                _lockHandle.ExitReadLock();
            }

            return service;
        }

        public void SetService(Type serviceType, object instance)
        {
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            if (_lockHandle == null)
            {
                _services[serviceType] = instance;
            }
            else
            {
                _lockHandle.EnterWriteLock();

                try
                {
                    _services[serviceType] = instance;
                }
                finally
                {
                    _lockHandle.ExitWriteLock();
                }
            }
        }

        public void RegisterService(Type serviceType, object instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            bool success;

            if (_lockHandle == null)
            {
                success = _services.TryAdd(serviceType, instance);
            }
            else
            {
                _lockHandle.EnterWriteLock();

                try
                {
                    success = _services.TryAdd(serviceType, instance);
                }
                finally
                {
                    _lockHandle.ExitWriteLock();
                }
            }

            if (!success)
                throw new InvalidOperationException($"A service has already been registered for type {serviceType.FullName}.");
        }

        public bool RegisterIfNotAvailable(Type serviceType, Func<object> serviceFactory, out object service)
        {
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));
            if (serviceFactory == null)
                throw new ArgumentNullException(nameof(serviceFactory));

            service = GetService(serviceType);

            if (service != null)
                return false;

            if (_lockHandle == null)
            {
                service = serviceFactory();
                _services[serviceType] = service;
            }
            else
            {
                _lockHandle.EnterWriteLock();


                try
                {
                    if (_services.TryGetValue(serviceType, out service))
                        return false;

                    service = serviceFactory();

                    _services[serviceType] = service;

                }
                finally
                {
                    _lockHandle.ExitWriteLock();
                }
            }

            return true;
        }

        public bool UnregisterService(Type serviceType)
        {
            if (_lockHandle == null)
            {
                return _services.Remove(serviceType);
            }
            else
            {
                _lockHandle.EnterWriteLock();

                try
                {
                    return _services.Remove(serviceType);
                }
                finally
                {
                    _lockHandle.ExitWriteLock();
                }
            }
        }
    }
}
