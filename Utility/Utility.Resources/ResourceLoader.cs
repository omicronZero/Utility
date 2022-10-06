using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Utility.Data.Resources
{
    public class ResourceLoader
    {
        public bool Async { get; }

        public static ResourceLoader ImmediateLoader { get; } = new ResourceLoader(false);
        public static ResourceLoader ThreadPoolBasedLoader { get; } = new ResourceLoader(true);

        protected ResourceLoader(bool async)
        {
            Async = async;
        }

        public virtual Task<IDisposable> BeginAllocate(IResource resource, CancellationToken cancellationToken)
        {
            if (Async)
            {
                if (resource is IAsyncResource asyncResource)
                {
                    return Task.Run(() => asyncResource.Allocate(cancellationToken), cancellationToken);
                }
                else
                {
                    return Task.Run(() => resource.Allocate(), cancellationToken);
                }
            }
            else
            {
                return new Task<IDisposable>(() => resource.Allocate(), cancellationToken);
            }
        }
    }
}
