using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Data.Resources
{
    public interface IResourceDictionary<in TID, TResourceType>
    {
        bool GetResource(TID id, out TResourceType resource);
    }
}
