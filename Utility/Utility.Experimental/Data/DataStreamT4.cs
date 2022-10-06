

using System;

namespace Utility.Data
{
	public unsafe partial class DataStream
	{
				public void Write(sbyte[] buffer, int index, int count)
		{
			Util.ValidateNamedRange(buffer, index, count, arrayName: nameof(buffer));

			fixed(sbyte* ptr = buffer)
				Write(new IntPtr(ptr + index), count * sizeof(sbyte));
		}
		
		public void Write(sbyte[] buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException(nameof(buffer));

			Write(buffer, 0, buffer.Length);
		}

		public void Write(sbyte value)
		{
			Write(new IntPtr(&value), sizeof(sbyte));
		}

        public AssembledObject<sbyte> ReadSBytes(int count)
        {
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count), count, "Non-negative count expected.");

            return new AssembledObject<sbyte>(count);
        }

        public AssembledObject<sbyte> ReadSByte()
        {
			return ReadSBytes(1);
        }
				public void Write(short[] buffer, int index, int count)
		{
			Util.ValidateNamedRange(buffer, index, count, arrayName: nameof(buffer));

			fixed(short* ptr = buffer)
				Write(new IntPtr(ptr + index), count * sizeof(short));
		}
		
		public void Write(short[] buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException(nameof(buffer));

			Write(buffer, 0, buffer.Length);
		}

		public void Write(short value)
		{
			Write(new IntPtr(&value), sizeof(short));
		}

        public AssembledObject<short> ReadInt16s(int count)
        {
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count), count, "Non-negative count expected.");

            return new AssembledObject<short>(count);
        }

        public AssembledObject<short> ReadInt16()
        {
			return ReadInt16s(1);
        }
				public void Write(ushort[] buffer, int index, int count)
		{
			Util.ValidateNamedRange(buffer, index, count, arrayName: nameof(buffer));

			fixed(ushort* ptr = buffer)
				Write(new IntPtr(ptr + index), count * sizeof(ushort));
		}
		
		public void Write(ushort[] buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException(nameof(buffer));

			Write(buffer, 0, buffer.Length);
		}

		public void Write(ushort value)
		{
			Write(new IntPtr(&value), sizeof(ushort));
		}

        public AssembledObject<ushort> ReadUInt16s(int count)
        {
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count), count, "Non-negative count expected.");

            return new AssembledObject<ushort>(count);
        }

        public AssembledObject<ushort> ReadUInt16()
        {
			return ReadUInt16s(1);
        }
				public void Write(int[] buffer, int index, int count)
		{
			Util.ValidateNamedRange(buffer, index, count, arrayName: nameof(buffer));

			fixed(int* ptr = buffer)
				Write(new IntPtr(ptr + index), count * sizeof(int));
		}
		
		public void Write(int[] buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException(nameof(buffer));

			Write(buffer, 0, buffer.Length);
		}

		public void Write(int value)
		{
			Write(new IntPtr(&value), sizeof(int));
		}

        public AssembledObject<int> ReadInt32s(int count)
        {
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count), count, "Non-negative count expected.");

            return new AssembledObject<int>(count);
        }

        public AssembledObject<int> ReadInt32()
        {
			return ReadInt32s(1);
        }
				public void Write(uint[] buffer, int index, int count)
		{
			Util.ValidateNamedRange(buffer, index, count, arrayName: nameof(buffer));

			fixed(uint* ptr = buffer)
				Write(new IntPtr(ptr + index), count * sizeof(uint));
		}
		
		public void Write(uint[] buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException(nameof(buffer));

			Write(buffer, 0, buffer.Length);
		}

		public void Write(uint value)
		{
			Write(new IntPtr(&value), sizeof(uint));
		}

        public AssembledObject<uint> ReadUInt32s(int count)
        {
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count), count, "Non-negative count expected.");

            return new AssembledObject<uint>(count);
        }

        public AssembledObject<uint> ReadUInt32()
        {
			return ReadUInt32s(1);
        }
				public void Write(long[] buffer, int index, int count)
		{
			Util.ValidateNamedRange(buffer, index, count, arrayName: nameof(buffer));

			fixed(long* ptr = buffer)
				Write(new IntPtr(ptr + index), count * sizeof(long));
		}
		
		public void Write(long[] buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException(nameof(buffer));

			Write(buffer, 0, buffer.Length);
		}

		public void Write(long value)
		{
			Write(new IntPtr(&value), sizeof(long));
		}

        public AssembledObject<long> ReadInt64s(int count)
        {
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count), count, "Non-negative count expected.");

            return new AssembledObject<long>(count);
        }

        public AssembledObject<long> ReadInt64()
        {
			return ReadInt64s(1);
        }
				public void Write(ulong[] buffer, int index, int count)
		{
			Util.ValidateNamedRange(buffer, index, count, arrayName: nameof(buffer));

			fixed(ulong* ptr = buffer)
				Write(new IntPtr(ptr + index), count * sizeof(ulong));
		}
		
		public void Write(ulong[] buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException(nameof(buffer));

			Write(buffer, 0, buffer.Length);
		}

		public void Write(ulong value)
		{
			Write(new IntPtr(&value), sizeof(ulong));
		}

        public AssembledObject<ulong> ReadUInt64s(int count)
        {
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count), count, "Non-negative count expected.");

            return new AssembledObject<ulong>(count);
        }

        public AssembledObject<ulong> ReadUInt64()
        {
			return ReadUInt64s(1);
        }
				public void Write(float[] buffer, int index, int count)
		{
			Util.ValidateNamedRange(buffer, index, count, arrayName: nameof(buffer));

			fixed(float* ptr = buffer)
				Write(new IntPtr(ptr + index), count * sizeof(float));
		}
		
		public void Write(float[] buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException(nameof(buffer));

			Write(buffer, 0, buffer.Length);
		}

		public void Write(float value)
		{
			Write(new IntPtr(&value), sizeof(float));
		}

        public AssembledObject<float> ReadSingles(int count)
        {
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count), count, "Non-negative count expected.");

            return new AssembledObject<float>(count);
        }

        public AssembledObject<float> ReadSingle()
        {
			return ReadSingles(1);
        }
				public void Write(double[] buffer, int index, int count)
		{
			Util.ValidateNamedRange(buffer, index, count, arrayName: nameof(buffer));

			fixed(double* ptr = buffer)
				Write(new IntPtr(ptr + index), count * sizeof(double));
		}
		
		public void Write(double[] buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException(nameof(buffer));

			Write(buffer, 0, buffer.Length);
		}

		public void Write(double value)
		{
			Write(new IntPtr(&value), sizeof(double));
		}

        public AssembledObject<double> ReadDoubles(int count)
        {
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count), count, "Non-negative count expected.");

            return new AssembledObject<double>(count);
        }

        public AssembledObject<double> ReadDouble()
        {
			return ReadDoubles(1);
        }
				public void Write(char[] buffer, int index, int count)
		{
			Util.ValidateNamedRange(buffer, index, count, arrayName: nameof(buffer));

			fixed(char* ptr = buffer)
				Write(new IntPtr(ptr + index), count * sizeof(char));
		}
		
		public void Write(char[] buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException(nameof(buffer));

			Write(buffer, 0, buffer.Length);
		}

		public void Write(char value)
		{
			Write(new IntPtr(&value), sizeof(char));
		}

        public AssembledObject<char> ReadChars(int count)
        {
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count), count, "Non-negative count expected.");

            return new AssembledObject<char>(count);
        }

        public AssembledObject<char> ReadChar()
        {
			return ReadChars(1);
        }
			}
}