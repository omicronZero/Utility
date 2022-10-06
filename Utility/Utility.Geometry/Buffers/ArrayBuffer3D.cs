using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Geometry.Buffers
{
    public class ArrayBuffer3D<T> : Buffer3D<T>
    {
        private readonly T[] _buffer;
        private readonly int _width, _height, _depth;

        public sealed override int Width => _width;
        public sealed override int Height => _height;
        public sealed override int Depth  => _depth;

        public override bool IsReadOnly => false;

        public ArrayBuffer3D(int width, int height, int depth)
        {
            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width), width, "Non-negative width expected.");
            if (height < 0)
                throw new ArgumentOutOfRangeException(nameof(height), height, "Non-negative height expected.");
            if (depth < 0)
                throw new ArgumentOutOfRangeException(nameof(depth), depth, "Non-negative depth expected.");

            _buffer = new T[width * height * depth];

            _width = width;
            _height = height;
            _depth = depth;
        }

        public ArrayBuffer3D(T[,,] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            _width = buffer.GetLength(0);
            _height = buffer.GetLength(1);
            _depth = buffer.GetLength(2);

            _buffer = new T[_width * _height * _depth];

            //TODO: copy
        }

        public ArrayBuffer3D(ArrayBuffer3D<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            _buffer = (T[])source._buffer.Clone();
            _width = source._width;
            _height = source._height;
        }

        public override ArrayBuffer3D<T> AsArrayBuffer()
        {
            return this;
        }

        private void CheckIndex(int x, int y, int z)
        {
            if (x < 0 || x >= _width)
                throw new ArgumentOutOfRangeException(nameof(x), x, "Coordinate does not fall into the buffer's boundaries.");
            if (y < 0 || y >= _height)
                throw new ArgumentOutOfRangeException(nameof(y), y, "Coordinate does not fall into the buffer's boundaries.");
            if (z < 0 || z >= _depth)
                throw new ArgumentOutOfRangeException(nameof(z), z, "Coordinate does not fall into the buffer's boundaries.");
        }

        public override T this[int x, int y, int z]
        {
            get
            {
                CheckIndex(x, y, z);

                return _buffer[x + y * _width + z * _width * _height];
            }
            set
            {
                if (IsReadOnly)
                    throw new NotSupportedException("The buffer is read-only.");

                CheckIndex(x, y, z);

                _buffer[x + y * _width + z * _width * _height] = value;
            }
        }
    }
}
