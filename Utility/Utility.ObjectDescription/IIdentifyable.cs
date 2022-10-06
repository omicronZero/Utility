using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.ObjectDescription
{
    public interface IIdentifyable<TId>
    {
        TId Id { get; }
    }
}
