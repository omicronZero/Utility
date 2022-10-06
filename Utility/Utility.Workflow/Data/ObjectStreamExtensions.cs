using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Data
{
    public static class ObjectStreamExtensions
    {
        public static ObjectStream<T> Convert<T, TUnderlying>(this ObjectStream<TUnderlying> underlyingStream, Func<TUnderlying, T> readConverter, Func<T, TUnderlying> writeConverter, int bufferSize, bool propagateDispose)
        {
            if (underlyingStream == null)
                throw new ArgumentNullException(nameof(underlyingStream));

            if (readConverter == null && writeConverter == null)
                throw new ArgumentException("At least reading or writing must be supported.");

            if (readConverter == null)
            {
                return Convert(underlyingStream, writeConverter, bufferSize, propagateDispose);
            }
            else if (writeConverter == null)
            {
                return Convert(underlyingStream, readConverter, bufferSize, propagateDispose);
            }
            else
            {
                return new ConverterStream<T, TUnderlying>(underlyingStream, readConverter, writeConverter, bufferSize, propagateDispose);
            }
        }

        public static ReadonlyObjectStream<T> Convert<T, TUnderlying>(this ObjectStream<TUnderlying> underlyingStream, Func<TUnderlying, T> readConverter, int bufferSize, bool propagateDispose)
        {
            if (underlyingStream == null)
                throw new ArgumentNullException(nameof(underlyingStream));

            if (readConverter == null)
                throw new ArgumentNullException(nameof(readConverter));

            return new ConverterReadonlyStream<T, TUnderlying>(underlyingStream, readConverter, bufferSize, propagateDispose);
        }

        public static WriteonlyObjectStream<T> Convert<T, TUnderlying>(this ObjectStream<TUnderlying> underlyingStream, Func<T, TUnderlying> writeConverter, int bufferSize, bool propagateDispose)
        {
            if (underlyingStream == null)
                throw new ArgumentNullException(nameof(underlyingStream));

            if (writeConverter == null)
                throw new ArgumentNullException(nameof(writeConverter));

            return new ConverterWriteonlyStream<T, TUnderlying>(underlyingStream, writeConverter, bufferSize, propagateDispose);
        }

        private sealed class ConverterStream<T, TUnderlying> : ObjectStream<T>
        {
            private readonly ObjectStream<TUnderlying> _underlyingStream;

            private Func<TUnderlying, T> _readConverter;
            private Func<T, TUnderlying> _writeConverter;
            private TUnderlying[] _underlyingBuffer;

            private readonly bool _propagateDispose;

            public ConverterStream(ObjectStream<TUnderlying> underlyingStream, Func<TUnderlying, T> readConverter, Func<T, TUnderlying> writeConverter, int bufferSize, bool propagateDispose)
            {
                if (underlyingStream == null)
                    throw new ArgumentNullException(nameof(underlyingStream));

                _underlyingStream = underlyingStream;
                _readConverter = readConverter;
                _writeConverter = writeConverter;
                _propagateDispose = propagateDispose;
                _underlyingBuffer = new TUnderlying[bufferSize];
            }

            public override bool CanRead => _underlyingStream.CanRead && _readConverter != null;

            public override bool CanWrite => _underlyingStream.CanWrite && _writeConverter != null;

            public override bool CanSeek => _underlyingStream.CanSeek;

            public override long Position
            {
                get => _underlyingStream.Position;
                set => _underlyingStream.Position = value;
            }

            public override long Length => _underlyingStream.Length;

            protected override int ReadCore(Span<T> buffer)
            {
                int c = 0;
                int index = 0;
                int count = buffer.Length;

                while (count > 0)
                {
                    int tc = _underlyingStream.Read(_underlyingBuffer, 0, Math.Min(count, _underlyingBuffer.Length));

                    for (int i = 0; i < tc; i++)
                        buffer[index + i] = _readConverter(_underlyingBuffer[i]);

                    index += tc;
                    count -= tc;
                    c += tc;
                }

                return c;
            }

            protected override void WriteCore(ReadOnlySpan<T> buffer)
            {
                int count = buffer.Length;
                int index = 0;

                while (count > 0)
                {
                    int tc = Math.Min(count, _underlyingBuffer.Length);

                    for (int i = 0; i < tc; i++)
                    {
                        _underlyingBuffer[i] = _writeConverter(buffer[index++]);
                    }

                    if (_underlyingBuffer.Length != tc)
                        Array.Clear(_underlyingBuffer, tc, _underlyingBuffer.Length - tc);

                    count -= tc;
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (_propagateDispose)
                {
                    _underlyingStream.Dispose();
                }
                _underlyingBuffer = null;
                _readConverter = null;
                _writeConverter = null;

                base.Dispose(disposing);
            }
        }

        private sealed class ConverterReadonlyStream<T, TUnderlying> : ReadonlyObjectStream<T>
        {
            private readonly ObjectStream<TUnderlying> _underlyingStream;

            private Func<TUnderlying, T> _readConverter;
            private TUnderlying[] _underlyingBuffer;

            private readonly bool _propagateDispose;

            public ConverterReadonlyStream(ObjectStream<TUnderlying> underlyingStream, Func<TUnderlying, T> readConverter, int bufferSize, bool propagateDispose)
            {
                if (underlyingStream == null)
                    throw new ArgumentNullException(nameof(underlyingStream));

                _underlyingStream = underlyingStream;
                _readConverter = readConverter;
                _propagateDispose = propagateDispose;
                _underlyingBuffer = new TUnderlying[bufferSize];
            }

            public override bool CanSeek => _underlyingStream.CanSeek;
            public override bool CanRead => _underlyingStream.CanRead;

            public override long Position
            {
                get => _underlyingStream.Position;
                set => _underlyingStream.Position = value;
            }

            public override long Length => _underlyingStream.Length;

            protected override int ReadCore(Span<T> buffer)
            {
                int c = 0;
                int count = buffer.Length;
                int index = 0;

                while (count > 0)
                {
                    int tc = _underlyingStream.Read(_underlyingBuffer, 0, Math.Min(count, _underlyingBuffer.Length));

                    for (int i = 0; i < tc; i++)
                        buffer[index + i] = _readConverter(_underlyingBuffer[i]);

                    index += tc;
                    count -= tc;
                    c += tc;
                }

                return c;
            }

            protected override void Dispose(bool disposing)
            {
                if (_propagateDispose)
                {
                    _underlyingStream.Dispose();
                }
                _underlyingBuffer = null;
                _readConverter = null;

                base.Dispose(disposing);
            }
        }

        private sealed class ConverterWriteonlyStream<T, TUnderlying> : WriteonlyObjectStream<T>
        {
            private readonly ObjectStream<TUnderlying> _underlyingStream;

            private Func<T, TUnderlying> _writeConverter;
            private TUnderlying[] _underlyingBuffer;

            private readonly bool _propagateDispose;

            public ConverterWriteonlyStream(ObjectStream<TUnderlying> underlyingStream, Func<T, TUnderlying> writeConverter, int bufferSize, bool propagateDispose)
            {
                if (underlyingStream == null)
                    throw new ArgumentNullException(nameof(underlyingStream));

                _underlyingStream = underlyingStream;
                _writeConverter = writeConverter;
                _propagateDispose = propagateDispose;
                _underlyingBuffer = new TUnderlying[bufferSize];
            }

            public override bool CanSeek => _underlyingStream.CanSeek;
            public override bool CanWrite => _underlyingStream.CanWrite;

            public override long Position
            {
                get => _underlyingStream.Position;
                set => _underlyingStream.Position = value;
            }

            public override long Length => _underlyingStream.Length;

            protected override void WriteCore(ReadOnlySpan<T> buffer)
            {
                int count = buffer.Length;
                int index = 0;

                while (count > 0)
                {
                    int tc = Math.Min(count, _underlyingBuffer.Length);

                    for (int i = 0; i < tc; i++)
                    {
                        _underlyingBuffer[i] = _writeConverter(buffer[index++]);
                    }

                    if (_underlyingBuffer.Length != tc)
                        Array.Clear(_underlyingBuffer, tc, _underlyingBuffer.Length - tc);

                    count -= tc;
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (_propagateDispose)
                {
                    _underlyingStream.Dispose();
                }
                _underlyingBuffer = null;
                _writeConverter = null;

                base.Dispose(disposing);
            }
        }
    }
}
