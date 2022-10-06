using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.Reflection;

namespace Utility.Dynamic.Disassembly
{
    public unsafe struct ILDisassemblyOperand
    {
        private ILDisassembly _disassembly;
        private readonly byte* _ilDataPointer;
        public OperandType OperandType { get; }

        internal ILDisassemblyOperand(ILDisassembly disassembly, byte* ilDataPointer, OperandType operandType)
        {
            _disassembly = disassembly;
            _ilDataPointer = ilDataPointer;
            OperandType = operandType;
        }

        public bool IsBranch
        {
            get { return OperandType == OperandType.InlineBrTarget || OperandType == OperandType.ShortInlineBrTarget; }
        }

        public bool IsMember
        {
            get
            {
                return OperandType == OperandType.InlineTok ||
                 OperandType == OperandType.InlineType ||
                 OperandType == OperandType.InlineField ||
                 OperandType == OperandType.InlineMethod;
            }
        }

        public bool IsSignature
        {
            get { return OperandType == OperandType.InlineSig; }
        }

        public bool IsInt32
        {
            get { return OperandType == OperandType.InlineI; }
        }

        public bool IsInt64
        {
            get { return OperandType == OperandType.InlineI8; }
        }

        public bool IsInt8
        {
            get { return OperandType == OperandType.ShortInlineI; }
        }

        public bool IsNone
        {
            get { return OperandType == OperandType.InlineNone; }
        }

        public bool IsDouble
        {
            get { return OperandType == OperandType.InlineR; }
        }

        public bool IsFloat
        {
            get { return OperandType == OperandType.ShortInlineR; }
        }

        public bool IsString
        {
            get { return OperandType == OperandType.InlineString; }
        }

        public bool IsSwitch
        {
            get { return OperandType == OperandType.InlineSwitch; }
        }

        public bool IsVariable
        {
            get { return OperandType == OperandType.InlineVar || OperandType == OperandType.ShortInlineVar; }
        }

        private byte* GetDataPointer()
        {
            if (_disassembly.IsDisposed)
                throw new ObjectDisposedException(typeof(ILDisassembly).Name);

            return _ilDataPointer;
        }

        public long RawInt64
        {
            get
            {
                byte* ptr = GetDataPointer();
                long cv = 0;

                for (int i = 0; i < ByteSize; i++)
                    cv |= ptr[i];

                return cv;
            }
        }

        public ulong RawUInt64
        {
            get { return unchecked((ulong)RawInt64); }
        }

        public int GetMemberToken()
        {
            if (OperandType == OperandType.InlineField
                    || OperandType == OperandType.InlineMethod
                    || OperandType == OperandType.InlineType
                    || OperandType == OperandType.InlineTok)
                return *(int*)GetDataPointer();
            else
                throw new InvalidOperationException("Operand is not a member token.");
        }

        public MemberInfo GetMember()
        {
            if (_disassembly.Module == null)
                throw new InvalidOperationException("Cannot resolve member without a module.");

            if (OperandType == OperandType.InlineField)
                return _disassembly.Module.ResolveField(*(int*)GetDataPointer());
            else if (OperandType == OperandType.InlineMethod)
                return _disassembly.Module.ResolveMethod(*(int*)GetDataPointer());
            else if (OperandType == OperandType.InlineType)
                return _disassembly.Module.ResolveType(*(int*)GetDataPointer());
            else if (OperandType == OperandType.InlineTok)
                return _disassembly.Module.ResolveMember(*(int*)GetDataPointer());
            else
                throw new InvalidOperationException("Operand is not a member token.");
        }

        public int GetSignatureToken()
        {
            if (OperandType == OperandType.InlineSig)
                return *(int*)GetDataPointer();
            else
                throw new InvalidOperationException("Operand is not a signature token.");
        }

        public byte[] GetSignature()
        {
            if (_disassembly.Module == null)
                throw new InvalidOperationException("Cannot resolve signature without a module.");

            return _disassembly.Module.ResolveSignature(GetSignatureToken());
        }

        public int GetBranchTarget()
        {
            if (OperandType == OperandType.InlineBrTarget)
                return *(int*)GetDataPointer();
            else if (OperandType == OperandType.ShortInlineBrTarget)
                return *GetDataPointer();
            else
                throw new InvalidOperationException("Operand is not a branch target.");
        }

        public string GetString()
        {
            if (_disassembly.Module == null)
                throw new InvalidOperationException("Cannot resolve string without a module.");

            return _disassembly.Module.ResolveString(GetStringToken());
        }

        public int GetStringToken()
        {
            if (OperandType == OperandType.InlineString)
                return *(int*)GetDataPointer();
            else
                throw new InvalidOperationException("Operand is not a string token.");
        }

        public short GetLocalVariableIndex()
        {
            if (OperandType == OperandType.InlineVar)
                return *(short*)GetDataPointer();
            else if (OperandType == OperandType.ShortInlineVar)
                return *GetDataPointer();
            else
                throw new InvalidOperationException("Operand is not a local variable.");
        }

        public int ByteSize
        {
            get { return OpCodeHelper.SizeOf(OperandType); }
        }

        public double GetDouble()
        {
            if (OperandType == OperandType.InlineR)
                return *(double*)GetDataPointer();
            else
                throw new ArgumentException("Operand is not a 64-Bit IEEE floating point number.");
        }

        public float GetFloat()
        {
            if (OperandType == OperandType.ShortInlineR)
                return *(float*)GetDataPointer();
            else
                throw new ArgumentException("Operand is not a 32-Bit IEEE floating point number.");
        }

        public double GetReal()
        {
            if (OperandType == OperandType.InlineR)
                return *(double*)GetDataPointer();
            else if (OperandType == OperandType.ShortInlineR)
                return *(float*)GetDataPointer();
            else
                throw new ArgumentException("Operand is not a 64-Bit or 32-Bit IEEE floating point number.");
        }

        public int GetSwitchToken()
        {
            if (OperandType == OperandType.InlineSwitch)
                return *(int*)GetDataPointer();
            else
                throw new ArgumentException("Operand is not a switch instruction.");
        }

        public byte GetByte()
        {
            if (OperandType == OperandType.ShortInlineI)
                return *GetDataPointer();
            else
                throw new ArgumentException("Operand is not an 8-bit integer.");
        }

        public int GetInt32()
        {
            if (OperandType == OperandType.InlineI)
                return *(int*)GetDataPointer();
            else
                throw new ArgumentException("Operand is not a 32-bit integer.");
        }

        public long GetInt64()
        {
            if (OperandType == OperandType.InlineI8)
                return *(long*)GetDataPointer();
            else
                throw new ArgumentException("Operand is not a 64-bit integer.");
        }

        public sbyte GetSByte()
        {
            return unchecked((sbyte)GetByte());
        }

        public uint GetUInt32()
        {
            return unchecked((uint)GetInt32());
        }

        public ulong GetUInt64()
        {
            return unchecked((ulong)GetInt64());
        }

        public int GetValueInt32()
        {
            if (ByteSize == 4)
                return *(int*)GetDataPointer();
            else
                throw new ArgumentException("Operand is not a 32-bit integer.");
        }

        public long GetValueInt64()
        {
            if (ByteSize == 8)
                return *(long*)GetDataPointer();
            else
                throw new ArgumentException("Operand is not a 64-bit integer.");
        }

        public byte GetValueByte()
        {
            if (ByteSize == 1)
                return *GetDataPointer();
            else
                throw new ArgumentException("Operand is not a byte.");
        }

        public sbyte GetValueSByte()
        {
            return unchecked((sbyte)GetValueByte());
        }

        public uint GetValueUInt32()
        {
            return unchecked((uint)GetValueInt32());
        }

        public ulong GetValueUInt64()
        {
            return unchecked((ulong)GetValueInt64());
        }

        public static explicit operator ILOperand(ILDisassemblyOperand value)
        {
            object rv = null;
            long v = 0;

            if (value.OperandType == OperandType.InlineField ||
                    value.OperandType == OperandType.InlineMethod ||
                    value.OperandType == OperandType.InlineType ||
                    value.OperandType == OperandType.InlineTok)
            {
                rv = value._disassembly.Module;
                v = value.GetMemberToken();
            }
            else if (value.OperandType == OperandType.InlineString)
                rv = value.GetString();
            else if (value.OperandType == OperandType.InlineSig)
                rv = new KeyValuePair<Module, byte[]>(value._disassembly.Module, value.GetSignature());
            else
                v = value.RawInt64;

            return new ILOperand(value.OperandType, v, rv);
        }
    }
}
