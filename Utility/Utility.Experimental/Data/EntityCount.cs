using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Data
{
    [Serializable]
    public struct EntityCount : IEquatable<EntityCount>
    {
        public long Count { get; set; }

        public EntityCount(long count)
        {
            Count = count;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var c = obj as EntityCount?;

            if (c == null)
                return false;

            return Equals(c.Value);
        }

        public override int GetHashCode()
        {
            return Count.GetHashCode();
        }

        public bool Equals(EntityCount other)
        {
            return Count == other.Count;
        }

        public override string ToString()
        {
            return Count.ToString();
        }

        public static implicit operator long(EntityCount value)
        {
            return value.Count;
        }

        public static implicit operator EntityCount(long value)
        {
            return new EntityCount(value);
        }

        public static bool operator <(EntityCount left, EntityCount right)
        {
            return left.Count < right.Count;
        }

        public static bool operator <=(EntityCount left, EntityCount right)
        {
            return left.Count <= right.Count;
        }

        public static bool operator ==(EntityCount left, EntityCount right)
        {
            return left.Count == right.Count;
        }

        public static bool operator !=(EntityCount left, EntityCount right)
        {
            return left.Count != right.Count;
        }

        public static bool operator >=(EntityCount left, EntityCount right)
        {
            return left.Count >= right.Count;
        }

        public static bool operator >(EntityCount left, EntityCount right)
        {
            return left.Count > right.Count;
        }
    }
}
