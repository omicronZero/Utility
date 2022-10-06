using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.Reflection;

namespace Utility.Dynamic.Disassembly
{
    public struct ILOperand
    {
        public OperandType OperandType { get; }

        private static readonly object NullModule = new object();

        private readonly object _referenceData;
        public long RawInt64 { get; }

        internal ILOperand(OperandType operandType, object referenceData)
        {
            if (referenceData == null)
                throw new ArgumentNullException(nameof(referenceData));

            OperandType = operandType;
            _referenceData = referenceData;
            RawInt64 = 0;
        }

        internal ILOperand(OperandType operandType, long data)
        {
            OperandType = operandType;
            RawInt64 = data;
            _referenceData = null;
        }

        internal ILOperand(OperandType operandType, long data, object referenceData)
        {
            OperandType = operandType;
            RawInt64 = data;
            _referenceData = referenceData;
        }


        public ulong RawUInt64
        {
            get { return unchecked((ulong)RawInt64); }
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

        public FieldInfo GetFieldMember()
        {
            return GetMember() as FieldInfo ?? throw new ArgumentException("Operand is not a field reference.");
        }

        public Type GetTypeMember()
        {
            return GetMember() as Type ?? throw new ArgumentException("Operand is not a type.");
        }

        public MethodBase GetMethodMember()
        {
            return GetMember() as MethodBase ?? throw new ArgumentException("Operand is not a method reference.");
        }

        public MemberInfo GetMember()
        {
            var member = _referenceData as MemberInfo;

            if (member == null)
            {
                if (_referenceData == NullModule)
                    throw new ArgumentException("Missing module to resolve member.");

                Module mdl = _referenceData as Module;

                if (mdl == null)
                    throw new InvalidOperationException("Operand is not a member reference.");

                return mdl.ResolveMember((int)RawInt64);
            }
            else
                return member;
        }

        public Module GetMemberModule()
        {
            if (OperandType != OperandType.InlineTok &&
                    OperandType != OperandType.InlineType &&
                    OperandType != OperandType.InlineField &&
                    OperandType != OperandType.InlineMethod)
                throw new InvalidOperationException("Operand is not a member reference.");

            if (_referenceData == NullModule)
                return null;

            var member = _referenceData as MemberInfo;

            if (member == null)
                return _referenceData as Module;
            else
                return member.Module;
        }

        public int GetMemberToken()
        {
            if (OperandType != OperandType.InlineTok &&
                    OperandType != OperandType.InlineType &&
                    OperandType != OperandType.InlineField &&
                    OperandType != OperandType.InlineMethod)
                throw new InvalidOperationException("Operand is not a member reference.");

            var member = _referenceData as MemberInfo;

            if (member == null)
            {
                return (int)RawInt64;
            }
            else
                return member.MetadataToken;
        }

        public byte[] GetSignature()
        {
            return GetSignatureCore().Value;
        }

        public Module GetSignatureModule()
        {
            return GetSignatureCore().Key;
        }

        private KeyValuePair<Module, byte[]> GetSignatureCore()
        {
            if (OperandType != OperandType.InlineSig)
                throw new InvalidOperationException("Operand is not a signature.");

            return (KeyValuePair<Module, byte[]>)_referenceData;
        }

        public int GetBranchTargetAddress()
        {
            if (OperandType != OperandType.InlineBrTarget && OperandType != OperandType.ShortInlineBrTarget)
                throw new InvalidOperationException("Operand is not a branch target index.");
            return GetBranchTarget().Address;
        }

        public ILLabel GetBranchTarget()
        {
            if (OperandType != OperandType.InlineBrTarget && OperandType != OperandType.ShortInlineBrTarget)
                throw new InvalidOperationException("Operand is not a branch target index.");
            return new ILLabel(unchecked((int)RawInt64), (ILabelProvider)_referenceData);
        }

        public string GetString()
        {
            return _referenceData as string ?? ((Module)_referenceData).ResolveString(GetStringToken());
        }

        public Module GetStringModule()
        {
            if (OperandType != OperandType.InlineString)
                throw new InvalidOperationException("Operand is not a string.");

            return _referenceData == NullModule ? null : _referenceData as Module;
        }

        public int GetStringToken()
        {
            if (OperandType != OperandType.InlineString)
                throw new InvalidOperationException("Operand is not a string.");

            if (GetStringModule() == null)
                throw new InvalidOperationException("Missing module to resolve string.");

            return unchecked((int)RawInt64);
        }

        public ushort GetLocalVariableIndex()
        {
            if (OperandType != OperandType.InlineVar && OperandType != OperandType.ShortInlineVar)
                throw new InvalidOperationException("Operand is not a variable index.");

            return unchecked((ushort)RawInt64);
        }

        public int ByteSize
        {
            get { return OpCodeHelper.SizeOf(OperandType); }
        }

        public unsafe double GetDouble()
        {
            if (OperandType != OperandType.InlineR)
                throw new InvalidOperationException("Operand is not a 64-bit IEEE floating point number.");

            long d = RawInt64;

            return *(double*)&d;
        }

        public unsafe float GetFloat()
        {
            if (OperandType != OperandType.ShortInlineR)
                throw new InvalidOperationException("Operand is not a 32-bit IEEE floating point number.");

            long d = RawInt64;

            return *(float*)&d;
        }

        public unsafe double GetReal()
        {
            long d = RawInt64;
            if (OperandType != OperandType.InlineR)
                return *(double*)&d;
            else if (OperandType != OperandType.ShortInlineR)
                return *(float*)&d;
            else
                throw new InvalidOperationException("Operand is not a 32-bit or 64-bit IEEE floating point number.");
        }

        public int GetSwitchToken()
        {
            if (OperandType != OperandType.InlineSwitch)
                throw new InvalidOperationException("Operand is not a switch token.");

            return unchecked((int)RawInt64);
        }

        public byte GetByte()
        {
            if (OperandType != OperandType.ShortInlineI)
                throw new InvalidOperationException("Operand is not a byte.");

            return unchecked((byte)RawInt64);
        }

        public int GetInt32()
        {
            if (OperandType != OperandType.ShortInlineI)
                throw new InvalidOperationException("Operand is not a 32-bit integer.");

            return unchecked((int)RawInt64);
        }

        public long GetInt64()
        {
            if (OperandType != OperandType.ShortInlineI)
                throw new InvalidOperationException("Operand is not a 64-bit integer.");
            return RawInt64;
        }

        public sbyte GetSByte()
        {
            if (OperandType != OperandType.ShortInlineI)
                throw new InvalidOperationException("Operand is not a byte and therefore cannot be regarded as a signed byte.");

            return unchecked((sbyte)RawInt64);
        }

        public uint GetUInt32()
        {
            if (OperandType != OperandType.ShortInlineI)
                throw new InvalidOperationException("Operand is not a 32-bit integer and therefore cannot be regarded as an unsigned 32-bit integer.");

            return unchecked((uint)RawInt64);
        }

        public ulong GetUInt64()
        {
            if (OperandType != OperandType.ShortInlineI)
                throw new InvalidOperationException("Operand is not a 64-bit integer and therefore cannot be regarded as an unsigned 64-bit integer.");

            return unchecked((ulong)RawInt64);
        }

        public byte GetValueByte()
        {
            if (_referenceData == null)
                throw new InvalidOperationException("Operand cannot be regarded as a byte in its current state.");
            if (ByteSize != 1)
                throw new InvalidOperationException("Operand canot be regarded as a byte.");

            return unchecked((byte)RawInt64);
        }

        public int GetValueInt32()
        {
            if (_referenceData == null)
                throw new InvalidOperationException("Operand cannot be regarded as a 32-bit integer in its current state.");
            if (ByteSize != 1)
                throw new InvalidOperationException("Operand canot be regarded as a 32-bit integer.");

            return unchecked((int)RawInt64);
        }

        public long GetValueInt64()
        {
            if (_referenceData == null)
                throw new InvalidOperationException("Operand cannot be regarded as a 64-bit integer in its current state.");
            if (ByteSize != 1)
                throw new InvalidOperationException("Operand canot be regarded as a 64-bit integer.");

            return RawInt64;
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

        #region static instantiation
        public static ILOperand BranchTarget(byte target)
        {
            return new ILOperand(OperandType.ShortInlineBrTarget, target);
        }

        public static ILOperand BranchTarget(int target)
        {
            return new ILOperand(OperandType.InlineBrTarget, target);
        }

        public static ILOperand BranchTarget(ILLabel target)
        {
            return new ILOperand(OperandType.InlineBrTarget, target.Raw, target.LabelProvider);
        }

        public static ILOperand Member(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            return new ILOperand(OperandType.InlineTok, method);
        }

        public static ILOperand Member(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return new ILOperand(OperandType.InlineTok, type);
        }

        public static ILOperand Member(FieldInfo field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            return new ILOperand(OperandType.InlineTok, field);
        }

        public static ILOperand Member(ConstructorInfo constructor)
        {
            if (constructor == null)
                throw new ArgumentNullException(nameof(constructor));

            return new ILOperand(OperandType.InlineTok, constructor);
        }

        public static ILOperand Member(Module module, int metadataToken)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));

            return new ILOperand(OperandType.InlineTok, metadataToken, module ?? NullModule);
        }

        public static ILOperand Method(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            return new ILOperand(OperandType.InlineMethod, method);
        }

        public static ILOperand Method(ConstructorInfo constructor)
        {
            if (constructor == null)
                throw new ArgumentNullException(nameof(constructor));

            return new ILOperand(OperandType.InlineMethod, constructor);
        }

        public static ILOperand Method(Module module, int metadataToken)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));

            return new ILOperand(OperandType.InlineMethod, metadataToken, module ?? NullModule);
        }

        public static ILOperand Field(FieldInfo field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            return new ILOperand(OperandType.InlineField, field);
        }

        public static ILOperand Field(Module module, int metadataToken)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));

            return new ILOperand(OperandType.InlineField, metadataToken, module ?? NullModule);
        }

        public static ILOperand Type(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return new ILOperand(OperandType.InlineType, type);
        }

        public static ILOperand Type(Module module, int metadataToken)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));

            return new ILOperand(OperandType.InlineType, metadataToken, module ?? NullModule);
        }

        public static ILOperand None()
        {
            return new ILOperand(OperandType.InlineNone, 0L);
        }

        public static ILOperand Constant(byte value)
        {
            return new ILOperand(OperandType.ShortInlineI, value);
        }

        public static ILOperand Constant(int value)
        {
            return new ILOperand(OperandType.InlineI, value);
        }

        public static ILOperand Constant(long value)
        {
            return new ILOperand(OperandType.InlineI8, value);
        }

        public static ILOperand Constant(float value)
        {
            return new ILOperand(OperandType.ShortInlineR, value);
        }

        public static ILOperand Constant(double value)
        {
            return new ILOperand(OperandType.InlineR, value);
        }

        public static ILOperand String(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return new ILOperand(OperandType.InlineString, value);
        }

        public static ILOperand String(Module module, int stringMetadataToken)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));

            return new ILOperand(OperandType.InlineString, stringMetadataToken, module ?? NullModule);
        }

        public static ILOperand Signature(Module signatureModule, int signatureMetadataToken)
        {
            if (signatureModule == null)
                throw new ArgumentNullException(nameof(signatureModule));

            return Signature(signatureModule, signatureModule.ResolveSignature(signatureMetadataToken));
        }

        public static ILOperand Signature(Module signatureModule, byte[] signatureBlob)
        {
            if (signatureModule == null)
                throw new ArgumentNullException(nameof(signatureModule));
            if (signatureBlob == null)
                throw new ArgumentNullException(nameof(signatureBlob));

            return new ILOperand(OperandType.InlineSig, new KeyValuePair<Module, byte[]>(signatureModule, signatureBlob));
        }

        public static ILOperand Switch(int token)
        {
            return new ILOperand(OperandType.InlineSwitch, token);
        }

        public static ILOperand Variable(byte variable)
        {
            return new ILOperand(OperandType.ShortInlineVar, variable);
        }

        public static ILOperand Variable(ushort variable)
        {
            return new ILOperand(OperandType.InlineVar, variable);
        }

        #endregion
    }
}
