using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Utility.Dynamic.Disassembly
{
    public class PropertySignature : Signature
    {
        public Module Module { get; }
        public bool HasThis { get; }
        public TypeDescription PropertyType { get; }

        private readonly IList<CustomModifier> _mods;
        private readonly ParameterDescription[] _param;

        public int ParameterCount
        {
            get { return _param.Length; }
        }

        public ParameterDescription GetParameter(int index)
        {
            //TODO: add index check
            return _param[index];
        }

        public int CustomModifierCount
        {
            get { return _mods?.Count ?? 0; }
        }

        public CustomModifier GetCustomModifier(int index)
        {
            //TODO: add null check, index check
            return _mods[index];
        }

        internal PropertySignature(Module module, bool hasThis, TypeDescription propertyType, IList<CustomModifier> mods, ParameterDescription[] parameters)
            : base(SignatureType.Property)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(Module));
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            
            Module = module;
            HasThis = hasThis;
            PropertyType = propertyType;
            _mods = mods;
            _param = parameters;
        }

        public override SignatureHelper ToSignatureHelper(ModuleBuilder module)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));

            //TODO: make use of instance methods of signature helper instead of putting together the types
            
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

            List<Type>[] paramReqModifiers = null;
            List<Type>[] paramOptModifiers = null;

            for (int i = 0; i < _param.Length; i++)
            {
                ParameterDescription p = _param[i];

                for (int j = 0; j < p.CustomModifierCount; j++)
                {
                    CustomModifier cm = p.GetCustomModifier(j);

                    List<Type>[] target = (cm.Required ? paramReqModifiers : paramOptModifiers);
                    List<Type> targetList;
                    if (target == null)
                    {
                        target = new List<Type>[_param.Length];
                        if (cm.Required)
                            paramReqModifiers = target;
                        else
                            paramOptModifiers = target;
                    }
                    targetList = target[i];

                    if (targetList == null)
                    {
                        targetList = new List<Type>();
                        target[i] = targetList;
                    }

                    targetList.Add(Module.ResolveType(cm.MetadataToken));
                }
            }

            return SignatureHelper.GetPropertySigHelper(
                Module,
                HasThis ? CallingConventions.HasThis : CallingConventions.Standard,
                PropertyType.ToType(),
                reqModifiers,
                optModifiers,
                Array.ConvertAll(_param, (p) => p.ParameterType.ToType()),
                paramReqModifiers == null ? null : Array.ConvertAll(paramReqModifiers, (s) => s == null ? null : s.ToArray()),
                paramOptModifiers == null ? null : Array.ConvertAll(paramReqModifiers, (s) => s == null ? null : s.ToArray()));
        }
    }
}