using System;
using System.Collections.Generic;
using System.Reflection;

namespace Utility.Dynamic.Disassembly
{
    public struct TypeDescription
    {
        public Module Module { get; }

        private ElementType _elementType;
        private int _param1;
        private int _param2;
        private object _param3;

        public ElementType ElementType
        {
            get { return _elementType; }
        }

        public bool IsGenericTypeParameter
        {
            get { return ElementType == ElementType.Var; }
        }

        public bool IsGenericMethodParameter
        {
            get { return ElementType == ElementType.MVar; }
        }

        public int GenericTypeParameterIndex
        {
            get
            {
                if (!IsGenericTypeParameter)
                    throw new NotSupportedException("Type description does not represent a generic type argument.");

                return _param1;
            }
        }

        public int GenericMethodParameterIndex
        {
            get
            {
                if (!IsGenericMethodParameter)
                    throw new NotSupportedException("Type description does not represent a generic method argument.");

                return _param1;
            }
        }

        public Type ToType(MethodInfo genericMethod)
        {
            if (genericMethod == null)
                return ToType();

            if (!(genericMethod.IsGenericMethod || genericMethod.IsGenericMethodDefinition))
                throw new ArgumentException("Generic method expected.", nameof(genericMethod));

            if (!IsGenericMethodParameter)
                throw new InvalidOperationException("Type is not a generic method argument.");

            int ind = GenericMethodParameterIndex;
            //TODO: validate ind
            return genericMethod.GetGenericArguments()[ind];
        }

        public Type ToType(Type genericType)
        {
            if (genericType == null)
                return ToType();

            if (genericType.IsGenericParameter)
                throw new ArgumentException("Generic type must not be a generic parameter.", nameof(genericType));

            if (!(genericType.IsGenericType || genericType.IsGenericTypeDefinition))
                throw new ArgumentException("Generic type expected.", nameof(genericType));

            if (!IsGenericTypeParameter)
                throw new InvalidOperationException("Type is not a generic method argument.");

            int ind = GenericTypeParameterIndex;
            //TODO: validate ind
            return genericType.GetGenericArguments()[ind];
        }

        public Type ToType()
        {
            if (Module == null)
                throw new InvalidOperationException("Module must not be null.");

            if (_elementType == Disassembly.ElementType.Array)
            {
                return ((ArrayDescription)_param3).ToType();
            }
            else if (_elementType == Disassembly.ElementType.Class
                || _elementType == Disassembly.ElementType.ValueType)
            {
                return Module.ResolveType(_param1);
            }
            else if (_elementType == Disassembly.ElementType.GenericInst)
            {
                Type tp = Module.ResolveType(_param2);
                var p = (TypeDescription[])_param3;
                Type[] genargs = new Type[p.Length];
                //TODO: model as tuple (parameterIndex, referencedArgIndex)
                Queue<int> genargRef = null;

                for (int i = 0; i < p.Length; i++)
                {
                    TypeDescription param = p[i];

                    if (param.IsGenericTypeParameter)
                    {
                        int ind = param.GenericTypeParameterIndex;

                        if (ind > i || genargs[i] == null)
                            (genargRef ?? (genargRef = new Queue<int>())).Enqueue(i);
                    }
                    else
                        genargs[i] = param.ToType();
                }

                if (genargRef != null)
                {
                    int remChanges = genargRef.Count;

                    while (genargRef.Count > 0)
                    {
                        int paramIndex = genargRef.Dequeue();
                        int argIndex = p[paramIndex].GenericTypeParameterIndex;

                        if (genargs[argIndex] != null)
                        {
                            genargs[paramIndex] = genargs[argIndex];
                            remChanges = genargRef.Count;
                        }
                        else
                        {
                            genargRef.Enqueue(paramIndex);
                            remChanges--;
                        }

                        if (remChanges == 0)
                            throw new ArgumentException("Could not resolve generic arguments for generic type due to cyclic definitions.");
                    }
                }

                return tp.MakeGenericType(Array.ConvertAll((TypeDescription[])_param3, (t) => t.ToType()));
            }
            else if (_elementType == Disassembly.ElementType.SzArray)
            {
                var v = (VectorArrayDescription)_param3;
                Type tp = v.Type.ToType();
                return tp.MakeArrayType();
            }
            else if (_elementType == Disassembly.ElementType.Var)
            {
                //generic type argument
                throw new InvalidOperationException("Type is a generic type argument. Supply the type defining the generic type argument to resolve this type.");
            }
            else if (_elementType == Disassembly.ElementType.MVar)
            {
                //generic method argument 
                throw new InvalidOperationException("Type is a generic method argument. Supply the method defining the generic type argument to resolve this type.");
            }
            else
            {
                Type tp;
                if (ResolveStandardType(_elementType, out tp))
                    return tp;
                throw new ArgumentException("Type description represents an unsupported type.");
            }
        }

        internal TypeDescription(Module module, ElementType elementType, int param1, int param2, object param3)
        {
            Module = module;
            _param1 = param1;
            _param2 = param2;
            _param3 = param3;
            _elementType = elementType;
        }

        //TODO: add public methods to instantiate TypeDescription

        private static readonly Type[] TypeMap = new Type[]{
                typeof(void),
                typeof(bool),
                typeof(char),
                typeof(sbyte),
                typeof(byte),
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                typeof(float),
                typeof(double),
                typeof(IntPtr),
                typeof(UIntPtr)
        };

        private static bool ResolveStandardType(ElementType elementType, out Type type)
        {
            if (elementType == Disassembly.ElementType.Object)
                type = typeof(object);
            else if (elementType == Disassembly.ElementType.String)
                type = typeof(string);
            else if (elementType >= Disassembly.ElementType.Void && elementType <= Disassembly.ElementType.U)
                type = TypeMap[(int)elementType - 1];
            else
            {
                type = null;
                return false;
            }
            return true;
        }
    }
}