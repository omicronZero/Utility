using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Serialization.Serializers
{
    public class ObjectBinaryReader : IObjectReader
    {
        private byte[] _buffer;
        private readonly Stream _underlyingStream;

        public T Read<T>()
        {
            int size = TypeExtensions.UnmanagedSize<T>() ?? throw new ArgumentException($"Type { typeof(T).FullName } is not supported.");

            if (size > _buffer.Length)
                Array.Resize(ref _buffer, size);

            int count = 0;
            int c;

            //we will read until either c == 0 (end of stream) or size == count (finished)
            do
            {
                c = _underlyingStream.Read(_buffer, 0, size - count);

                count += c;

                if (c < 0)
                    throw new ArgumentException("Non-negative value expected as the number of bytes read from the underlying stream.");
                if (count > size)
                    throw new ArgumentException("Too many bytes returned from the underlying stream.");

                if (count == size)
                    break;
            } while (c != 0);

            //catch end of stream when unfinished
            if (count < size)
                throw new IOException("The end of the stream was reached before the object was finished.");

            return BinaryHelper<T>.GetObject(_buffer, 0);
        }

        public ObjectBinaryReader(Stream sourceStream, int initialBufferCapacity = 256)
        {
            if (initialBufferCapacity < 0)
                throw new ArgumentOutOfRangeException(nameof(initialBufferCapacity));

            _underlyingStream = sourceStream ?? throw new ArgumentNullException(nameof(sourceStream));
            _buffer = new byte[initialBufferCapacity];
        }
    }
}
