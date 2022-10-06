using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Utility
{
    [Serializable]
    public unsafe struct ExtendingUInt : IEquatable<ExtendingUInt>, ISerializable, IComparable<ExtendingUInt>
    {
        private fixed byte _buffer[9];

        public ExtendingUInt(ulong value)
        {
            fixed (byte* bptr = _buffer)
            {
                for (int i = 0; i < 9; i++)
                {
                    bptr[i] = (byte)((value > 127 ? (1 << 7) : 0) | (int)(value & 0x7f));
                }
            }
        }

        public ExtendingUInt(long rawInt64)
            : this(unchecked((ulong)rawInt64))
        { }

        private ExtendingUInt(SerializationInfo info, StreamingContext context)
            : this((info ?? throw new ArgumentNullException(nameof(info))).GetUInt64("Value"))
        { }

        public ulong ToUInt64()
        {
            fixed (byte* bptr = _buffer)
            {
                ulong v = 0;
                int i = 0;
                byte t;

                do
                {
                    t = bptr[i];

                    v |= (t & 0x7ful) << (7 * i);
                    i++;
                } while (i < 9 && (t >> 7) == 1);

                if (i == 9 && (t >> 7) == 1)
                {
                    v |= 1ul << 63;
                }

                return v;
            }
        }

        public long AsRawInt64()
        {
            return unchecked((long)ToUInt64());
        }

        public int GetDataLength()
        {
            fixed (byte* bptr = _buffer)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (bptr[i] <= 127)
                        return i;
                }
            }
            return 9;
        }

        public byte[] GetData()
        {
            byte[] buffer = new byte[GetDataLength()];

            GetData(buffer, 0, buffer.Length);

            return buffer;
        }

        public int GetData(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            return GetData(buffer, 0, buffer.Length);
        }

        public int GetData(byte[] buffer, int index, int count)
        {
            return GetData(0, buffer, index, count);
        }

        //TODO: test
        public int GetData(int offset, byte[] buffer, int index, int count)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "Non-negative offset expected.");

            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Non-negative index expected.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative count expected.");

            if (index + count > buffer.Length)
                throw new ArgumentException("The indicated range exceeds the boundaries of the specified buffer.");

            if (count == 0 || offset >= 9)
            {
                return GetDataLength() - offset;
            }

            int c = -offset;
            fixed (byte* bptr = _buffer)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (offset-- <= 0)
                    {
                        if (count == 0)
                            return c;

                        buffer[index++] = bptr[i];

                        count--;
                    }

                    c++;

                    if (bptr[i] <= 127)
                        break;
                }
            }

            return c;
        }

        public override string ToString()
        {
            return ToUInt64().ToString();
        }

        public override int GetHashCode()
        {
            return ToUInt64().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is ExtendingUInt other && Equals(other);
        }

        public bool Equals(ExtendingUInt other)
        {
            byte* rptr = other._buffer;

            fixed (byte* lptr = _buffer)
            {
                return *(long*)lptr == *(long*)rptr && lptr[8] == rptr[8];
            }
        }

        public int CompareTo(ExtendingUInt other)
        {
            return ToUInt64().CompareTo(other.ToUInt64());
        }

        private void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            info.AddValue("Value", ToUInt64());
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(info, context);
        }

        public static implicit operator ExtendingUInt(ulong value)
        {
            return new ExtendingUInt(value);
        }

        public static explicit operator ulong(ExtendingUInt value)
        {
            return value.ToUInt64();
        }

        public static bool operator ==(ExtendingUInt left, ExtendingUInt right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ExtendingUInt left, ExtendingUInt right)
        {
            return !(left == right);
        }
    }
}
