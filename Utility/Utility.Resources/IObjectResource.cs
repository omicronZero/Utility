using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Data.Resources
{
    public interface IObjectResource : IResource
    {
        object Resource { get; }
        Type ResourceType { get; }
    }

    public interface IObjectResource<out T> : IObjectResource
    {
        new T Resource { get; }
    }
}
