using System;
using System.Linq;
using System.Runtime.Serialization;
using Utility.Collections;
using Utility;

namespace Utility.Dynamic
{
    [Serializable]
    public struct TypeArrayKey : IEquatable<TypeArrayKey>, ISerializable
    {
        private readonly int _hashCode;

        private readonly Type[] _types;

        public Type ReturnType { get; }

        private TypeArrayKey(SerializationInfo info, StreamingContext context)
            : this((info ?? throw new ArgumentNullException(nameof(info))).GetValue<Type>("ReturnType"), info.GetValue<Type[]>("Types"))
        { }

        public TypeArrayKey(Type returnType, Type[] types)
        {
            if (returnType == null)
                throw new ArgumentNullException(nameof(returnType));
            if (types == null)
                throw new ArgumentNullException(nameof(types));

            types = (Type[])types.Clone();

            int hc = returnType.GetHashCode();

            foreach (Type tp in types)
            {
                if (tp == null)
                    throw new ArgumentException("Null is not a valid value for the entries in types.", nameof(types));

                hc ^= tp.GetHashCode();
            }

            ReturnType = returnType;
            _types = types;
            _hashCode = hc;
        }

        public ReadOnlyList<Type> Types => new ReadOnlyList<Type>(_types);

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is TypeArrayKey k && Equals(k);
        }

        public bool Equals(TypeArrayKey other)
        {
            if (_hashCode != other._hashCode || ReturnType != other.ReturnType)
                return false;

            if (_types == other._types)
                return true;

            if (_types.Length != other._types.Length)
                return false;

            for (int i = 0; i < _types.Length; i++)
            {
                if (_types[i] != other._types[i])
                    return false;
            }

            return true;
        }

        private void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            info.AddValue("ReturnType", ReturnType);
            info.AddValue("Types", _types);
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(info, context);
        }

        public static bool operator ==(TypeArrayKey left, TypeArrayKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TypeArrayKey left, TypeArrayKey right)
        {
            return !(left == right);
        }
    }
}
