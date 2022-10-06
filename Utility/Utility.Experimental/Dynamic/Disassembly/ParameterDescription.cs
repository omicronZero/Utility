using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Dynamic.Disassembly
{
    public struct ParameterDescription
    {
        private IList<CustomModifier> _mods;

        public TypeDescription ParameterType { get; }

        public int CustomModifierCount
        {
            get { return _mods?.Count ?? 0; }
        }

        public CustomModifier GetCustomModifier(int index)
        {
            return _mods[index];
        }

        public ParameterDescription(TypeDescription parameterType, IList<CustomModifier> mods)
        {
            ParameterType = parameterType;
            _mods = mods;
        }
    }
}
