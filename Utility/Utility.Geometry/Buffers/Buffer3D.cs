using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Geometry.Buffers
{
    public abstract class Buffer3D<T> : IBuffer<T, (int, int, int)>
    {
        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract int Depth { get; }
        public abstract bool IsReadOnly { get; }
        public abstract T this[int x, int y, int z] { get; set; }

        public virtual ArrayBuffer3D<T> AsArrayBuffer()
        {
            var b = new ArrayBuffer3D<T>(Width, Height, Depth);

            for (int z = 0; z < Depth; z++)
                for (int y = 0; y < Height; y++)
                    for (int x = 0; x < Width; x++)
                        b[x, y, z] = this[x, y, z];

            return b;
        }

        int IBuffer<T>.Dimensions => 3;

        T IBuffer<T, (int, int, int)>.this[(int, int, int) index]
        {
            get => this[index.Item1, index.Item2, index.Item3];
            set => this[index.Item1, index.Item2, index.Item3] = value;
        }

        int IBuffer<T>.GetLength(int dimension)
        {
            if (dimension == 0)
                return Width;
            else if (dimension == 1)
                return Height;
            else if (dimension == 2)
                return Depth;
            else
                throw new ArgumentOutOfRangeException(nameof(dimension), "The dimension does not exist for the current buffer.");
        }

        T IBuffer<T>.Get(params int[] index)
        {
            if (index == null)
                throw new ArgumentNullException(nameof(index));

            if (index.Length != 3)
                throw new ArgumentException("Length of index is not equal to the dimension count.");

            return this[index[0], index[1], index[2]];
        }
    }
}
