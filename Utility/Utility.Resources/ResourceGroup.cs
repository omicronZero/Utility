using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utility.Data.Resources
{
    public class ResourceGroup : IResource, IAsyncResource
    {
        private readonly ICollection<IResource> _resources;
        private readonly ResourceLoader _resourceLoader;
        private readonly object _syncObject;

        private int _allocated;
        private IDisposable _groupDisposable;

        public ResourceGroup(params IResource[] resources)
            : this((ICollection<IResource>)(resources ?? throw new ArgumentNullException(nameof(resources))))
        { }

        public ResourceGroup(ICollection<IResource> resourceCollection)
            : this(null, resourceCollection)
        { }

        public ResourceGroup(ResourceLoader resourceLoader, params IResource[] resources)
            : this(resourceLoader, (ICollection<IResource>)(resources ?? throw new ArgumentNullException(nameof(resources))))
        { }

        public ResourceGroup(ResourceLoader resourceLoader, ICollection<IResource> resourceCollection)
        {
            if (resourceCollection == null)
                throw new ArgumentNullException(nameof(resourceCollection));

            _resourceLoader = resourceLoader;
            _resources = resourceCollection;

            _syncObject = new object();
        }

        protected ICollection<IResource> GetResources()
        {
            return _resources.AsReadonlyCollection();
        }

        protected virtual void Created()
        { }

        protected virtual void Destroyed()
        { }

        public IDisposable Allocate()
        {
            return Allocate(CancellationToken.None);
        }

        public IDisposable Allocate(CancellationToken token)
        {
            lock (_syncObject)
            {
                if (_allocated++ == 0)
                {
                    Initialize(token);
                }

                while (_groupDisposable == null)
                    Monitor.Wait(_syncObject);
            }

            return Disposable.Create(() =>
            {
                lock (_syncObject)
                {
                    if (--_allocated == 0)
                        Free();
                }
            }, true);
        }

        private void Free()
        {
            lock (_syncObject)
            {
                _groupDisposable.Dispose();
                _groupDisposable = null;
                Destroyed();
            }
        }

        private void Initialize(CancellationToken token)
        {
            var disposables = new IDisposable[_resources.Count];
            if (_resourceLoader == null)
            {
                //if no resource loader was specified, allocate the resource group items one by one sequentially

                int i = -1;

                foreach (IResource r in _resources)
                {
                    disposables[++i] = r.Allocate();
                }
            }
            else
            {
                var disposableTasks = new Task<IDisposable>[disposables.Length];

                int i = -1;

                //allocate a task for each resource
                foreach (IResource r in _resources)
                {
                    disposableTasks[++i] = _resourceLoader.BeginAllocate(r, token);
                }

                Task.WaitAll(disposableTasks, token);

                i = -1;

                bool success = true;

                //gather disposables as computed by the underlying resource loader
                foreach (Task<IDisposable> disposable in disposableTasks)
                {
                    if (disposable.IsCanceled)
                    {
                        success = false;
                        break;
                    }
                    disposables[++i] = disposable.Result;
                }

                if (!success)
                {
                    for (int j = 0; j <= i; j++)
                        disposables[j].Dispose();

                    for (int j = i + 1; j < disposableTasks.Length; j++)
                    {
                        if (disposableTasks[j].IsCompleted)
                        {
                            disposableTasks[j].Dispose();
                        }
                    }

                    token.ThrowIfCancellationRequested();
                    throw new OperationCanceledException();
                }

                _groupDisposable = Disposable.Join(disposables);

                Monitor.PulseAll(_syncObject);
            }

            //Created is called within lock(_syncObject)
            Created();
        }
    }
}
