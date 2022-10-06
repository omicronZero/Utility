using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Geometry.Buffers
{
    public abstract class Buffer2D<T> : IBuffer<T, (int, int)>
    {
        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract bool IsReadOnly { get; }
        public abstract T this[int x, int y] { get; set; }

        public virtual ArrayBuffer2D<T> AsArrayBuffer()
        {
            var b = new ArrayBuffer2D<T>(Width, Height);

            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    b[x, y] = this[x, y];

            return b;
        }

        int IBuffer<T>.Dimensions => 2;

        T IBuffer<T, (int, int)>.this[(int, int) index]
        {
            get => this[index.Item1, index.Item2];
            set => this[index.Item1, index.Item2] = value;
        }

        int IBuffer<T>.GetLength(int dimension)
        {
            if (dimension == 0)
                return Width;
            else if (dimension == 1)
                return Height;
            else
                throw new ArgumentOutOfRangeException(nameof(dimension), "The dimension does not exist for the current buffer.");
        }

        T IBuffer<T>.Get(params int[] index)
        {
            if (index == null)
                throw new ArgumentNullException(nameof(index));

            if (index.Length != 2)
                throw new ArgumentException("Length of index is not equal to the dimension count.");

            return this[index[0], index[1]];
        }
    }
}
