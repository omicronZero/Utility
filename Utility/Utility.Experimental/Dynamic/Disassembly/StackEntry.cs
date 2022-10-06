using System;

namespace Utility.Dynamic.Disassembly
{
    public struct StackEntry
    {
        public StackObjectType ObjectInfo { get; }

        private readonly Type _type;
        public int InstructionIndex { get; }

        private readonly ulong _constant;

        public bool IsInferred
        {
            get { return _type == null; }
        }

        public Type Type
        {
            get { return _type ?? typeof(object); }
        }

        public ulong GetAsULong()
        {
            return _constant;
        }

        public unsafe double GetDouble()
        {
            if (_type == typeof(double))
                throw new ArgumentException("Object is not a double.");

            ThrowNotConstant();

            ulong inst = _constant;

            return *(double*)&inst;
        }

        public unsafe float GetFloat()
        {
            if (_type == typeof(float))
                throw new ArgumentException("Object is not a float.");

            ThrowNotConstant();

            ulong inst = _constant;

            return *(float*)&inst;
        }

        public int GetInt32()
        {
            if (_type == typeof(int))
                throw new ArgumentException("Object is not a 32-bit integer.");

            ThrowNotConstant();

            return unchecked((int)_constant);
        }

        private unsafe void ThrowNotConstant()
        {
            if (ObjectInfo != StackObjectType.Constant)
                throw new ArgumentException("Object is not a constant.");
        }

        //TODO: add all other supported stack entry constant types

        //type == null indicates a type that is inferred
        internal StackEntry(StackObjectType objectType, Type type, int instructionIndex)
            : this(objectType, type, instructionIndex, 0)
        { }

        internal StackEntry(StackObjectType objectType, Type type, int instructionIndex, ulong instance)
        {
            ObjectInfo = objectType;
            _type = type;
            InstructionIndex = instructionIndex;
            _constant = instance;
        }
    }
}