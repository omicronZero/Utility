using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Dynamic.Disassembly
{
    internal static class OpCodeHelper
    {
        private static readonly Dictionary<short, OpCode> _opCodes;

        public static OpCode GetOpCode(short opCode)
        {
            OpCode opc;
            if (!_opCodes.TryGetValue(opCode, out opc))
                throw new ArgumentException("Unsupported op-code detected.", nameof(OpCode));

            return opc;
        }

        public static bool TryGetOpCode(short opCode, out OpCode mappedOpCode)
        {
            return _opCodes.TryGetValue(opCode, out mappedOpCode);
        }

        public static int SizeOf(OperandType operandType)
        {
            if (operandType == OperandType.InlineVar)
                return 2;
            else if (operandType == OperandType.InlineR ||
                    operandType == OperandType.InlineI8)
                return 8;
            else if (operandType == OperandType.ShortInlineI ||
                    operandType == OperandType.ShortInlineVar ||
                    operandType == OperandType.ShortInlineBrTarget)
                return 1;
            else if (operandType == OperandType.InlineBrTarget ||
                    operandType == OperandType.InlineField ||
                    operandType == OperandType.InlineI ||
                    operandType == OperandType.InlineMethod ||
                    operandType == OperandType.InlineSig ||
                    operandType == OperandType.InlineString ||
                    operandType == OperandType.InlineSwitch ||
                    operandType == OperandType.InlineTok ||
                    operandType == OperandType.InlineType ||
                    operandType == OperandType.ShortInlineR)
                return 4;
            else
                return 0;
        }

        internal static CallingConvention MapCallingConventions(CallingConventions callingConventions)
        {
            CallingConvention c = 0;

            if ((callingConventions & CallingConventions.ExplicitThis) == CallingConventions.ExplicitThis)
                c |= CallingConvention.ExplicitThis;

            if ((callingConventions & CallingConventions.HasThis) == CallingConventions.HasThis)
                c |= CallingConvention.HasThis;

            callingConventions &= ~(CallingConventions.HasThis | CallingConventions.ExplicitThis);

            if (callingConventions == CallingConventions.Standard)
                c = CallingConvention.Default;
            else if (callingConventions == CallingConventions.VarArgs)
                c = CallingConvention.VarArg;
            else if (callingConventions != 0)
                throw new ArgumentException("Unsupported calling convention. Either HasThis, ExplicitThis and HasThis, Standard or VarArgs may be supplied.", nameof(callingConventions));

            return c;
        }

        internal static CallingConvention MapCallingConventions(System.Runtime.InteropServices.CallingConvention callingConventions)
        {
            CallingConvention c = 0;

            if (callingConventions == System.Runtime.InteropServices.CallingConvention.Cdecl)
                c = CallingConvention.C;
            else if (callingConventions == System.Runtime.InteropServices.CallingConvention.FastCall)
                c = CallingConvention.FastCall;
            else if (callingConventions == System.Runtime.InteropServices.CallingConvention.StdCall)
                c = CallingConvention.StdCall;
            else if (callingConventions == System.Runtime.InteropServices.CallingConvention.ThisCall)
                c = CallingConvention.ThisCall;
            else if (callingConventions == System.Runtime.InteropServices.CallingConvention.Winapi)
                if (Environment.OSVersion.Platform == PlatformID.WinCE)
                    return CallingConvention.C;
                else
                    return CallingConvention.StdCall;
            else
                throw new ArgumentException("Unsupported calling convention.", nameof(callingConventions));

            return c;
        }

        static OpCodeHelper()
        {
            _opCodes = new Dictionary<short, OpCode>();

            foreach (FieldInfo opcfield in typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
                if (opcfield.FieldType == typeof(OpCode))
                {
                    var opc = (OpCode)opcfield.GetValue(null);

                    _opCodes.Add(opc.Value, opc);
                }
        }

        internal static bool IsPrefix(byte opc)
        {
            return GetOpCode(opc).OpCodeType == OpCodeType.Prefix;
        }
    }
}
