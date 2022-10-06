using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    internal class ByteArrayReader
    {
        private readonly byte[] _data;
        private int _index;

        public ByteArrayReader(byte[] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (index < 0 || index >= data.Length)
                throw new ArgumentOutOfRangeException(nameof(index), "Index does not fall into the range of the supplied data array.");

            _data = data;
            _index = index;
        }

        public byte CurrentValue
        {
            get
            {
                if (_index < 0)
                    throw new InvalidOperationException("Non-negative index expected.");
                if (_index >= _data.Length)
                    throw UnexpectedEndOfStream();

                return _data[_index];
            }
        }

        public bool Advance()
        {
            if (_index < _data.Length)
            {
                _index++;
                return _index >= 0 && _index < _data.Length;
            }
            else
                return false;
        }

        public bool Advance(out byte value)
        {
            if (Advance())
            {
                value = CurrentValue;
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public byte ReadByte()
        {
            byte cb = CurrentValue;
            _index++;
            return cb;
        }

        //TODO: optimize ReadInt16, ReadInt32, ReadInt64, ReadSingle, ReadDouble
        public short ReadInt16()
        {
            return (short)((ReadByte() << 8) | ReadByte());
        }

        public int ReadInt32()
        {
            return (ReadInt16() << 16) | unchecked((ushort)ReadInt16());
        }

        public byte PeekByte()
        {
            byte b;

            if (!Advance(out b))
                throw UnexpectedEndOfStream();

            _index--;
            return b;
        }

        public long ReadInt64()
        {
            return ((long)ReadInt32() << 32) | unchecked((uint)ReadInt32());
        }

        public ushort ReadUInt16()
        {
            return unchecked((ushort)ReadInt16());
        }

        public uint ReadUInt32()
        {
            return unchecked((uint)ReadInt32());
        }

        public ulong ReadUInt64()
        {
            return unchecked((ulong)ReadInt64());
        }

        public unsafe double ReadDouble()
        {
            long v = ReadInt64();

            return *(double*)&v;
        }

        public unsafe float ReadSingle()
        {
            int v = ReadInt32();

            return *(float*)&v;
        }

        public static implicit operator byte(ByteArrayReader indexer)
        {
            return indexer.CurrentValue;
        }

        private static Exception UnexpectedEndOfStream()
        {
            return new InvalidOperationException("Unexpected end of byte stream.");
        }
    }
}
