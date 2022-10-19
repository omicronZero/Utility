using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Serialization.Serializers
{
    public class ObjectBinaryWriter : IObjectWriter
    {
        private readonly Stream _underlyingStream;

        private byte[] _buffer;

        private IObjectWriter? _innerWriter;

        protected IObjectWriter StreamWriter { get; }

        public ObjectBinaryWriter(Stream targetStream, int initialBufferCapacity = 256)
        {
            if (initialBufferCapacity < 0)
                throw new ArgumentOutOfRangeException(nameof(initialBufferCapacity));

            _underlyingStream = targetStream ?? throw new ArgumentNullException(nameof(targetStream));
            _buffer = new byte[initialBufferCapacity];
            StreamWriter = new StreamWriterImpl(this);
        }

        protected virtual IObjectWriter CreateInnerWriter()
        {
            return new LeafWriter(new LateObjectWriter(StreamWriter));
        }

        protected void WriteToStream<T>(T instance)
        {
            int size = TypeExtensions.UnmanagedSize<T>() ?? throw new ArgumentException($"Type { typeof(T).FullName } is not supported.");

            if (size > _buffer.Length)
                Array.Resize(ref _buffer, size);

            BinaryHelper<T>.GetData(_buffer, 0, instance);

            _underlyingStream.Write(_buffer);
        }

        public void Write<T>(T instance)
        {
            if (_innerWriter == null)
                _innerWriter = CreateInnerWriter() ?? throw new InvalidOperationException("Null not an allowed return value of CreateInnerWriter.");

            _innerWriter.Write(instance);
        }

        private sealed class StreamWriterImpl : IObjectWriter
        {
            private readonly ObjectBinaryWriter _binaryWriter;

            public StreamWriterImpl(ObjectBinaryWriter binaryWriter)
            {
                _binaryWriter = binaryWriter ?? throw new ArgumentNullException(nameof(binaryWriter));
            }

            public void Write<T>(T instance)
            {
                _binaryWriter.WriteToStream(instance);
            }
        }
    }
}
