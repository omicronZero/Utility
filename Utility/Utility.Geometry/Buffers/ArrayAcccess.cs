
using System;


namespace Utility.Geometry.Buffers
{
	public struct ArrayAccess2D<T>
	{
		private readonly long _startIndex;
		private readonly T[] _array;

				public long Width { get; }
				public long Height { get; }
		
		public int Length
		{
			get => (int)LongLength;
		}

		public long LongLength
		{
			get => Width * Height;
		}

		public ArrayAccess2D(T[] array, long width, long height)
		: this(array, 0, width, height)
		{ }

		public ArrayAccess2D(T[] array, long startIndex, long width, long height)
		{
			if (array == null)
				throw new ArgumentNullException(nameof(array));

			if (startIndex < 0)
				throw new ArgumentOutOfRangeException(nameof(startIndex), "Non-negative start index expected.");

			_array = array;
			_startIndex = startIndex;
				
			long len = 1;
			
			if (width < 0)
				throw new ArgumentOutOfRangeException(nameof(width), "Non-negative width expected.");
			len *= width;

			
			if (height < 0)
				throw new ArgumentOutOfRangeException(nameof(height), "Non-negative height expected.");
			len *= height;

			
			if (array.LongLength - startIndex < len)
				throw new ArgumentException("The array's size is too small to hold the amount of entries required by the dimensions.", nameof(array));
			
						Width = width;
						Height = height;
					}

		public T this[long arrayIndex]
		{
			get
			{
				CheckIndex(arrayIndex);
				return _array[arrayIndex + _startIndex];
			}
			set
			{
				CheckIndex(arrayIndex);
				_array[arrayIndex + _startIndex] = value;
			}
		}

		public T this[long x, long y]
		{
			get
			{
				return _array[ComputeIndex(x, y)];				
			}
			set
			{
				_array[ComputeIndex(x, y)] = value;
			}
		}

		private long ComputeIndex(long x, long y)
		{
			//uninitialized
			if (_array == null)
				throw new ArgumentException("Array access has not been initialized.");

						if (x < 0 || x >= Width)
				throw new ArgumentOutOfRangeException(nameof(x), "x does not lie within its dimension size.");

						if (y < 0 || y >= Height)
				throw new ArgumentOutOfRangeException(nameof(y), "y does not lie within its dimension size.");

			
			return _startIndex + x + Width * (y);
		}

		private void CheckIndex(long arrayIndex)
		{
			if (arrayIndex < 0 || arrayIndex >= LongLength)
				throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Array index does not fall into the valid range.");
		}

		//TODO: CopyTo, vector access, etc.
	}
}


namespace Utility.Geometry.Buffers
{
	public struct ArrayAccess3D<T>
	{
		private readonly long _startIndex;
		private readonly T[] _array;

				public long Width { get; }
				public long Height { get; }
				public long Depth { get; }
		
		public int Length
		{
			get => (int)LongLength;
		}

		public long LongLength
		{
			get => Width * Height * Depth;
		}

		public ArrayAccess3D(T[] array, long width, long height, long depth)
		: this(array, 0, width, height, depth)
		{ }

		public ArrayAccess3D(T[] array, long startIndex, long width, long height, long depth)
		{
			if (array == null)
				throw new ArgumentNullException(nameof(array));

			if (startIndex < 0)
				throw new ArgumentOutOfRangeException(nameof(startIndex), "Non-negative start index expected.");

			_array = array;
			_startIndex = startIndex;
				
			long len = 1;
			
			if (width < 0)
				throw new ArgumentOutOfRangeException(nameof(width), "Non-negative width expected.");
			len *= width;

			
			if (height < 0)
				throw new ArgumentOutOfRangeException(nameof(height), "Non-negative height expected.");
			len *= height;

			
			if (depth < 0)
				throw new ArgumentOutOfRangeException(nameof(depth), "Non-negative depth expected.");
			len *= depth;

			
			if (array.LongLength - startIndex < len)
				throw new ArgumentException("The array's size is too small to hold the amount of entries required by the dimensions.", nameof(array));
			
						Width = width;
						Height = height;
						Depth = depth;
					}

		public T this[long arrayIndex]
		{
			get
			{
				CheckIndex(arrayIndex);
				return _array[arrayIndex + _startIndex];
			}
			set
			{
				CheckIndex(arrayIndex);
				_array[arrayIndex + _startIndex] = value;
			}
		}

		public T this[long x, long y, long z]
		{
			get
			{
				return _array[ComputeIndex(x, y, z)];				
			}
			set
			{
				_array[ComputeIndex(x, y, z)] = value;
			}
		}

		private long ComputeIndex(long x, long y, long z)
		{
			//uninitialized
			if (_array == null)
				throw new ArgumentException("Array access has not been initialized.");

						if (x < 0 || x >= Width)
				throw new ArgumentOutOfRangeException(nameof(x), "x does not lie within its dimension size.");

						if (y < 0 || y >= Height)
				throw new ArgumentOutOfRangeException(nameof(y), "y does not lie within its dimension size.");

						if (z < 0 || z >= Depth)
				throw new ArgumentOutOfRangeException(nameof(z), "z does not lie within its dimension size.");

			
			return _startIndex + x + Width * (y + Height * (z));
		}

		private void CheckIndex(long arrayIndex)
		{
			if (arrayIndex < 0 || arrayIndex >= LongLength)
				throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Array index does not fall into the valid range.");
		}

		//TODO: CopyTo, vector access, etc.
	}
}


namespace Utility.Geometry.Buffers
{
	public struct ArrayAccess4D<T>
	{
		private readonly long _startIndex;
		private readonly T[] _array;

				public long Dimension1 { get; }
				public long Dimension2 { get; }
				public long Dimension3 { get; }
				public long Dimension4 { get; }
		
		public int Length
		{
			get => (int)LongLength;
		}

		public long LongLength
		{
			get => Dimension1 * Dimension2 * Dimension3 * Dimension4;
		}

		public ArrayAccess4D(T[] array, long dimension1, long dimension2, long dimension3, long dimension4)
		: this(array, 0, dimension1, dimension2, dimension3, dimension4)
		{ }

		public ArrayAccess4D(T[] array, long startIndex, long dimension1, long dimension2, long dimension3, long dimension4)
		{
			if (array == null)
				throw new ArgumentNullException(nameof(array));

			if (startIndex < 0)
				throw new ArgumentOutOfRangeException(nameof(startIndex), "Non-negative start index expected.");

			_array = array;
			_startIndex = startIndex;
				
			long len = 1;
			
			if (dimension1 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension1), "Non-negative dimension1 expected.");
			len *= dimension1;

			
			if (dimension2 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension2), "Non-negative dimension2 expected.");
			len *= dimension2;

			
			if (dimension3 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension3), "Non-negative dimension3 expected.");
			len *= dimension3;

			
			if (dimension4 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension4), "Non-negative dimension4 expected.");
			len *= dimension4;

			
			if (array.LongLength - startIndex < len)
				throw new ArgumentException("The array's size is too small to hold the amount of entries required by the dimensions.", nameof(array));
			
						Dimension1 = dimension1;
						Dimension2 = dimension2;
						Dimension3 = dimension3;
						Dimension4 = dimension4;
					}

		public T this[long arrayIndex]
		{
			get
			{
				CheckIndex(arrayIndex);
				return _array[arrayIndex + _startIndex];
			}
			set
			{
				CheckIndex(arrayIndex);
				_array[arrayIndex + _startIndex] = value;
			}
		}

		public T this[long index1, long index2, long index3, long index4]
		{
			get
			{
				return _array[ComputeIndex(index1, index2, index3, index4)];				
			}
			set
			{
				_array[ComputeIndex(index1, index2, index3, index4)] = value;
			}
		}

		private long ComputeIndex(long index1, long index2, long index3, long index4)
		{
			//uninitialized
			if (_array == null)
				throw new ArgumentException("Array access has not been initialized.");

						if (index1 < 0 || index1 >= Dimension1)
				throw new ArgumentOutOfRangeException(nameof(index1), "index1 does not lie within its dimension size.");

						if (index2 < 0 || index2 >= Dimension2)
				throw new ArgumentOutOfRangeException(nameof(index2), "index2 does not lie within its dimension size.");

						if (index3 < 0 || index3 >= Dimension3)
				throw new ArgumentOutOfRangeException(nameof(index3), "index3 does not lie within its dimension size.");

						if (index4 < 0 || index4 >= Dimension4)
				throw new ArgumentOutOfRangeException(nameof(index4), "index4 does not lie within its dimension size.");

			
			return _startIndex + index1 + Dimension1 * (index2 + Dimension2 * (index3 + Dimension3 * (index4)));
		}

		private void CheckIndex(long arrayIndex)
		{
			if (arrayIndex < 0 || arrayIndex >= LongLength)
				throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Array index does not fall into the valid range.");
		}

		//TODO: CopyTo, vector access, etc.
	}
}


namespace Utility.Geometry.Buffers
{
	public struct ArrayAccess5D<T>
	{
		private readonly long _startIndex;
		private readonly T[] _array;

				public long Dimension1 { get; }
				public long Dimension2 { get; }
				public long Dimension3 { get; }
				public long Dimension4 { get; }
				public long Dimension5 { get; }
		
		public int Length
		{
			get => (int)LongLength;
		}

		public long LongLength
		{
			get => Dimension1 * Dimension2 * Dimension3 * Dimension4 * Dimension5;
		}

		public ArrayAccess5D(T[] array, long dimension1, long dimension2, long dimension3, long dimension4, long dimension5)
		: this(array, 0, dimension1, dimension2, dimension3, dimension4, dimension5)
		{ }

		public ArrayAccess5D(T[] array, long startIndex, long dimension1, long dimension2, long dimension3, long dimension4, long dimension5)
		{
			if (array == null)
				throw new ArgumentNullException(nameof(array));

			if (startIndex < 0)
				throw new ArgumentOutOfRangeException(nameof(startIndex), "Non-negative start index expected.");

			_array = array;
			_startIndex = startIndex;
				
			long len = 1;
			
			if (dimension1 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension1), "Non-negative dimension1 expected.");
			len *= dimension1;

			
			if (dimension2 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension2), "Non-negative dimension2 expected.");
			len *= dimension2;

			
			if (dimension3 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension3), "Non-negative dimension3 expected.");
			len *= dimension3;

			
			if (dimension4 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension4), "Non-negative dimension4 expected.");
			len *= dimension4;

			
			if (dimension5 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension5), "Non-negative dimension5 expected.");
			len *= dimension5;

			
			if (array.LongLength - startIndex < len)
				throw new ArgumentException("The array's size is too small to hold the amount of entries required by the dimensions.", nameof(array));
			
						Dimension1 = dimension1;
						Dimension2 = dimension2;
						Dimension3 = dimension3;
						Dimension4 = dimension4;
						Dimension5 = dimension5;
					}

		public T this[long arrayIndex]
		{
			get
			{
				CheckIndex(arrayIndex);
				return _array[arrayIndex + _startIndex];
			}
			set
			{
				CheckIndex(arrayIndex);
				_array[arrayIndex + _startIndex] = value;
			}
		}

		public T this[long index1, long index2, long index3, long index4, long index5]
		{
			get
			{
				return _array[ComputeIndex(index1, index2, index3, index4, index5)];				
			}
			set
			{
				_array[ComputeIndex(index1, index2, index3, index4, index5)] = value;
			}
		}

		private long ComputeIndex(long index1, long index2, long index3, long index4, long index5)
		{
			//uninitialized
			if (_array == null)
				throw new ArgumentException("Array access has not been initialized.");

						if (index1 < 0 || index1 >= Dimension1)
				throw new ArgumentOutOfRangeException(nameof(index1), "index1 does not lie within its dimension size.");

						if (index2 < 0 || index2 >= Dimension2)
				throw new ArgumentOutOfRangeException(nameof(index2), "index2 does not lie within its dimension size.");

						if (index3 < 0 || index3 >= Dimension3)
				throw new ArgumentOutOfRangeException(nameof(index3), "index3 does not lie within its dimension size.");

						if (index4 < 0 || index4 >= Dimension4)
				throw new ArgumentOutOfRangeException(nameof(index4), "index4 does not lie within its dimension size.");

						if (index5 < 0 || index5 >= Dimension5)
				throw new ArgumentOutOfRangeException(nameof(index5), "index5 does not lie within its dimension size.");

			
			return _startIndex + index1 + Dimension1 * (index2 + Dimension2 * (index3 + Dimension3 * (index4 + Dimension4 * (index5))));
		}

		private void CheckIndex(long arrayIndex)
		{
			if (arrayIndex < 0 || arrayIndex >= LongLength)
				throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Array index does not fall into the valid range.");
		}

		//TODO: CopyTo, vector access, etc.
	}
}


namespace Utility.Geometry.Buffers
{
	public struct ArrayAccess6D<T>
	{
		private readonly long _startIndex;
		private readonly T[] _array;

				public long Dimension1 { get; }
				public long Dimension2 { get; }
				public long Dimension3 { get; }
				public long Dimension4 { get; }
				public long Dimension5 { get; }
				public long Dimension6 { get; }
		
		public int Length
		{
			get => (int)LongLength;
		}

		public long LongLength
		{
			get => Dimension1 * Dimension2 * Dimension3 * Dimension4 * Dimension5 * Dimension6;
		}

		public ArrayAccess6D(T[] array, long dimension1, long dimension2, long dimension3, long dimension4, long dimension5, long dimension6)
		: this(array, 0, dimension1, dimension2, dimension3, dimension4, dimension5, dimension6)
		{ }

		public ArrayAccess6D(T[] array, long startIndex, long dimension1, long dimension2, long dimension3, long dimension4, long dimension5, long dimension6)
		{
			if (array == null)
				throw new ArgumentNullException(nameof(array));

			if (startIndex < 0)
				throw new ArgumentOutOfRangeException(nameof(startIndex), "Non-negative start index expected.");

			_array = array;
			_startIndex = startIndex;
				
			long len = 1;
			
			if (dimension1 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension1), "Non-negative dimension1 expected.");
			len *= dimension1;

			
			if (dimension2 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension2), "Non-negative dimension2 expected.");
			len *= dimension2;

			
			if (dimension3 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension3), "Non-negative dimension3 expected.");
			len *= dimension3;

			
			if (dimension4 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension4), "Non-negative dimension4 expected.");
			len *= dimension4;

			
			if (dimension5 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension5), "Non-negative dimension5 expected.");
			len *= dimension5;

			
			if (dimension6 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension6), "Non-negative dimension6 expected.");
			len *= dimension6;

			
			if (array.LongLength - startIndex < len)
				throw new ArgumentException("The array's size is too small to hold the amount of entries required by the dimensions.", nameof(array));
			
						Dimension1 = dimension1;
						Dimension2 = dimension2;
						Dimension3 = dimension3;
						Dimension4 = dimension4;
						Dimension5 = dimension5;
						Dimension6 = dimension6;
					}

		public T this[long arrayIndex]
		{
			get
			{
				CheckIndex(arrayIndex);
				return _array[arrayIndex + _startIndex];
			}
			set
			{
				CheckIndex(arrayIndex);
				_array[arrayIndex + _startIndex] = value;
			}
		}

		public T this[long index1, long index2, long index3, long index4, long index5, long index6]
		{
			get
			{
				return _array[ComputeIndex(index1, index2, index3, index4, index5, index6)];				
			}
			set
			{
				_array[ComputeIndex(index1, index2, index3, index4, index5, index6)] = value;
			}
		}

		private long ComputeIndex(long index1, long index2, long index3, long index4, long index5, long index6)
		{
			//uninitialized
			if (_array == null)
				throw new ArgumentException("Array access has not been initialized.");

						if (index1 < 0 || index1 >= Dimension1)
				throw new ArgumentOutOfRangeException(nameof(index1), "index1 does not lie within its dimension size.");

						if (index2 < 0 || index2 >= Dimension2)
				throw new ArgumentOutOfRangeException(nameof(index2), "index2 does not lie within its dimension size.");

						if (index3 < 0 || index3 >= Dimension3)
				throw new ArgumentOutOfRangeException(nameof(index3), "index3 does not lie within its dimension size.");

						if (index4 < 0 || index4 >= Dimension4)
				throw new ArgumentOutOfRangeException(nameof(index4), "index4 does not lie within its dimension size.");

						if (index5 < 0 || index5 >= Dimension5)
				throw new ArgumentOutOfRangeException(nameof(index5), "index5 does not lie within its dimension size.");

						if (index6 < 0 || index6 >= Dimension6)
				throw new ArgumentOutOfRangeException(nameof(index6), "index6 does not lie within its dimension size.");

			
			return _startIndex + index1 + Dimension1 * (index2 + Dimension2 * (index3 + Dimension3 * (index4 + Dimension4 * (index5 + Dimension5 * (index6)))));
		}

		private void CheckIndex(long arrayIndex)
		{
			if (arrayIndex < 0 || arrayIndex >= LongLength)
				throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Array index does not fall into the valid range.");
		}

		//TODO: CopyTo, vector access, etc.
	}
}


namespace Utility.Geometry.Buffers
{
	public struct ArrayAccess7D<T>
	{
		private readonly long _startIndex;
		private readonly T[] _array;

				public long Dimension1 { get; }
				public long Dimension2 { get; }
				public long Dimension3 { get; }
				public long Dimension4 { get; }
				public long Dimension5 { get; }
				public long Dimension6 { get; }
				public long Dimension7 { get; }
		
		public int Length
		{
			get => (int)LongLength;
		}

		public long LongLength
		{
			get => Dimension1 * Dimension2 * Dimension3 * Dimension4 * Dimension5 * Dimension6 * Dimension7;
		}

		public ArrayAccess7D(T[] array, long dimension1, long dimension2, long dimension3, long dimension4, long dimension5, long dimension6, long dimension7)
		: this(array, 0, dimension1, dimension2, dimension3, dimension4, dimension5, dimension6, dimension7)
		{ }

		public ArrayAccess7D(T[] array, long startIndex, long dimension1, long dimension2, long dimension3, long dimension4, long dimension5, long dimension6, long dimension7)
		{
			if (array == null)
				throw new ArgumentNullException(nameof(array));

			if (startIndex < 0)
				throw new ArgumentOutOfRangeException(nameof(startIndex), "Non-negative start index expected.");

			_array = array;
			_startIndex = startIndex;
				
			long len = 1;
			
			if (dimension1 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension1), "Non-negative dimension1 expected.");
			len *= dimension1;

			
			if (dimension2 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension2), "Non-negative dimension2 expected.");
			len *= dimension2;

			
			if (dimension3 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension3), "Non-negative dimension3 expected.");
			len *= dimension3;

			
			if (dimension4 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension4), "Non-negative dimension4 expected.");
			len *= dimension4;

			
			if (dimension5 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension5), "Non-negative dimension5 expected.");
			len *= dimension5;

			
			if (dimension6 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension6), "Non-negative dimension6 expected.");
			len *= dimension6;

			
			if (dimension7 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension7), "Non-negative dimension7 expected.");
			len *= dimension7;

			
			if (array.LongLength - startIndex < len)
				throw new ArgumentException("The array's size is too small to hold the amount of entries required by the dimensions.", nameof(array));
			
						Dimension1 = dimension1;
						Dimension2 = dimension2;
						Dimension3 = dimension3;
						Dimension4 = dimension4;
						Dimension5 = dimension5;
						Dimension6 = dimension6;
						Dimension7 = dimension7;
					}

		public T this[long arrayIndex]
		{
			get
			{
				CheckIndex(arrayIndex);
				return _array[arrayIndex + _startIndex];
			}
			set
			{
				CheckIndex(arrayIndex);
				_array[arrayIndex + _startIndex] = value;
			}
		}

		public T this[long index1, long index2, long index3, long index4, long index5, long index6, long index7]
		{
			get
			{
				return _array[ComputeIndex(index1, index2, index3, index4, index5, index6, index7)];				
			}
			set
			{
				_array[ComputeIndex(index1, index2, index3, index4, index5, index6, index7)] = value;
			}
		}

		private long ComputeIndex(long index1, long index2, long index3, long index4, long index5, long index6, long index7)
		{
			//uninitialized
			if (_array == null)
				throw new ArgumentException("Array access has not been initialized.");

						if (index1 < 0 || index1 >= Dimension1)
				throw new ArgumentOutOfRangeException(nameof(index1), "index1 does not lie within its dimension size.");

						if (index2 < 0 || index2 >= Dimension2)
				throw new ArgumentOutOfRangeException(nameof(index2), "index2 does not lie within its dimension size.");

						if (index3 < 0 || index3 >= Dimension3)
				throw new ArgumentOutOfRangeException(nameof(index3), "index3 does not lie within its dimension size.");

						if (index4 < 0 || index4 >= Dimension4)
				throw new ArgumentOutOfRangeException(nameof(index4), "index4 does not lie within its dimension size.");

						if (index5 < 0 || index5 >= Dimension5)
				throw new ArgumentOutOfRangeException(nameof(index5), "index5 does not lie within its dimension size.");

						if (index6 < 0 || index6 >= Dimension6)
				throw new ArgumentOutOfRangeException(nameof(index6), "index6 does not lie within its dimension size.");

						if (index7 < 0 || index7 >= Dimension7)
				throw new ArgumentOutOfRangeException(nameof(index7), "index7 does not lie within its dimension size.");

			
			return _startIndex + index1 + Dimension1 * (index2 + Dimension2 * (index3 + Dimension3 * (index4 + Dimension4 * (index5 + Dimension5 * (index6 + Dimension6 * (index7))))));
		}

		private void CheckIndex(long arrayIndex)
		{
			if (arrayIndex < 0 || arrayIndex >= LongLength)
				throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Array index does not fall into the valid range.");
		}

		//TODO: CopyTo, vector access, etc.
	}
}


namespace Utility.Geometry.Buffers
{
	public struct ArrayAccess8D<T>
	{
		private readonly long _startIndex;
		private readonly T[] _array;

				public long Dimension1 { get; }
				public long Dimension2 { get; }
				public long Dimension3 { get; }
				public long Dimension4 { get; }
				public long Dimension5 { get; }
				public long Dimension6 { get; }
				public long Dimension7 { get; }
				public long Dimension8 { get; }
		
		public int Length
		{
			get => (int)LongLength;
		}

		public long LongLength
		{
			get => Dimension1 * Dimension2 * Dimension3 * Dimension4 * Dimension5 * Dimension6 * Dimension7 * Dimension8;
		}

		public ArrayAccess8D(T[] array, long dimension1, long dimension2, long dimension3, long dimension4, long dimension5, long dimension6, long dimension7, long dimension8)
		: this(array, 0, dimension1, dimension2, dimension3, dimension4, dimension5, dimension6, dimension7, dimension8)
		{ }

		public ArrayAccess8D(T[] array, long startIndex, long dimension1, long dimension2, long dimension3, long dimension4, long dimension5, long dimension6, long dimension7, long dimension8)
		{
			if (array == null)
				throw new ArgumentNullException(nameof(array));

			if (startIndex < 0)
				throw new ArgumentOutOfRangeException(nameof(startIndex), "Non-negative start index expected.");

			_array = array;
			_startIndex = startIndex;
				
			long len = 1;
			
			if (dimension1 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension1), "Non-negative dimension1 expected.");
			len *= dimension1;

			
			if (dimension2 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension2), "Non-negative dimension2 expected.");
			len *= dimension2;

			
			if (dimension3 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension3), "Non-negative dimension3 expected.");
			len *= dimension3;

			
			if (dimension4 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension4), "Non-negative dimension4 expected.");
			len *= dimension4;

			
			if (dimension5 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension5), "Non-negative dimension5 expected.");
			len *= dimension5;

			
			if (dimension6 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension6), "Non-negative dimension6 expected.");
			len *= dimension6;

			
			if (dimension7 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension7), "Non-negative dimension7 expected.");
			len *= dimension7;

			
			if (dimension8 < 0)
				throw new ArgumentOutOfRangeException(nameof(dimension8), "Non-negative dimension8 expected.");
			len *= dimension8;

			
			if (array.LongLength - startIndex < len)
				throw new ArgumentException("The array's size is too small to hold the amount of entries required by the dimensions.", nameof(array));
			
						Dimension1 = dimension1;
						Dimension2 = dimension2;
						Dimension3 = dimension3;
						Dimension4 = dimension4;
						Dimension5 = dimension5;
						Dimension6 = dimension6;
						Dimension7 = dimension7;
						Dimension8 = dimension8;
					}

		public T this[long arrayIndex]
		{
			get
			{
				CheckIndex(arrayIndex);
				return _array[arrayIndex + _startIndex];
			}
			set
			{
				CheckIndex(arrayIndex);
				_array[arrayIndex + _startIndex] = value;
			}
		}

		public T this[long index1, long index2, long index3, long index4, long index5, long index6, long index7, long index8]
		{
			get
			{
				return _array[ComputeIndex(index1, index2, index3, index4, index5, index6, index7, index8)];				
			}
			set
			{
				_array[ComputeIndex(index1, index2, index3, index4, index5, index6, index7, index8)] = value;
			}
		}

		private long ComputeIndex(long index1, long index2, long index3, long index4, long index5, long index6, long index7, long index8)
		{
			//uninitialized
			if (_array == null)
				throw new ArgumentException("Array access has not been initialized.");

						if (index1 < 0 || index1 >= Dimension1)
				throw new ArgumentOutOfRangeException(nameof(index1), "index1 does not lie within its dimension size.");

						if (index2 < 0 || index2 >= Dimension2)
				throw new ArgumentOutOfRangeException(nameof(index2), "index2 does not lie within its dimension size.");

						if (index3 < 0 || index3 >= Dimension3)
				throw new ArgumentOutOfRangeException(nameof(index3), "index3 does not lie within its dimension size.");

						if (index4 < 0 || index4 >= Dimension4)
				throw new ArgumentOutOfRangeException(nameof(index4), "index4 does not lie within its dimension size.");

						if (index5 < 0 || index5 >= Dimension5)
				throw new ArgumentOutOfRangeException(nameof(index5), "index5 does not lie within its dimension size.");

						if (index6 < 0 || index6 >= Dimension6)
				throw new ArgumentOutOfRangeException(nameof(index6), "index6 does not lie within its dimension size.");

						if (index7 < 0 || index7 >= Dimension7)
				throw new ArgumentOutOfRangeException(nameof(index7), "index7 does not lie within its dimension size.");

						if (index8 < 0 || index8 >= Dimension8)
				throw new ArgumentOutOfRangeException(nameof(index8), "index8 does not lie within its dimension size.");

			
			return _startIndex + index1 + Dimension1 * (index2 + Dimension2 * (index3 + Dimension3 * (index4 + Dimension4 * (index5 + Dimension5 * (index6 + Dimension6 * (index7 + Dimension7 * (index8)))))));
		}

		private void CheckIndex(long arrayIndex)
		{
			if (arrayIndex < 0 || arrayIndex >= LongLength)
				throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Array index does not fall into the valid range.");
		}

		//TODO: CopyTo, vector access, etc.
	}
}

