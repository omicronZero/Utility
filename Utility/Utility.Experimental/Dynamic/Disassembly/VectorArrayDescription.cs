using System.Collections.Generic;

namespace Utility.Dynamic.Disassembly
{
    public struct VectorArrayDescription
    {
        public TypeDescription Type { get; }
        private IList<CustomModifier> _mods;

        public int CustomModifierCount
        {
            get { return _mods?.Count ?? 0; }
        }

        public CustomModifier GetCustomModifier(int index)
        {
            //TODO: add null check, index check
            return _mods[index];
        }

        public VectorArrayDescription(TypeDescription type, IList<CustomModifier> mods)
        {
            Type = type;
            _mods = mods;
        }
    }
}