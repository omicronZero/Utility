using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Utility;
using System.Collections.ObjectModel;
using Utility.Collections.Labeled;

namespace Utility.Dynamic.Disassembly
{
    public class MethodManipulator : ILabelProvider
    {
        public Module SourceModule { get; }
        public Type InstanceType { get; }
        public IList<Type> Parameters { get; }
        public LabeledCollection<ILInstruction> Instructions { get; }
        public Type ReturnType { get; set; }
        public CallingConvention CallingConvention { get; }
        public IList<Type> LocalVariables { get; }
        public IList<Type> MethodGenericArguments { get; }

        private Dictionary<int, Signature> _signatureMap;

        private readonly List<Type> _parameters;
        private readonly List<Type> _localVariables;
        private readonly List<int> _labels;

        public MethodManipulator(Module sourceModule, CallingConventions callingConventions, Type instanceType, Type[] parameters, Type returnType, Type[] localVariables, ILInstruction[] instructions, Type[] methodGenericArguments)
            : this(sourceModule, OpCodeHelper.MapCallingConventions(callingConventions), instanceType, parameters, returnType, localVariables, instructions, methodGenericArguments)
        { }

        public MethodManipulator(Module sourceModule, CallingConvention callingConvention, Type instanceType, Type[] parameters, Type returnType, Type[] localVariables, ILInstruction[] instructions, Type[] methodGenericArguments)
        {
            if (sourceModule == null)
                throw new ArgumentNullException(nameof(sourceModule));

            if (parameters != null)
                foreach (Type p in parameters)
                    if (p == null)
                        throw new ArgumentException("At least one of the parameter types supplied was null.", nameof(parameters));

            if (localVariables != null)
                foreach (Type p in localVariables)
                    if (p == null)
                        throw new ArgumentException("At least one of the local variable types supplied was null.", nameof(localVariables));

            if (methodGenericArguments == null)
                methodGenericArguments = Array.Empty<Type>();
            else
            {
                foreach (Type tp in methodGenericArguments)
                    if (tp == null)
                        throw new ArgumentException("Generic arguments of method must not be null.", nameof(methodGenericArguments));
                methodGenericArguments = (Type[])methodGenericArguments.Clone();
            }

            SourceModule = sourceModule;
            InstanceType = instanceType;
            MethodGenericArguments = Array.AsReadOnly(methodGenericArguments);

            ObservableCollection<ILInstruction> instr;

            instr = new ObservableCollection<ILInstruction>();

            if (instructions != null)
            {
                List<(int, uint)> branchInstructions = new List<(int, uint)>();
                uint byteOffset = 0;

                for (int i = 0; i < instructions.Length; i++)
                {
                    byteOffset += (uint)instructions[i].ByteSize;
                    ILInstruction cinstr = instructions[i];

                    if (cinstr.Operand.IsBranch)
                    {
                        //register branch instruction at index i to absolute branch address indicated in ctr.Operand in list
                        //absolute branch address is computed by: branchAddress + currentInstructionSize + instructionByteOffset
                        //hence byteOffset is increased before the operation in this loop (in the second pass byteOffset is increased AFTER each iteration)
                        branchInstructions.Add((i, (uint)((long)cinstr.Operand.GetBranchTargetAddress() + byteOffset)));
                        //add placeholder that can be replaced in a second pass by processing list branchInstructions
                        instr.Add(default(ILInstruction));
                    }
                    else
                        instr.Add(cinstr);
                }

                byteOffset = 0;

                //sort branch instructions descending by their absolute byte address
                branchInstructions.Sort((l, r) => r.Item2.CompareTo(l.Item2));

                //until there is either no instruction left or the branch instructions have completely been processed.
                //go through instructions sequentially and compute absolute byte address for each instruction
                //to infer index of instruction from it. Use index-based ILLabel for branch targets instead of the absolute addresses
                for (int i = 0; i < instructions.Length && branchInstructions.Count > 0; i++)
                {
                    while (branchInstructions.Count > 0)
                    {
                        (int, uint) branch = branchInstructions[branchInstructions.Count - 1];

                        //exit while loop, if the byte offset of the label exceeds the current byte offset
                        if (branch.Item2 > byteOffset)
                            break;
                        else if (branch.Item2 != byteOffset) //target label has been skipped
                            throw new ArgumentException($"Label pointing to index that is not an operation code at index { branch.Item1 }.");

                        //normalize branch instruction so that it does only take InlineBrTarget operands avoiding 
                        ILInstruction p = instructions[branch.Item1];
                        p.Normalize();

                        //replace placeholder from first pass passing the index of the branch target to it
                        Instructions[branch.Item1] = new ILInstruction(p.OpCode, ILOperand.BranchTarget(new ILLabel(i, this)));

                        branchInstructions.RemoveAt(branchInstructions.Count - 1);
                    }
                    byteOffset += (uint)instructions[i].ByteSize;
                }
            }

            Instructions = new LabeledCollection<ILInstruction>(instr);

            instr.CollectionChanged += ObservedInstructions_CollectionChanged;

            _parameters = parameters == null ? new List<Type>() : new List<Type>(parameters);
            _localVariables = localVariables == null ? new List<Type>() : new List<Type>(localVariables);
            _labels = new List<int>();

            Parameters = _parameters.AsReadOnly();
        }

        public int InstructionCount => Instructions.Count;

        public Signature GetSignature(int metadataToken)
        {
            if (_signatureMap == null)
                _signatureMap = new Dictionary<int, Signature>();

            Signature s;

            if (!_signatureMap.TryGetValue(metadataToken, out s))
            {
                s = SignatureDecoder.DecodeSignature(SourceModule, SourceModule.ResolveSignature(metadataToken));
                _signatureMap.Add(metadataToken, s);
            }

            return s;
        }

        internal void CheckIndex(int index)
        {
            CheckIndex(index, nameof(index));
        }

        internal void CheckIndex(int index, string parameterName)
        {
            if (index < 0 || index >= Instructions.Count)
                throw new ArgumentOutOfRangeException(parameterName, "Index does not fall into the range of instructions.");
        }

        public void RemoveNop()
        {
            foreach (int i in Instructions.EnumerateIndicesLabeled())
            {
                if (Instructions[i].OpCode == OpCodes.Nop)
                    Instructions.RemoveAt(i);
            }
        }

        public int GetRelativeAddress(int index, int relativeToInstructionIndex)
        {
            CheckIndex(index);
            CheckIndex(relativeToInstructionIndex, nameof(relativeToInstructionIndex));

            int indexAddr = 0;
            int relativeAddr = 0;
            int sgn;

            if (relativeToInstructionIndex < index)
            {
                sgn = index;
                index = relativeToInstructionIndex;
                relativeToInstructionIndex = sgn;
                sgn = -1;
            }
            else
                sgn = 1;

            for (int i = 0; i < relativeToInstructionIndex; i++)
                relativeAddr += Instructions[i].ByteSize;

            return sgn * indexAddr;
        }

        public int GetIndex(int index, int offsetInBytes)
        {
            CheckIndex(index);

            int instrc = Instructions.Count;

            if (offsetInBytes < 0)
            {
                while (index >= 0 && offsetInBytes < 0)
                {
                    offsetInBytes += Instructions[index].ByteSize;
                    index--;
                }
            }
            else if (offsetInBytes > 0)
            {
                while (index < instrc && offsetInBytes > 0)
                {
                    offsetInBytes -= Instructions[index].ByteSize;
                    index++;
                }
            }

            if (index < 0 || index >= instrc)
                throw new ArgumentException("Offset in bytes exceeds boundaries defined by the byte size of the instructions.");

            if (offsetInBytes != 0)
                throw new ArgumentException("Offset in bytes does not point to the beginning of an instruction.", nameof(offsetInBytes));

            return index;
        }

        public int GetIndex(int address)
        {
            int index = 0;
            int instrc = Instructions.Count;

            while (index < instrc && address < 0)
            {
                address -= Instructions[index].ByteSize;
                index++;
            }

            if (index < 0 || index >= instrc)
                throw new ArgumentException("Address exceeds boundaries defined by the byte size of the instructions.");

            if (address != 0)
                throw new ArgumentException("Address does not point to the beginning of an instruction.", nameof(address));

            return index;
        }

        public int GetByteAddress(int index)
        {
            CheckIndex(index);

            int offset = 0;

            for (int i = 0; i < index; i++)
                offset += Instructions[i].ByteSize;

            return offset;
        }

        public int GetByteAddress(int index, int relativeToIndex)
        {
            if (index == relativeToIndex)
                return 0;

            int sign;
            int offset = 0;

            if (relativeToIndex < index)
            {
                sign = index;
                index = relativeToIndex;
                relativeToIndex = sign;
                sign = -1;
            }

            for (; index < relativeToIndex; index++)
                offset += Instructions[index].ByteSize;

            return offset;
        }

        public ILLabel GetLabel(int instructionIndex)
        {
            //initialize and perform index check directly by setting the property instead of initializing it with instructionIndex as a value
            return new ILLabel(0, this) { Index = instructionIndex };
        }

        private void ObservedInstructions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            int rangeIndex;
            int rangeUpper;
            int delta;

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                delta = e.NewItems.Count;
                rangeIndex = e.NewStartingIndex;
                rangeUpper = Instructions.Count;
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Move)
            {
                //unknown what the user's intention is (and it should not occur anyways)
                rangeIndex = 0;
                rangeUpper = 0;
                delta = 0;
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                delta = -e.OldItems.Count;
                rangeIndex = e.OldStartingIndex;
                rangeUpper = Instructions.Count;
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                delta = 0;
                rangeIndex = e.NewStartingIndex;
                rangeUpper = e.NewStartingIndex + e.NewItems.Count;
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                delta = 0;
                rangeUpper = 0;
                rangeIndex = 0;
            }
            else
                return;

            //update all previously registered labels depending on the operation applied
            if (delta != 0)
            {
                for (int i = 0; i < _labels.Count; i++)
                {
                    int lblind = _labels[i];

                    if (lblind >= rangeIndex && lblind < rangeUpper)
                    {
                        if (lblind + delta < rangeIndex)
                        {
                            //instruction with label has been removed
                            _labels.RemoveAt(i--);
                            continue;
                        }
                        _labels[i] = lblind = lblind + delta;
                    }

                    ILInstruction instr = Instructions[lblind];

                    ILLabel lbl = instr.Operand.GetBranchTarget();

                    if (lbl.Index >= rangeIndex && lbl.Index < rangeUpper)
                    {
                        int d = lbl.Index + delta;

                        Instructions[lblind] = new ILInstruction(instr.OpCode, ILOperand.BranchTarget(lbl));
                    }
                }
            }

            //initialize labels that are not relative to the current method manipulator and add the instruction index to the label collection
            if (delta >= 0)
            {
                for (int i = rangeIndex; i < rangeUpper; i++)
                {
                    ILInstruction instr = Instructions[i];

                    if (instr.Operand.IsBranch)
                    {
                        ILLabel lbl = instr.Operand.GetBranchTarget();

                        if (!lbl.IsIndex)
                        {
                            //relative address stored in label
                            lbl.MakeRelativeTo(this, i, true);
                        }
                        else if (lbl.LabelProvider != this)
                        {
                            //index relative to other method manipulator
                            lbl.MakeRelativeTo(this);
                        }
                        Instructions[i] = new ILInstruction(instr.OpCode, ILOperand.BranchTarget(lbl));
                        //add only once
                        if (!_labels.Contains(i))
                            _labels.Add(i);
                    }
                }
            }
        }

        private void CreateParameter(int parameterIndex, Type parameterType)
        {
            if (parameterType == null)
                throw new ArgumentNullException(nameof(parameterType));

            if (_parameters.Count == 0xffff)
                throw new InvalidOperationException("Maximum amount of parameters reached.");

            if (parameterIndex < 0 || parameterIndex > _parameters.Count)
                throw new ArgumentOutOfRangeException(nameof(parameterIndex), "Parameter index exceeds amount of available parameters.");

            _parameters.Insert(parameterIndex, parameterType);

            //Parameter has been inserted at the last position, no parameter updates required
            if (parameterIndex == _parameters.Count - 1)
                return;

            for (int i = 0; i < Instructions.Count; i++)
            {
                ILInstruction instr = Instructions[i];

                bool ld;
                int ind;

                if (instr.OpCode == OpCodes.Ldarga_S)
                {
                    ld = true;
                    ind = instr.Operand.GetLocalVariableIndex();
                    ind++;

                    if (ind > 255)
                        instr = new ILInstruction(OpCodes.Ldarg, ILOperand.Variable((ushort)ind));
                    else
                        instr = new ILInstruction(OpCodes.Ldarg_S, ILOperand.Variable((byte)ind));

                    Instructions[ind] = instr;
                    continue;
                }
                else if (instr.OpCode == OpCodes.Ldarga)
                {
                    ld = true;
                    ind = instr.Operand.GetLocalVariableIndex();

                    ind++;

                    instr = new ILInstruction(OpCodes.Ldarg, ILOperand.Variable((ushort)ind));

                    Instructions[ind] = instr;
                    continue;
                }
                else if (instr.OpCode == OpCodes.Ldarg_S || instr.OpCode == OpCodes.Ldarg)
                {
                    ld = true;
                    ind = instr.Operand.GetLocalVariableIndex();
                }
                else if (instr.OpCode == OpCodes.Ldarg_0)
                {
                    ind = 0;
                    ld = true;
                }
                else if (instr.OpCode == OpCodes.Ldarg_1)
                {
                    ind = 1;
                    ld = true;
                }
                else if (instr.OpCode == OpCodes.Ldarg_2)
                {
                    ind = 2;
                    ld = true;
                }
                else if (instr.OpCode == OpCodes.Ldarg_3)
                {
                    ind = 3;
                    ld = true;
                }
                else if (instr.OpCode == OpCodes.Starg || instr.OpCode == OpCodes.Starg_S)
                {
                    ind = instr.Operand.GetLocalVariableIndex();
                    ld = false;
                }
                else
                    continue;

                if (ind < parameterIndex)
                    continue;

                ind++;

                if (ind > 255)
                    instr = new ILInstruction(ld ? OpCodes.Ldarg : OpCodes.Starg, ILOperand.Variable((ushort)ind));
                else
                    instr = new ILInstruction(ld ? OpCodes.Ldarg_S : OpCodes.Starg_S, ILOperand.Variable((byte)ind));

                Instructions[ind] = instr;
            }
        }

        public void FieldToParameter(FieldInfo field, int parameterIndex, bool createParameter)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (field.IsStatic)
                throw new ArgumentException("Static fields cannot be replaced by parameters.", nameof(field));

            //TODO: get metadata token relative to source module

            if (createParameter)
                CreateParameter(parameterIndex, field.FieldType);


            int fieldToken = field.MetadataToken;
            ushort paramInd = (ushort)parameterIndex;

            var instrl = new List<KeyValuePair<int, ILInstruction>>();

            //determine and validate changes and store them in instrl
            for (int instrIndex = 0; instrIndex < Instructions.Count; instrIndex++)
            {
                ILInstruction instr = Instructions[instrIndex];
                if (instr.Operand.OperandType == OperandType.InlineField || instr.Operand.OperandType == OperandType.InlineTok)
                {
                    if (instr.Operand.GetMemberToken() == fieldToken)
                    {
                        if (instr.OpCode == OpCodes.Ldfld)
                            instrl.Add(new KeyValuePair<int, ILInstruction>(instrIndex, InstructionLdarg(paramInd)));
                        else if (instr.OpCode == OpCodes.Ldflda)
                            instrl.Add(new KeyValuePair<int, ILInstruction>(instrIndex, new ILInstruction(OpCodes.Ldarga, new ILOperand(OperandType.InlineVar, paramInd))));
                        else if (instr.OpCode == OpCodes.Stfld)
                            instrl.Add(new KeyValuePair<int, ILInstruction>(instrIndex, new ILInstruction(OpCodes.Starg, new ILOperand(OperandType.InlineVar, paramInd))));
                        else if (instr.OpCode == OpCodes.Ldtoken)
                            throw new ArgumentException("Unsupported replacement due to field token occurence in il code (instruction Ldtoken).");
                    }
                }
            }

            //Commit changes to 
            foreach (var kvp in instrl)
                Instructions[kvp.Key] = kvp.Value;
        }

        public StackIterator GetStackState()
        {
            return new StackIterator(this);
        }

        public IEnumerable<int> GetInstructionIndicesTo(Type type, bool inherited)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            foreach (int i in Instructions.EnumerateIndicesLabeled())
            {
                ILInstruction instr = Instructions[i];

                if (instr.OpCode == OpCodes.Call
                    || instr.OpCode == OpCodes.Callvirt
                    || instr.OpCode == OpCodes.Ldftn
                    || instr.OpCode == OpCodes.Ldvirtftn
                    || instr.OpCode == OpCodes.Ldtoken
                    || instr.OpCode == OpCodes.Jmp
                    || instr.OpCode == OpCodes.Newobj)
                {
                    var target = (MethodBase)instr.Operand.GetMember();

                    if (inherited ? type.IsAssignableFrom(target.DeclaringType) : type == target)
                        yield return i;
                }
            }
        }

        public IEnumerable<int> GetInstructionIndicesTo(MethodBase method, bool inherited)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            MethodInfo mc = method as MethodInfo;
            ConstructorInfo ctrc = method as ConstructorInfo;

            if (mc == null || ctrc == null)
                throw new ArgumentException("Unsupported type of method.", nameof(method));

            Type type = method.DeclaringType;
            bool generic = method.IsGenericMethodDefinition;
            bool isInterface = method.DeclaringType.IsInterface;

            foreach (int i in Instructions.EnumerateIndicesLabeled())
            {
                ILInstruction instr = Instructions[i];

                if (instr.OpCode == OpCodes.Call
                    || instr.OpCode == OpCodes.Callvirt
                    || instr.OpCode == OpCodes.Ldftn
                    || instr.OpCode == OpCodes.Ldvirtftn
                    || instr.OpCode == OpCodes.Ldtoken
                    || instr.OpCode == OpCodes.Jmp
                    || instr.OpCode == OpCodes.Newobj)
                {
                    var target = (MethodBase)instr.Operand.GetMember();

                    MethodInfo m = target as MethodInfo;
                    ConstructorInfo ctr = target as ConstructorInfo;

                    //TODO: check generic, check interfaces
                    if (m != null) //call to method
                    {
                        if (MethodEquals(mc, m, true))
                            yield return i;
                    }
                    else if (ctr == method) //check whether it is a call to a constructor
                        yield return i;
                }
            }
        }

        private static ILInstruction InstructionLdarg(ushort argIndex)
        {
            if (argIndex == 0)
                return new ILInstruction(OpCodes.Ldarg_0, ILOperand.None());
            else if (argIndex == 1)
                return new ILInstruction(OpCodes.Ldarg_1, ILOperand.None());
            else if (argIndex == 2)
                return new ILInstruction(OpCodes.Ldarg_2, ILOperand.None());
            else if (argIndex == 3)
                return new ILInstruction(OpCodes.Ldarg_3, ILOperand.None());
            else if (argIndex <= 255)
                return new ILInstruction(OpCodes.Ldarga_S, new ILOperand(OperandType.ShortInlineVar, argIndex));
            else
                return new ILInstruction(OpCodes.Ldarg, new ILOperand(OperandType.InlineVar, argIndex));
        }

        public MethodBuilder CreateMethod()
        {
            MethodBuilder method = DynamicMethod.Create(string.Empty, ReturnType ?? typeof(void), Parameters.ToArray());

            ILGenerator ilgen = method.GetILGenerator();

            var targetModule = (ModuleBuilder)method.Module;

            int index = 0;

            foreach (ILInstruction instr in Instructions)
                CodeManipulator.EmitOperation(SourceModule ?? targetModule, targetModule, ilgen, instr, index++);

            return method;
        }

        private static bool MethodEquals(MethodInfo baseDefinition, MethodInfo definition, bool checkOverride)
        {
            if (baseDefinition == definition)
                return true;

            if (!checkOverride)
                return false;

            MethodInfo pdef = null;

            for (; definition != null && !definition.IsHideBySig && pdef != definition; pdef = definition, definition = definition.GetBaseDefinition())
            {
                if (definition == baseDefinition)
                    return true;
            }

            return false;
        }
    }
}
