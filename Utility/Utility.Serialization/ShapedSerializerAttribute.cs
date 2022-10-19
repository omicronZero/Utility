using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Serialization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Delegate,
        AllowMultiple = false, Inherited = false)]
    public class ShapedSerializerAttribute : ObjectSerializerAttribute
    {
        public bool BindByName { get; }

        public ShapedSerializerAttribute(bool bindByName = false)
            : base(typeof(ShapedSerializer))
        {
            BindByName = bindByName;
        }
    }
}
