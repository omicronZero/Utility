using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Dynamic.Disassembly
{
    public struct ILInstruction
    {
        public OpCode OpCode { get; }
        public ILOperand Operand { get; }

        public int ByteSize
        {
            get { return OpCode.Size + Operand.ByteSize; }
        }

        public void Normalize()
        {
            if (OpCode == OpCodes.Ldarg_0)
                this = new ILInstruction(OpCodes.Ldarg, new ILOperand(OperandType.InlineVar, 0));
            else if (OpCode == OpCodes.Ldarg_1)
                this = new ILInstruction(OpCodes.Ldarg, new ILOperand(OperandType.InlineVar, 1));
            else if (OpCode == OpCodes.Ldarg_2)
                this = new ILInstruction(OpCodes.Ldarg, new ILOperand(OperandType.InlineVar, 2));
            else if (OpCode == OpCodes.Ldarg_3)
                this = new ILInstruction(OpCodes.Ldarg, new ILOperand(OperandType.InlineVar, 3));
            else if (OpCode == OpCodes.Ldloc_0)
                this = new ILInstruction(OpCodes.Ldloc, new ILOperand(OperandType.InlineVar, 0));
            else if (OpCode == OpCodes.Ldloc_1)
                this = new ILInstruction(OpCodes.Ldloc, new ILOperand(OperandType.InlineVar, 1));
            else if (OpCode == OpCodes.Ldloc_2)
                this = new ILInstruction(OpCodes.Ldloc, new ILOperand(OperandType.InlineVar, 2));
            else if (OpCode == OpCodes.Ldloc_3)
                this = new ILInstruction(OpCodes.Ldloc, new ILOperand(OperandType.InlineVar, 3));
            else if (OpCode == OpCodes.Stloc_0)
                this = new ILInstruction(OpCodes.Stloc, new ILOperand(OperandType.InlineVar, 0));
            else if (OpCode == OpCodes.Stloc_1)
                this = new ILInstruction(OpCodes.Stloc, new ILOperand(OperandType.InlineVar, 1));
            else if (OpCode == OpCodes.Stloc_2)
                this = new ILInstruction(OpCodes.Stloc, new ILOperand(OperandType.InlineVar, 2));
            else if (OpCode == OpCodes.Stloc_3)
                this = new ILInstruction(OpCodes.Stloc, new ILOperand(OperandType.InlineVar, 3));
            else if (OpCode == OpCodes.Ldarg_S)
                this = new ILInstruction(OpCodes.Ldarg, new ILOperand(OperandType.InlineVar, Operand.RawInt64));
            else if (OpCode == OpCodes.Ldarga_S)
                this = new ILInstruction(OpCodes.Ldarga, new ILOperand(OperandType.InlineVar, Operand.RawInt64));
            else if (OpCode == OpCodes.Starg_S)
                this = new ILInstruction(OpCodes.Starg, new ILOperand(OperandType.InlineVar, Operand.RawInt64));
            else if (OpCode == OpCodes.Ldloc_S)
                this = new ILInstruction(OpCodes.Ldloc, new ILOperand(OperandType.InlineVar, Operand.RawInt64));
            else if (OpCode == OpCodes.Ldloca_S)
                this = new ILInstruction(OpCodes.Ldloca, new ILOperand(OperandType.InlineVar, Operand.RawInt64));
            else if (OpCode == OpCodes.Stloc_S)
                this = new ILInstruction(OpCodes.Stloc, new ILOperand(OperandType.InlineVar, Operand.RawInt64));
            else if (OpCode == OpCodes.Ldc_I4_M1)
                this = new ILInstruction(OpCodes.Ldc_I4, new ILOperand(OperandType.InlineI, -1));
            else if (OpCode == OpCodes.Ldc_I4_0)
                this = new ILInstruction(OpCodes.Ldc_I4, new ILOperand(OperandType.InlineI, 0));
            else if (OpCode == OpCodes.Ldc_I4_1)
                this = new ILInstruction(OpCodes.Ldc_I4, new ILOperand(OperandType.InlineI, 1));
            else if (OpCode == OpCodes.Ldc_I4_2)
                this = new ILInstruction(OpCodes.Ldc_I4, new ILOperand(OperandType.InlineI, 2));
            else if (OpCode == OpCodes.Ldc_I4_3)
                this = new ILInstruction(OpCodes.Ldc_I4, new ILOperand(OperandType.InlineI, 3));
            else if (OpCode == OpCodes.Ldc_I4_4)
                this = new ILInstruction(OpCodes.Ldc_I4, new ILOperand(OperandType.InlineI, 4));
            else if (OpCode == OpCodes.Ldc_I4_5)
                this = new ILInstruction(OpCodes.Ldc_I4, new ILOperand(OperandType.InlineI, 5));
            else if (OpCode == OpCodes.Ldc_I4_6)
                this = new ILInstruction(OpCodes.Ldc_I4, new ILOperand(OperandType.InlineI, 6));
            else if (OpCode == OpCodes.Ldc_I4_7)
                this = new ILInstruction(OpCodes.Ldc_I4, new ILOperand(OperandType.InlineI, 7));
            else if (OpCode == OpCodes.Ldc_I4_8)
                this = new ILInstruction(OpCodes.Ldc_I4, new ILOperand(OperandType.InlineI, 8));
            else if (OpCode == OpCodes.Ldc_I4_S)
                this = new ILInstruction(OpCodes.Ldc_I4, new ILOperand(OperandType.InlineI, Operand.RawInt64));
            else if (OpCode == OpCodes.Br_S)
                this = new ILInstruction(OpCodes.Br, new ILOperand(OperandType.InlineBrTarget, Operand.RawInt64));
            else if (OpCode == OpCodes.Brfalse_S)
                this = new ILInstruction(OpCodes.Brfalse, new ILOperand(OperandType.InlineBrTarget, Operand.RawInt64));
            else if (OpCode == OpCodes.Brtrue_S)
                this = new ILInstruction(OpCodes.Brtrue, new ILOperand(OperandType.InlineBrTarget, Operand.RawInt64));
            else if (OpCode == OpCodes.Beq_S)
                this = new ILInstruction(OpCodes.Beq, new ILOperand(OperandType.InlineBrTarget, Operand.RawInt64));
            else if (OpCode == OpCodes.Bge_S)
                this = new ILInstruction(OpCodes.Bge, new ILOperand(OperandType.InlineBrTarget, Operand.RawInt64));
            else if (OpCode == OpCodes.Bgt_S)
                this = new ILInstruction(OpCodes.Bgt, new ILOperand(OperandType.InlineBrTarget, Operand.RawInt64));
            else if (OpCode == OpCodes.Ble_S)
                this = new ILInstruction(OpCodes.Ble, new ILOperand(OperandType.InlineBrTarget, Operand.RawInt64));
            else if (OpCode == OpCodes.Blt_S)
                this = new ILInstruction(OpCodes.Blt, new ILOperand(OperandType.InlineBrTarget, Operand.RawInt64));
            else if (OpCode == OpCodes.Bne_Un_S)
                this = new ILInstruction(OpCodes.Bne_Un, new ILOperand(OperandType.InlineBrTarget, Operand.RawInt64));
            else if (OpCode == OpCodes.Bge_Un_S)
                this = new ILInstruction(OpCodes.Bge_Un, new ILOperand(OperandType.InlineBrTarget, Operand.RawInt64));
            else if (OpCode == OpCodes.Bgt_Un_S)
                this = new ILInstruction(OpCodes.Bgt_Un, new ILOperand(OperandType.InlineBrTarget, Operand.RawInt64));
            else if (OpCode == OpCodes.Ble_Un_S)
                this = new ILInstruction(OpCodes.Ble_Un, new ILOperand(OperandType.InlineBrTarget, Operand.RawInt64));
            else if (OpCode == OpCodes.Blt_Un_S)
                this = new ILInstruction(OpCodes.Blt_Un, new ILOperand(OperandType.InlineBrTarget, Operand.RawInt64));
        }

        public void Compress()
        {
            if (OpCode == OpCodes.Ldarg)
            {
                if (Operand.RawUInt64 == 0)
                    this = new ILInstruction(OpCodes.Ldarg_0, new ILOperand(OperandType.InlineNone, 0));
                else if (Operand.RawUInt64 == 1)
                    this = new ILInstruction(OpCodes.Ldarg_1, new ILOperand(OperandType.InlineNone, 0));
                else if (Operand.RawUInt64 == 2)
                    this = new ILInstruction(OpCodes.Ldarg_2, new ILOperand(OperandType.InlineNone, 0));
                else if (Operand.RawUInt64 == 3)
                    this = new ILInstruction(OpCodes.Ldarg_3, new ILOperand(OperandType.InlineNone, 0));
                else if (Operand.RawUInt64 <= 255)
                    this = new ILInstruction(OpCodes.Ldarg_S, new ILOperand(OperandType.ShortInlineVar, Operand.RawInt64));
            }
            else if (OpCode == OpCodes.Ldloc)
            {
                if (Operand.RawUInt64 == 0)
                    this = new ILInstruction(OpCodes.Ldloc_0, new ILOperand(OperandType.InlineNone, 0));
                else if (Operand.RawUInt64 == 1)
                    this = new ILInstruction(OpCodes.Ldloc_1, new ILOperand(OperandType.InlineNone, 0));
                else if (Operand.RawUInt64 == 2)
                    this = new ILInstruction(OpCodes.Ldloc_2, new ILOperand(OperandType.InlineNone, 0));
                else if (Operand.RawUInt64 == 3)
                    this = new ILInstruction(OpCodes.Ldloc_3, new ILOperand(OperandType.InlineNone, 0));
                else if (Operand.RawUInt64 <= 255)
                    this = new ILInstruction(OpCodes.Ldloc_S, new ILOperand(OperandType.ShortInlineVar, Operand.RawInt64));
            }
            else if (OpCode == OpCodes.Stloc)
            {
                if (Operand.RawUInt64 == 0)
                    this = new ILInstruction(OpCodes.Stloc_0, new ILOperand(OperandType.InlineNone, 0));
                else if (Operand.RawUInt64 == 1)
                    this = new ILInstruction(OpCodes.Stloc_1, new ILOperand(OperandType.InlineNone, 0));
                else if (Operand.RawUInt64 == 2)
                    this = new ILInstruction(OpCodes.Stloc_2, new ILOperand(OperandType.InlineNone, 0));
                else if (Operand.RawUInt64 == 3)
                    this = new ILInstruction(OpCodes.Stloc_3, new ILOperand(OperandType.InlineNone, 0));
                else if (Operand.RawUInt64 <= 255)
                    this = new ILInstruction(OpCodes.Stloc_S, new ILOperand(OperandType.ShortInlineVar, Operand.RawInt64));
            }
            else if (OpCode == OpCodes.Ldarga)
            {
                if (Operand.RawUInt64 <= 255)
                    this = new ILInstruction(OpCodes.Ldarga_S, new ILOperand(OperandType.ShortInlineVar, Operand.RawInt64));
            }
            else if (OpCode == OpCodes.Starg)
            {
                if (Operand.RawUInt64 <= 255)
                    this = new ILInstruction(OpCodes.Starg_S, new ILOperand(OperandType.ShortInlineVar, Operand.RawInt64));
            }
            else if (OpCode == OpCodes.Ldloca)
            {
                if (Operand.RawUInt64 <= 255)
                    this = new ILInstruction(OpCodes.Ldloca_S, new ILOperand(OperandType.ShortInlineVar, Operand.RawInt64));
            }
            else if (OpCode == OpCodes.Ldc_I4)
            {
                if (Operand.GetInt32() == -1)
                    this = new ILInstruction(OpCodes.Ldc_I4_M1, new ILOperand(OperandType.InlineNone, 0));
                else if (Operand.RawUInt64 == 0)
                    this = new ILInstruction(OpCodes.Ldc_I4_0, new ILOperand(OperandType.InlineNone, 0));
                else if (Operand.RawUInt64 == 1)
                    this = new ILInstruction(OpCodes.Ldc_I4_1, new ILOperand(OperandType.InlineNone, 0));
                else if (Operand.RawUInt64 == 2)
                    this = new ILInstruction(OpCodes.Ldc_I4_2, new ILOperand(OperandType.InlineNone, 0));
                else if (Operand.RawUInt64 == 3)
                    this = new ILInstruction(OpCodes.Ldc_I4_3, new ILOperand(OperandType.InlineNone, 0));
                else if (Operand.RawUInt64 == 4)
                    this = new ILInstruction(OpCodes.Ldc_I4_4, new ILOperand(OperandType.InlineNone, 0));
                else if (Operand.RawUInt64 == 5)
                    this = new ILInstruction(OpCodes.Ldc_I4_5, new ILOperand(OperandType.InlineNone, 0));
                else if (Operand.RawUInt64 == 6)
                    this = new ILInstruction(OpCodes.Ldc_I4_6, new ILOperand(OperandType.InlineNone, 0));
                else if (Operand.RawUInt64 == 7)
                    this = new ILInstruction(OpCodes.Ldc_I4_7, new ILOperand(OperandType.InlineNone, 0));
                else if (Operand.RawUInt64 == 8)
                    this = new ILInstruction(OpCodes.Ldc_I4_8, new ILOperand(OperandType.InlineNone, 0));
                else if (Operand.RawUInt64 <= 255)
                    this = new ILInstruction(OpCodes.Ldc_I4_S, new ILOperand(OperandType.ShortInlineI, Operand.RawInt64));
            }
            else if (OpCode == OpCodes.Br)
            {
                if (!Operand.GetBranchTarget().IsIndex && IsInRange(Operand.GetBranchTargetAddress(), -128, 127))
                    this = new ILInstruction(OpCodes.Br_S, new ILOperand(OperandType.ShortInlineBrTarget, Operand.RawInt64));
            }
            else if (OpCode == OpCodes.Brfalse)
            {
                if (!Operand.GetBranchTarget().IsIndex && IsInRange(Operand.GetBranchTargetAddress(), -128, 127))
                    this = new ILInstruction(OpCodes.Brfalse_S, new ILOperand(OperandType.ShortInlineBrTarget, Operand.RawInt64));
            }
            else if (OpCode == OpCodes.Brtrue)
            {
                if (!Operand.GetBranchTarget().IsIndex && IsInRange(Operand.GetBranchTargetAddress(), -128, 127))
                    this = new ILInstruction(OpCodes.Brtrue_S, new ILOperand(OperandType.ShortInlineBrTarget, Operand.RawInt64));
            }
            else if (OpCode == OpCodes.Beq)
            {
                if (!Operand.GetBranchTarget().IsIndex && IsInRange(Operand.GetBranchTargetAddress(), -128, 127))
                    this = new ILInstruction(OpCodes.Beq_S, new ILOperand(OperandType.ShortInlineBrTarget, Operand.RawInt64));
            }
            else if (OpCode == OpCodes.Bge)
            {
                if (!Operand.GetBranchTarget().IsIndex && IsInRange(Operand.GetBranchTargetAddress(), -128, 127))
                    this = new ILInstruction(OpCodes.Bge_S, new ILOperand(OperandType.ShortInlineBrTarget, Operand.RawInt64));
            }
            else if (OpCode == OpCodes.Bgt)
            {
                if (!Operand.GetBranchTarget().IsIndex && IsInRange(Operand.GetBranchTargetAddress(), -128, 127))
                    this = new ILInstruction(OpCodes.Bgt_S, new ILOperand(OperandType.ShortInlineBrTarget, Operand.RawInt64));
            }
            else if (OpCode == OpCodes.Ble)
            {
                if (!Operand.GetBranchTarget().IsIndex && IsInRange(Operand.GetBranchTargetAddress(), -128, 127))
                    this = new ILInstruction(OpCodes.Ble_S, new ILOperand(OperandType.ShortInlineBrTarget, Operand.RawInt64));
            }
            else if (OpCode == OpCodes.Blt)
            {
                if (!Operand.GetBranchTarget().IsIndex && IsInRange(Operand.GetBranchTargetAddress(), -128, 127))
                    this = new ILInstruction(OpCodes.Blt_S, new ILOperand(OperandType.ShortInlineBrTarget, Operand.RawInt64));
            }
            else if (OpCode == OpCodes.Bne_Un)
            {
                if (!Operand.GetBranchTarget().IsIndex && IsInRange(Operand.GetBranchTargetAddress(), -128, 127))
                    this = new ILInstruction(OpCodes.Bne_Un_S, new ILOperand(OperandType.ShortInlineBrTarget, Operand.RawInt64));
            }
            else if (OpCode == OpCodes.Bge_Un)
            {
                if (!Operand.GetBranchTarget().IsIndex && IsInRange(Operand.GetBranchTargetAddress(), -128, 127))
                    this = new ILInstruction(OpCodes.Bge_Un_S, new ILOperand(OperandType.ShortInlineBrTarget, Operand.RawInt64));
            }
            else if (OpCode == OpCodes.Bgt_Un)
            {
                if (!Operand.GetBranchTarget().IsIndex && IsInRange(Operand.GetBranchTargetAddress(), -128, 127))
                    this = new ILInstruction(OpCodes.Bgt_Un_S, new ILOperand(OperandType.ShortInlineBrTarget, Operand.RawInt64));
            }
            else if (OpCode == OpCodes.Ble_Un)
            {
                if (!Operand.GetBranchTarget().IsIndex && IsInRange(Operand.GetBranchTargetAddress(), -128, 127))
                    this = new ILInstruction(OpCodes.Ble_Un_S, new ILOperand(OperandType.ShortInlineBrTarget, Operand.RawInt64));
            }
            else if (OpCode == OpCodes.Blt_Un)
            {
                if (!Operand.GetBranchTarget().IsIndex && IsInRange(Operand.GetBranchTargetAddress(), -128, 127))
                    this = new ILInstruction(OpCodes.Blt_Un_S, new ILOperand(OperandType.ShortInlineBrTarget, Operand.RawInt64));
            }
        }

        private bool IsInRange(int value, int minimum, int maximum)
        {
            return minimum <= value && value <= maximum;
        }

        public ILInstruction(OpCode opCode, ILOperand operand)
        {
            if (opCode.OperandType != operand.OperandType)
                throw new ArgumentException("Operand is not a valid value for the specified operation code.");

            OpCode = opCode;
            Operand = operand;
        }
    }
}
