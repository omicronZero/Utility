using Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Dynamic.Disassembly
{
    public partial class StackIterator : IDisposable
    {
        private readonly MethodManipulator _method;
        private readonly List<StackEntry> _stack;
        private readonly Workflow.Collections.Labeled.Label _label;
        private readonly IList<StackEntry> _exposedStack;

        public IList<StackEntry> Stack
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(this.GetType().Name);
                return _exposedStack;
            }
        }

        public bool IsDisposed
        {
            get { return !_label.IsAttached; }
        }

        public bool MoveNext()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(this.GetType().Name);

            if (_label.IsEnd)
                return false;

            ILInstruction instr = _method.Instructions[_label];
            _label.Index += 1;

            int popCount;

            StackBehaviour behaviour = instr.OpCode.StackBehaviourPop;
            MethodSignature calliSignature = null;

            int popIndex = _stack.Count;

            if (behaviour == StackBehaviour.Pop1
                    || behaviour == StackBehaviour.Popi
                    || behaviour == StackBehaviour.Popref
                    || behaviour == StackBehaviour.Varpop)
                popCount = 1;
            else if (behaviour == StackBehaviour.Pop1_pop1
                    || behaviour == StackBehaviour.Popi_pop1
                    || behaviour == StackBehaviour.Popi_popi
                    || behaviour == StackBehaviour.Popi_popi8
                    || behaviour == StackBehaviour.Popi_popr4
                    || behaviour == StackBehaviour.Popi_popr8)
                popCount = 2;
            else if (behaviour == StackBehaviour.Popi_popi_popi
                    || behaviour == StackBehaviour.Popref_popi_pop1
                    || behaviour == StackBehaviour.Popref_pop1
                    || behaviour == StackBehaviour.Popref_popi
                    || behaviour == StackBehaviour.Popref_popi_popi
                    || behaviour == StackBehaviour.Popref_popi_popi8
                    || behaviour == StackBehaviour.Popref_popi_popr4
                    || behaviour == StackBehaviour.Popref_popi_popr8
                    || behaviour == StackBehaviour.Popref_popi_popref)
                popCount = 3;
            else
                popCount = 0;

            if (instr.OpCode == OpCodes.Call)
                popCount += ((MethodBase)instr.Operand.GetMember()).GetParameters().Length;
            else if (instr.OpCode == OpCodes.Calli)
            {
                calliSignature = SignatureDecoder.DecodeSignature(instr.Operand.GetSignatureModule(), instr.Operand.GetSignature()) as MethodSignature;

                if (calliSignature == null)
                    throw new ArgumentException($"Method signature expected for Calli instruction at instruction index { _label.Index }.");

                popCount += calliSignature.ParameterCount;
            }


            if (instr.OpCode.StackBehaviourPush != StackBehaviour.Push0)
            {
                StackObjectType so;
                StackObjectType modifiers;

                instr.Normalize();

                if (instr.OpCode == OpCodes.Ldarg || instr.OpCode == OpCodes.Ldarga)
                {
                    so = StackObjectType.Argument;

                    if (instr.OpCode == OpCodes.Ldarga)
                        so |= StackObjectType.Address;

                    if ((_method.CallingConvention & CallingConvention.HasThis) == CallingConvention.HasThis && instr.Operand.GetInt32() == 0)
                        so |= StackObjectType.ReferencesToThis;
                }
                else if (instr.OpCode == OpCodes.Arglist)
                    so = StackObjectType.Argument | StackObjectType.Address;
                else if (instr.OpCode == OpCodes.Ldc_I4 || instr.OpCode == OpCodes.Ldc_I8 || instr.OpCode == OpCodes.Ldc_R4 || instr.OpCode == OpCodes.Ldc_R8 || instr.OpCode == OpCodes.Ldstr)
                    so = StackObjectType.Constant;
                else if (instr.OpCode == OpCodes.Ldfld || instr.OpCode == OpCodes.Ldsfld)
                    so = StackObjectType.Field;
                else if (instr.OpCode == OpCodes.Ldflda || instr.OpCode == OpCodes.Ldsflda)
                    so = StackObjectType.Field | StackObjectType.Address;
                else if (instr.OpCode == OpCodes.Ldloc)
                    so = StackObjectType.Local;
                else if (instr.OpCode == OpCodes.Ldloca)
                    so = StackObjectType.Local | StackObjectType.Address;
                else if (instr.OpCode == OpCodes.Ldnull)
                    so = StackObjectType.Inferred;
                else if (instr.OpCode == OpCodes.Ldtoken)
                    so = StackObjectType.MetadataToken;
                else if (instr.OpCode == OpCodes.Unbox || instr.OpCode == OpCodes.Refanyval)
                    so = StackObjectType.Computed | StackObjectType.Address;
                else if (instr.OpCode == OpCodes.Localloc)
                    so = StackObjectType.LocalDynamicPoolMemory | StackObjectType.Address;
                else
                    so = StackObjectType.Computed;

                //separate stack object type and modifiers for following computations (they will be merged again, later)
                modifiers = so & StackObjectType.Modifiers;
                so = so & ~StackObjectType.Modifiers;

                if (instr.OpCode == OpCodes.Call || instr.OpCode == OpCodes.Callvirt || instr.OpCode == OpCodes.Newobj)
                {
                    MethodBase method = (MethodBase)instr.Operand.GetMember();
                    ParameterInfo[] param = method.GetParameters();

                    if (method.MemberType == MemberTypes.Method)
                    {
                        Type tp = ((MethodInfo)method).ReturnType;

                        if (tp != typeof(void))
                            _stack.Add(new StackEntry(StackObjectType.Return, tp, _label.Index));
                    }
                    else if (method.MemberType == MemberTypes.Constructor)
                        _stack.Add(new StackEntry(StackObjectType.Return, ((ConstructorInfo)method).DeclaringType, _label.Index));
                    else
                        throw new NotSupportedException($"Method or constructor expected as instruction target at instruction index { _label.Index }.");
                }
                else if (instr.OpCode == OpCodes.Calli)
                {
                    var pt = calliSignature.ReturnType.ParameterType;

                    if (pt.ElementType != ElementType.Void)
                    {
                        Type tp;
                        if (pt.IsGenericTypeParameter)
                            tp = calliSignature.ReturnType.ParameterType.ToType(_method.InstanceType);
                        else if (pt.IsGenericMethodParameter)
                        {
                            if (pt.GenericMethodParameterIndex >= _method.MethodGenericArguments.Count)
                                throw new ArgumentException($"Index of generic method argument exceeds amount of available generic arguments at instruction index { _label.Index }.");

                            tp = _method.MethodGenericArguments[pt.GenericMethodParameterIndex];
                        }
                        else
                            tp = calliSignature.ReturnType.ParameterType.ToType();

                        _stack.Add(new StackEntry(StackObjectType.Return, tp, _label.Index));
                    }
                }
                else if (so == StackObjectType.Constant)
                {
                    if (instr.OpCode == OpCodes.Ldstr)
                        _stack.Add(new StackEntry(so | modifiers, typeof(string), _label.Index, 0));
                    else
                    {
                        Type tp;
                        ulong v;
                        if (instr.OpCode == OpCodes.Ldc_I4)
                            tp = typeof(int);
                        else if (instr.OpCode == OpCodes.Ldc_I8)
                            tp = typeof(long);
                        else if (instr.OpCode == OpCodes.Ldc_R4)
                            tp = typeof(float);
                        else if (instr.OpCode == OpCodes.Ldc_R8)
                            tp = typeof(double);
                        else
                            throw new InvalidOperationException();
                        v = instr.Operand.RawUInt64;

                        _stack.Add(new StackEntry(so | modifiers, tp, _label.Index, v));
                    }
                }
                else
                {
                    Type tp;
                    if (so == StackObjectType.Field)
                        tp = instr.Operand.GetFieldMember().FieldType;
                    else if (so == StackObjectType.Argument)
                        tp = _method.Parameters[instr.Operand.GetLocalVariableIndex()];
                    else if (so == StackObjectType.Local)
                    {
                        int ind = instr.Operand.GetLocalVariableIndex();

                        if (ind >= _method.LocalVariables.Count)
                            throw new ArgumentException($"Local variable index { ind.ToString() } out of range at instruction index { _label.Index }.");

                        tp = _method.LocalVariables[ind];
                    }
                    else
                    {
                        StackBehaviour sp = instr.OpCode.StackBehaviourPush;

                        //TODO: distinguish between unsigned and signed int, bool?
                        if (sp == StackBehaviour.Pushi)
                            tp = typeof(int);
                        else if (sp == StackBehaviour.Pushi8)
                            tp = typeof(long);
                        else if (sp == StackBehaviour.Pushr4)
                            tp = typeof(float);
                        else if (sp == StackBehaviour.Pushr8)
                            tp = typeof(double);
                        else if (instr.OpCode == OpCodes.Ldnull)
                            tp = null;
                        else if (instr.OpCode == OpCodes.Dup)
                        {
                            if (_stack.Count == 0)
                                ThrowMissingElements(instr);

                            tp = _stack[_stack.Count - 1].Type;
                        }
                        else if (instr.OpCode == OpCodes.Ldind_Ref || instr.OpCode == OpCodes.Ldobj)
                        {
                            if (_stack.Count == 0)
                                ThrowMissingElements(instr);

                            tp = _stack[_stack.Count - 1].Type.GetElementType(); //TODO: check whether this is correct
                        }
                        else if (instr.OpCode == OpCodes.Add
                            || instr.OpCode == OpCodes.Sub
                            || instr.OpCode == OpCodes.Mul
                            || instr.OpCode == OpCodes.Div
                            || instr.OpCode == OpCodes.Rem)
                        {
                            if (_stack.Count < 2)
                                ThrowMissingElements(instr);

                            tp = EvaluateArithmeticResultType(_stack[0].Type, _stack[1].Type);
                        }
                        else if (instr.OpCode == OpCodes.And
                            || instr.OpCode == OpCodes.Or
                            || instr.OpCode == OpCodes.Xor)
                        {
                            if (_stack.Count < 2)
                                ThrowMissingElements(instr);

                            tp = EvaluateLogicResultType(_stack[0].Type, _stack[1].Type);
                        }
                        else if (instr.OpCode == OpCodes.Rem_Un
                            || instr.OpCode == OpCodes.Div_Un)
                        {
                            if (_stack.Count < 2)
                                ThrowMissingElements(instr);

                            //TODO: treat stack top as unsigned
                            tp = EvaluateArithmeticResultType(_stack[0].Type, _stack[1].Type);
                        }
                        else if (instr.OpCode == OpCodes.Shl
                            || instr.OpCode == OpCodes.Shr
                            || instr.OpCode == OpCodes.Shr_Un)
                        {
                            if (_stack.Count < 2)
                                ThrowMissingElements(instr);

                            tp = EvaluateShiftResultType(_stack[0].Type, _stack[1].Type);
                        }
                        else if (instr.OpCode == OpCodes.Neg)
                        {
                            if (_stack.Count == 0)
                                ThrowMissingElements(instr);

                            tp = EvaluateArithmeticResultType(_stack[0].Type);
                        }
                        else if (instr.OpCode == OpCodes.Not)
                        {
                            if (_stack.Count == 0)
                                ThrowMissingElements(instr);

                            tp = EvaluateLogicResultType(_stack[0].Type, _stack[1].Type);
                        }
                        else if (instr.OpCode == OpCodes.Castclass)
                            tp = instr.Operand.GetTypeMember();
                        else if (instr.OpCode == OpCodes.Unbox) //TODO: check whether this is correct (operand is not a pointer type?)
                            tp = instr.Operand.GetTypeMember();
                        else if (instr.OpCode == OpCodes.Box) //TODO: check whether this is correct (what is stored in instr.Operand?)
                            tp = typeof(object);
                        else if (instr.OpCode == OpCodes.Newarr)
                            tp = instr.Operand.GetTypeMember();
                        else if (instr.OpCode == OpCodes.Ldelem)
                            tp = instr.Operand.GetTypeMember();
                        else if (instr.OpCode == OpCodes.Unbox_Any)
                            tp = instr.Operand.GetTypeMember();
                        else if (instr.OpCode == OpCodes.Refanyval)
                            tp = instr.Operand.GetTypeMember();
                        else if (instr.OpCode == OpCodes.Mkrefany)
                            tp = instr.Operand.GetTypeMember();
                        else if (instr.OpCode == OpCodes.Localloc)
                            tp = typeof(byte);
                        else if (instr.OpCode == OpCodes.Ldvirtftn) //TODO: check whether this is correct
                            tp = typeof(Delegate);
                        else if (instr.OpCode == OpCodes.Arglist)
                            tp = typeof(IntPtr);
                        else
                            throw new ArgumentException($"Unknown return type on instruction { instr.OpCode.Name } at instruction index { _label.Index }.");
                    }

                    if ((modifiers & StackObjectType.Address) == StackObjectType.Address)
                        tp = tp?.MakePointerType() ?? typeof(IntPtr);


                    _stack.Add(new StackEntry(so | modifiers, tp, _label.Index));
                }
            }

            if (popCount > 0)
            {
                if (popIndex - popCount < 0)
                    throw new ArgumentException($"Stack does not hold enough items to execute instruction { instr.OpCode.Name } at instruction index { _label.Index }.");

                _stack.RemoveRange(popIndex - popCount, popCount);
            }
            return true;
        }

        private static Type EvaluateArithmeticResultType(Type left, Type right)
        {
            //ECMA III.1.5/Table III.2
            //TODO: add unsigned
            if (left == typeof(int))
            {
                if (right == typeof(int))
                    return typeof(int);
                else if (right == typeof(IntPtr))
                    return typeof(IntPtr);
                else if (right.IsPointer)
                    return right;
            }
            else if (left == typeof(long))
            {
                if (right == typeof(long))
                    return typeof(long);
            }
            else if (left == typeof(IntPtr))
            {
                if (right == typeof(int))
                    return typeof(IntPtr);
                else if (right == typeof(IntPtr))
                    return typeof(IntPtr);
                else if (right.IsPointer)
                    return right;
            }
            else if (left.IsPointer)
            {
                if (right == typeof(int))
                    return left;
                else if (right == typeof(IntPtr))
                    return left;
                else if (right.IsPointer)
                    return typeof(IntPtr);

            }
            else if (left == typeof(float))
            {
                if (right == typeof(float))
                    return typeof(float);
                else if (right == typeof(double))
                    return typeof(double);
            }
            else if (left == typeof(double))
            {
                if (right == typeof(float))
                    return typeof(double);
                else if (right == typeof(double))
                    return typeof(double);
            }

            throw new NotSupportedException($"Result type undefined for arithmetic binary operation between types { left.FullName } and { right.FullName }.");
        }

        private static Type EvaluateArithmeticResultType(Type unary)
        {
            return unary;
        }

        private static Type EvaluateLogicResultType(Type left, Type right)
        {
            //ECMA III.1.5/Table III.6
            //TODO: add unsigned types
            if (left == typeof(int))
            {
                if (right == typeof(int))
                    return typeof(int);
                else if (right == typeof(IntPtr))
                    return typeof(IntPtr);
            }
            else if (left == typeof(long))
            {
                if (right == typeof(long))
                    return typeof(long);
            }
            else if (left == typeof(IntPtr))
            {
                if (right == typeof(int))
                    return typeof(IntPtr);
                else if (right == typeof(IntPtr))
                    return typeof(IntPtr);
            }
            throw new NotSupportedException($"Result type undefined for logic binary operation between types { left.FullName } and { right.FullName }.");
        }

        private static Type EvaluateLogicResultType(Type unary)
        {
            return unary;
        }

        private static Type EvaluateShiftResultType(Type left, Type right)
        {
            //ECMA III.1.5/Table III.6

            //TODO: add unsigned types
            if (left == typeof(int))
            {
                if (right == typeof(int) || right == typeof(IntPtr))
                    return typeof(int);
            }
            else if (left == typeof(long))
            {
                if (right == typeof(int) || right == typeof(IntPtr))
                    return typeof(long);
            }
            else if (left == typeof(IntPtr))
            {
                if (right == typeof(int) || right == typeof(IntPtr))
                    return typeof(IntPtr);
            }
            throw new NotSupportedException($"Result type undefined for shifting operation between types { left.FullName } and { right.FullName }.");
        }

        private void ThrowMissingElements(ILInstruction instr)
        {
            throw new ArgumentException($"Could not infer type for { instr.OpCode.Name } instruction at index { _label.Index } as stack did not hold sufficient items.");
        }

        public void GoTo(int instructionIndex, bool clearStack)
        {
            if (instructionIndex < 0 || instructionIndex > _method.Instructions.Count)
                throw new ArgumentOutOfRangeException(nameof(instructionIndex), "Instruction index does not lie within the range of instructions.");

            _label.Index = instructionIndex;
            _stack.Clear();
        }

        public bool Move(int steps)
        {
            if (steps < 0)
                throw new ArgumentException("Non-negative amount of steps expected.", nameof(steps));
            else if (steps == 0 && _label.IsEnd)
                return false;


            for (int i = 0; i < steps; i++)
                if (!MoveNext())
                    return false;

            return true;
        }

        public void Reset()
        {
            GoTo(0, true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                _method.Instructions.RemoveLabel(_label);
                _stack.Clear();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        internal StackIterator(MethodManipulator method)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            _stack = new List<StackEntry>();
            _method = method;
            _label = method.Instructions.CreateLabel(0);
            _exposedStack = _stack.AsReadOnly();
        }
    }
}
