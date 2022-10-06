using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Utility.Data
{
    public unsafe partial class DataStream : Stream
    {
        public static int DefaultBufferSize { get; } = 0x10000;

        private readonly Stream _underlyingStream;
        private readonly byte[] _buffer;
        private bool _disposeUnderlyingStream;

        public DataStream(Stream underlyingStream)
            : this(underlyingStream, DefaultBufferSize, false)
        { }

        public DataStream(Stream underlyingStream, bool disposeUnderlyingStream)
            : this(underlyingStream, DefaultBufferSize, disposeUnderlyingStream)
        { }

        public DataStream(Stream underlyingStream, int bufferSize)
            : this(underlyingStream, bufferSize, false)
        { }

        public DataStream(Stream underlyingStream, int bufferSize, bool disposeUnderlyingStream)
        {
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize), bufferSize, "Positive buffer size expected.");

            _underlyingStream = underlyingStream;
            _buffer = new byte[bufferSize];
            _disposeUnderlyingStream = disposeUnderlyingStream;
        }

        public override bool CanRead => _underlyingStream.CanRead;
        public override bool CanWrite => _underlyingStream.CanWrite;
        public override bool CanSeek => _underlyingStream.CanSeek;

        public override long Length => _underlyingStream.Length;

        public override long Position
        {
            get => _underlyingStream.Position;
            set => _underlyingStream.Position = value;
        }

        public override void Flush()
        {
            _underlyingStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _underlyingStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _underlyingStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _underlyingStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _underlyingStream.Write(buffer, offset, count);
        }

        public unsafe void Write(void* pointer, int byteCount)
        {
            if (pointer == null)
                throw new ArgumentException("Pointer must not be null.", nameof(pointer));
            if (byteCount < 0)
                throw new ArgumentOutOfRangeException(nameof(byteCount), byteCount, "Non-negative byte count expected.");

            Write(new IntPtr(pointer), byteCount);
        }

        public unsafe void Write(void* pointer, int byteOffset, int byteCount)
        {
            Write((byte*)pointer + byteOffset, byteCount);
        }

        public override int ReadByte()
        {
            if (Read(_buffer, 0, 1) == 0)
                return -1;

            return _buffer[0];
        }

        public AssembledObject<byte> ReadOneByte()
        {
            return ReadBytes(1);
        }

        public AssembledObject<byte> ReadBytes(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, "Non-negative count expected.");

            return new AssembledObject<byte>(count);
        }

        public void ReadAssembled<T>(AssembledObject<T> instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            int c = Read(_buffer, 0, Math.Min(_buffer.Length, instance.RemainingBytes));
            instance.Write(_buffer, 0, c);
        }

        public T ReadAssembledSingletonComplete<T>(AssembledObject<T> instance)
        {
            return ReadAssembledSingletonComplete(instance, YieldRead);
        }

        public T ReadAssembledSingletonComplete<T>(AssembledObject<T> instance, Func<bool> continueAction)
        {
            Complete(instance, continueAction);

            return instance.AsSingleton();
        }

        public T ReadAssembledArrayComplete<T>(AssembledObject<T> instance)
        {
            return ReadAssembledSingletonComplete(instance, YieldRead);
        }

        public T[] ReadAssembledArrayComplete<T>(AssembledObject<T> instance, Func<bool> continueAction)
        {
            Complete(instance, continueAction);

            return instance.AsArray();
        }

        private void Complete<T>(AssembledObject<T> instance, Func<bool> continueAction)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            if (instance.IsComplete)
                return;

            while (true)
            {
                ReadAssembled(instance);

                if (instance.IsComplete)
                    break;

                if (!(continueAction?.Invoke() ?? true))
                    throw new InvalidOperationException("Object has not been completed.");
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_disposeUnderlyingStream)
                _underlyingStream.Dispose();
        }

        public void Write(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            Write(buffer, 0, buffer.Length);
        }

        public void Write(byte value)
        {
            _buffer[0] = value;
            Write(_buffer, 0, 1);
        }

        private void Write(IntPtr ptr, int count)
        {
            if (ptr == IntPtr.Zero)
                throw new ArgumentException("Pointer must not be zero.", nameof(ptr));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, "Non-negative count expected.");

            while (count > 0)
            {
                int c = Math.Min(count, _buffer.Length);

                Marshal.Copy(ptr, _buffer, 0, c);
                Write(_buffer, 0, c);

                count -= c;
                ptr += c;
            }
        }

        public void Write(string chars)
        {
            if (chars == null)
                throw new ArgumentNullException(nameof(chars));

            Write(chars, 0, chars.Length);
        }

        public void Write(string chars, int index, int count)
        {
            if (chars == null)
                throw new ArgumentNullException(nameof(chars));

            Util.ValidateNamedRange(index, count, chars.Length);

            fixed (char* c = chars)
                Write(new IntPtr(c + count), count * sizeof(char));
        }

        public static DataStream AsDataStream(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            return AsDataStream(stream, DefaultBufferSize, false);
        }

        public static DataStream AsDataStream(Stream stream, bool constructDisposeUnderlyingStream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            return AsDataStream(stream, DefaultBufferSize, false);
        }

        public static DataStream AsDataStream(Stream stream, int constructBufferSize)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            return AsDataStream(stream, DefaultBufferSize, false);
        }

        public static DataStream AsDataStream(Stream stream, int constructBufferSize, bool constructDisposeUnderlyingStream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (constructBufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(constructBufferSize), constructBufferSize, "Positive buffer size expected.");

            return stream as DataStream ?? new DataStream(stream, constructBufferSize, constructDisposeUnderlyingStream);
        }

        private static bool YieldRead()
        {
            System.Threading.Thread.Yield();
            return true;
        }
    }
}
