using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Geometry.Buffers
{
    public class ArrayBuffer2D<T> : Buffer2D<T>
    {
        private readonly T[] _buffer;
        private readonly int _width, _height;

        public override int Width => _width;
        public override int Height => _height;

        public override bool IsReadOnly => false;

        public ArrayBuffer2D(int width, int height)
        {
            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width), width, "Non-negative width expected.");
            if (height < 0)
                throw new ArgumentOutOfRangeException(nameof(height), height, "Non-negative height expected.");

            _buffer = new T[width * height];
            _width = width;
            _height = height;
        }

        public ArrayBuffer2D(T[,] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            _width = buffer.GetLength(0);
            _height = buffer.GetLength(1);

            _buffer = new T[_width * _height];

            //TODO: copy
        }

        public ArrayBuffer2D(ArrayBuffer2D<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            _buffer = (T[])source._buffer.Clone();
            _width = source._width;
            _height = source._height;
        }

        public override ArrayBuffer2D<T> AsArrayBuffer()
        {
            return this;
        }

        private void CheckIndex(int x, int y)
        {
            if (x < 0 || x >= _width)
                throw new ArgumentOutOfRangeException(nameof(x), x, "Coordinate does not fall into the buffer's boundaries.");
            if (y < 0 || y >= _height)
                throw new ArgumentOutOfRangeException(nameof(y), y, "Coordinate does not fall into the buffer's boundaries.");
        }

        public override T this[int x, int y]
        {
            get
            {
                CheckIndex(x, y);

                return _buffer[x + y * _width];
            }
            set
            {
                if (IsReadOnly)
                    throw new NotSupportedException("The buffer is read-only.");

                CheckIndex(x, y);

                _buffer[x + y * _width] = value;
            }
        }
    }
}
