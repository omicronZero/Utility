using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Utility.Dynamic.Disassembly
{
    internal class FieldSignature : Signature
    {
        public Module Module { get; }
        public TypeDescription FieldType { get; }

        private readonly IList<CustomModifier> _mods;

        public int CustomModifierCount
        {
            get { return _mods?.Count ?? 0; }
        }

        public CustomModifier GetCustomModifier(int index)
        {
            //TODO: add null check, index check
            return _mods[index];
        }

        public override SignatureHelper ToSignatureHelper(ModuleBuilder module)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));

            SignatureHelper h = SignatureHelper.GetFieldSigHelper(module);

            Type[] optModifiers;
            Type[] reqModifiers;
            if (_mods == null)
                optModifiers = reqModifiers = null;
            else
            {
                List<Type> o = null;
                List<Type> r = null;

                for (int i = 0; i < _mods.Count; i++)
                {
                    CustomModifier mod = _mods[i];
                    (mod.Required ? (r ?? (r = new List<Type>())) : (o ?? (o = new List<Type>()))).Add(Module.ResolveType(mod.MetadataToken));
                }
                optModifiers = o?.ToArray();
                reqModifiers = r?.ToArray();
            }

            h.AddArgument(FieldType.ToType(), reqModifiers, optModifiers);

            return h;
        }

        internal FieldSignature(Module module, TypeDescription fieldType, IList<CustomModifier> mods)
            : base(SignatureType.Field)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));

            Module = module;
            FieldType = fieldType;
            _mods = mods;
        }
    }
}