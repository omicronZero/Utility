using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using InteropServices = System.Runtime.InteropServices;
using Reflection = System.Reflection;

namespace Utility.Dynamic.Disassembly
{
    public class MethodSignature : Signature
    {
        public Module Module { get; }
        public CallingConvention CallingConvention { get; }
        public ParameterDescription ReturnType { get; }

        private ParameterDescription[] _param;

        public int SentinelIndex { get; }

        internal MethodSignature(Module module, CallingConvention callingConvention, ParameterDescription returnType, ParameterDescription[] parameterType, int sentinelIndex)
            : base(SignatureType.Method)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));

            if (parameterType == null)
                throw new ArgumentNullException(nameof(parameterType));

            Module = module;
            CallingConvention = callingConvention;
            ReturnType = returnType;
            _param = parameterType;
            SentinelIndex = sentinelIndex;
        }

        public int ParameterCount
        {
            get { return unchecked((int)_param.LongLength); }
        }

        public ParameterDescription GetParameter(int index)
        {
            return _param[index];
        }

        public override SignatureHelper ToSignatureHelper(ModuleBuilder module)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));

            SignatureHelper hlp;

            CallingConvention cnv = CallingConvention & (CallingConvention)0xf;

            //TODO: warning, SignatureHelper is missing return type modifiers!

            if (cnv == CallingConvention.Default ||
                cnv == CallingConvention.VarArg)
            {
                CallingConventions c = 0;

                if (cnv == CallingConvention.VarArg)
                    c = CallingConventions.VarArgs;
                if ((CallingConvention & CallingConvention.HasThis) == CallingConvention.HasThis)
                    c |= CallingConventions.HasThis;
                if ((CallingConvention & CallingConvention.ExplicitThis) == CallingConvention.ExplicitThis)
                    c |= CallingConventions.ExplicitThis;

                hlp = SignatureHelper.GetMethodSigHelper(c, ReturnType.ParameterType.ToType());
            }
            else
            {
                //InteropServices.CallingConvention c;
                //if (cnv == CallingConvention.C)
                //    c = InteropServices.CallingConvention.Cdecl;
                //else if (cnv == CallingConvention.FastCall)
                //    c = InteropServices.CallingConvention.FastCall;
                //else if (cnv == CallingConvention.StdCall)
                //    c = InteropServices.CallingConvention.StdCall;
                //else if (cnv == CallingConvention.ThisCall)
                //    c = InteropServices.CallingConvention.ThisCall;
                //else
                //    throw new ArgumentException("Invalid calling convention supplied.");

                // hlp = SignatureHelper.GetMethodSigHelper(c, ReturnType.ParameterType.ToType());

                throw new NotSupportedException("The creation of an unmanaged call signature is not supported by the underlying API.");
            }

            for (long i = 0; i < _param.LongLength; i++)
            {
                if (i == SentinelIndex)
                    hlp.AddSentinel();

                Type[] optModifiers;
                Type[] reqModifiers;

                ParameterDescription param = _param[i];

                if (param.CustomModifierCount == 0)
                    optModifiers = reqModifiers = null;
                else
                {
                    List<Type> o = null;
                    List<Type> r = null;

                    for (int j = 0; j < param.CustomModifierCount; j++)
                    {
                        CustomModifier mod = param.GetCustomModifier(j);
                        (mod.Required ? (r ?? (r = new List<Type>())) : (o ?? (o = new List<Type>()))).Add(Module.ResolveType(mod.MetadataToken));
                    }
                    optModifiers = o?.ToArray();
                    reqModifiers = r?.ToArray();
                }


                hlp.AddArgument(param.ParameterType.ToType(), reqModifiers, optModifiers);
            }

            return hlp;
        }
    }
}
