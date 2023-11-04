using System;

namespace Utility
{
    [Serializable]
    public struct Id : IEquatable<Id>, IComparable<Id>
    {
        public ulong Major { get; set; }
        public ulong Minor { get; set; }

        public Id(ulong major, ulong minor)
        {
            Major = major;
            Minor = minor;
        }

        public void Deconstruct(out ulong major, out ulong minor)
        {
            major = Major;
            minor = Minor;
        }

        public override bool Equals(object obj)
        {
            return obj is Id id && Equals(id);
        }

        public bool Equals(Id other)
        {
            return Major == other.Major &&
                   Minor == other.Minor;
        }

        public override int GetHashCode()
        {
            return Minor.GetHashCode() ^ Major.GetHashCode();
        }

        public override string ToString()
        {
            return (Major, Minor).ToString();
        }

        public int CompareTo(Id other)
        {
            int c = Major.CompareTo(other.Major);

            if (c != 0) 
                return c;

            return Minor.CompareTo(other.Minor);
        }

        public static bool operator ==(Id left, Id right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Id left, Id right)
        {
            return !(left == right);
        }

        public static bool operator <(Id left, Id right)
        {
            if (left.Major < right.Major)
                return true;
            else if (left.Major == right.Major)
                return left.Minor < right.Minor;

            return false;
        }

        public static bool operator <=(Id left, Id right)
        {
            if (left.Major <= right.Major)
                return true;
            else if (left.Major == right.Major)
                return left.Minor <= right.Minor;

            return false;
        }

        public static bool operator >(Id left, Id right) => right < left;
        public static bool operator >=(Id left, Id right) => right <= left;

        public static implicit operator Id(ulong value) => new Id(0, value);

        public static Id operator ++(Id value)
        {
            unchecked
            {
                ulong maj = value.Major;
                ulong min = value.Minor;

                if (++min == 0)
                    maj++;

                return new Id(maj, min);
            }
        }

        public static Id operator --(Id value)
        {
            unchecked
            {
                ulong maj = value.Major;
                ulong min = value.Minor;

                if (--min == ulong.MaxValue)
                    maj--;

                return new Id(maj, min);
            }
        }

        public static implicit operator Id((ulong Major, ulong Minor) id) => new Id(id.Major, id.Minor);
    }
}
