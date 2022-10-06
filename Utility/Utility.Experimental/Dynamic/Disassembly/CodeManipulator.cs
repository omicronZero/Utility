using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Dynamic.Disassembly
{
    public static class CodeManipulator
    {
        public static MethodBuilder ReplaceInstructions(MethodInfo method, Func<ILInstruction, int, ILInstruction> instructionReplacer)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            if (instructionReplacer == null)
                throw new ArgumentNullException(nameof(instructionReplacer));

            return ReplaceInstructions(DynamicMethod.NextType(), method, instructionReplacer);
        }

        public static MethodBuilder ReplaceInstructions(TypeBuilder targetType, MethodInfo method, Func<ILInstruction, int, ILInstruction> instructionReplacer)
        {
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            if (instructionReplacer == null)
                throw new ArgumentNullException(nameof(instructionReplacer));

            Type[] param;
            if ((method.CallingConvention & CallingConventions.HasThis) == CallingConventions.HasThis)
            {
                ParameterInfo[] p = method.GetParameters();
                param = new Type[p.Length + 1];
                param[0] = method.DeclaringType;
                for (int i = 0; i < p.Length; i++)
                    param[i + 1] = p[i].ParameterType;
            }
            else
                param = Array.ConvertAll(method.GetParameters(), (m) => m.ParameterType);

            return ReplaceInstructions(targetType.DefineMethod(method.Name,
                method.Attributes,
                CallingConventions.Standard,
                method.ReturnType,
                param),
                method,
                instructionReplacer);
        }

        public static MethodBuilder ReplaceInstructions(MethodBuilder targetMethod, MethodInfo method, Func<ILInstruction, int, ILInstruction> instructionReplacer)
        {
            if (targetMethod == null)
                throw new ArgumentNullException(nameof(targetMethod));
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            if (instructionReplacer == null)
                throw new ArgumentNullException(nameof(instructionReplacer));

            Module module = Assembly.GetCallingAssembly().ManifestModule;

            ReplaceInstructions(method, (ModuleBuilder)targetMethod.Module, targetMethod.GetILGenerator(), instructionReplacer);

            return targetMethod;
        }

        public static void ReplaceInstructions(MethodInfo method, ModuleBuilder targetModule, ILGenerator targetGenerator, Func<ILInstruction, int, ILInstruction> instructionReplacer)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            if (targetModule == null)
                throw new ArgumentNullException(nameof(targetModule));
            if (targetGenerator == null)
                throw new ArgumentNullException(nameof(targetGenerator));
            if (instructionReplacer == null)
                throw new ArgumentNullException(nameof(instructionReplacer));

            using (var ild = new ILDisassembly(method))
            {
                int instructionIndex = 0;
                while (ild.MoveNext())
                {
                    EmitOperation(method.Module, targetModule, targetGenerator, instructionReplacer.Invoke(ild.CurrentInstruction, instructionIndex), instructionIndex);

                    instructionIndex++;
                }
            }
        }

        internal static void EmitOperation(Module sourceModule, ModuleBuilder targetModule, ILGenerator targetGenerator, ILInstruction instruction, int index)
        {
            OperandType operandType = instruction.OpCode.OperandType;

            if (operandType == OperandType.InlineBrTarget)
                targetGenerator.Emit(instruction.OpCode, GetBranchTarget(instruction.Operand.GetBranchTarget(), index));
            else if (operandType == OperandType.InlineI)
                targetGenerator.Emit(instruction.OpCode, instruction.Operand.GetInt32());
            else if (operandType == OperandType.InlineI8)
                targetGenerator.Emit(instruction.OpCode, instruction.Operand.GetInt64());
            else if (operandType == OperandType.InlineR)
                targetGenerator.Emit(instruction.OpCode, instruction.Operand.GetDouble());
            else if (operandType == OperandType.InlineSig)
                targetGenerator.Emit(instruction.OpCode, SignatureDecoder.DecodeSignature(sourceModule, instruction.Operand.GetSignature()).ToSignatureHelper(targetModule));
            else if (operandType == OperandType.InlineString)
                targetGenerator.Emit(instruction.OpCode, instruction.Operand.GetString());
            else if (operandType == OperandType.InlineSwitch)
                targetGenerator.Emit(instruction.OpCode, instruction.Operand.GetSwitchToken());
            else if (operandType == OperandType.InlineTok || operandType == OperandType.InlineMethod || operandType == OperandType.InlineType || operandType == OperandType.InlineField)
            {
                MemberInfo mem = instruction.Operand.GetMember();
                if (mem is Type)
                    targetGenerator.Emit(instruction.OpCode, (Type)mem);
                else if (mem is MethodInfo)
                    targetGenerator.Emit(instruction.OpCode, (MethodInfo)mem);
                else if (mem is FieldInfo)
                    targetGenerator.Emit(instruction.OpCode, (FieldInfo)mem);
                else if (mem is ConstructorInfo)
                    targetGenerator.Emit(instruction.OpCode, (ConstructorInfo)mem);
                else
                    throw new InvalidOperationException("Unsupported member type dectected while emitting op-code with token operand.");
            }
            else if (operandType == OperandType.InlineVar)
                targetGenerator.Emit(instruction.OpCode, instruction.Operand.GetLocalVariableIndex());
            else if (operandType == OperandType.ShortInlineBrTarget)
                targetGenerator.Emit(instruction.OpCode, (byte)GetBranchTarget(instruction.Operand.GetBranchTarget(), index));
            else if (operandType == OperandType.ShortInlineI)
                targetGenerator.Emit(instruction.OpCode, (byte)instruction.Operand.GetInt32());
            else if (operandType == OperandType.ShortInlineR)
                targetGenerator.Emit(instruction.OpCode, instruction.Operand.GetDouble());
            else if (operandType == OperandType.ShortInlineVar)
                targetGenerator.Emit(instruction.OpCode, (byte)instruction.Operand.GetLocalVariableIndex());
            else //Phi or None
                targetGenerator.Emit(instruction.OpCode);
        }

        private static int GetBranchTarget(ILLabel label, int currentInstructionIndex)
        {
            return label.IsIndex ? label.Index - currentInstructionIndex : label.Address;
        }

        private class TypeKey { }
    }
}
