using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Data.Resources
{
    public interface IResource
    {
        /// <summary>
        /// Allocates the specified resource once. While at least one allocation is active, the resource is generated and available.
        /// </summary>
        /// <returns>A handle used in the resource allocation. If the handle is disposed, the allocation is released once.</returns>
        /// <remarks>The handle returned by Allocate is not thread-safe but all operations on resources should be.</remarks>
        IDisposable Allocate();
    }
}
