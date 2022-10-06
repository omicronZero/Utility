using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Dynamic
{
    public abstract class DescriptionProvider<TDescription>
    {
        public abstract TDescription[] GetDescriptions(Type type);
    }
}
