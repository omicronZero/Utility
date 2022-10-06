using System;
using System.Threading;

namespace Utility.Data.Resources
{
    public interface IAsyncResource : IResource
    {
        IDisposable Allocate(CancellationToken cancellationToken);
    }
}
