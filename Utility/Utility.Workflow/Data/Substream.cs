using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Utility.Data
{
    public class Substream : Stream
    {
        private readonly Stream _underlyingStream;
        private long _length;
        private long _startOffset;
        private long _position;
        private Flags _flags;
        private int _readTimeout;
        private int _writeTimeout;

        public Substream(
                Stream underlyingStream,
                long startOffset,
                long length,
                bool resetPosition,
                bool allowRead,
                bool allowWrite,
                bool fixedSizeStream,
                bool disposeUnderlyingStream,
                bool overrideTimeout
            )
        {
            if (underlyingStream == null)
                throw new ArgumentNullException(nameof(underlyingStream));

            if (!underlyingStream.CanSeek)
                throw new ArgumentException("Seekable stream expected.", nameof(underlyingStream));

            if (length < 0)
                throw new ArgumentException("Non-negative length expected.", nameof(length));

            if (!(allowRead || allowWrite))
                throw new ArgumentException("At least reading or writing must be supported by the stream.");

            if (overrideTimeout && _underlyingStream.CanTimeout)
                throw new ArgumentException("Timeout not supported by underlying stream.", nameof(overrideTimeout));

            _underlyingStream = underlyingStream;

            SubstreamOffset = startOffset;
            _length = length;

            _flags = (allowRead ? Flags.Read : 0)
                ^ (allowWrite ? Flags.Write : 0)
                ^ (resetPosition ? Flags.ResetPosition : 0)
                ^ (fixedSizeStream ? Flags.FixedSize : 0)
                ^ (disposeUnderlyingStream ? Flags.DisposeUnderlyingStream : 0)
                ^ (overrideTimeout ? Flags.OverrideTimeout : 0);
        }

        public override bool CanRead => (_flags & Flags.Read) == Flags.Read;
        public override bool CanWrite => (_flags & Flags.Write) == Flags.Write;
        public override bool CanSeek => true;

        public long SubstreamOffset
        {
            get { return _startOffset; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Non-negative substream offset expected.", nameof(value));

                //preserves absolute position
                _position += _startOffset - value;

                _startOffset = value;
            }
        }

        public long SubstreamLength
        {
            get { return _length; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Non-negative length expected.", nameof(value));

                _length = value;
            }
        }

        public bool ResetPosition
        {
            get { return (_flags & Flags.ResetPosition) == Flags.ResetPosition; }
        }

        public bool DisposeUnderlyingStream
        {
            get { return (_flags & Flags.DisposeUnderlyingStream) == Flags.DisposeUnderlyingStream; }
        }

        public bool FixedSize
        {
            get { return (_flags & Flags.FixedSize) == Flags.FixedSize; }
        }

        public override long Length => Math.Max(0, Math.Min(_underlyingStream.Length, SubstreamOffset + SubstreamLength) - SubstreamLength);

        public override long Position
        {
            get => _position;
            set => _position = value;
        }

        public override void Flush()
        {
            _underlyingStream.Flush();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("Setting the length of the stream is not supported.");
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.Begin)
                Position = offset;
            else if (origin == SeekOrigin.Current)
                Position += offset;
            else if (origin == SeekOrigin.End)
                Position = Length - offset;

            return Position;
        }

        private void ValidateStreamRange()
        {
            if (Position < 0 || Position > SubstreamLength)
                throw new InvalidOperationException("Indicated stream position is not within the stream.");
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            Util.ValidateRange(buffer, offset, count);

            if ((_flags & Flags.Read) != Flags.Read)
                throw new NotSupportedException("Reading is not supported by the stream.");

            if (count > Length - _position)
                count = (int)(Length - _position);

            ValidateStreamRange();

            long p = _underlyingStream.Position;

            int timeout = 0;
            bool timeoutFailed = false;

            if ((_flags & Flags.OverrideTimeout) == Flags.OverrideTimeout)
                try
                {
                    timeout = _underlyingStream.WriteTimeout;
                }
                catch
                {
                    timeoutFailed = true;
                }
            if (p != _position)
                _underlyingStream.Position = _position;

            try
            {
                return _underlyingStream.Read(buffer, offset, count);
            }
            finally
            {
                if (ResetPosition)
                    try
                    {
                        _underlyingStream.Position = p;
                    }
                    catch { }

                if ((_flags & Flags.OverrideTimeout) == Flags.OverrideTimeout && !timeoutFailed)
                    _underlyingStream.WriteTimeout = timeout;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Util.ValidateRange(buffer, offset, count);

            if ((_flags & Flags.Write) != Flags.Write)
                throw new NotSupportedException("Writing is not supported by the stream.");

            if (count > Length - _position)
                throw new InvalidOperationException("Stream size is fixed.");

            ValidateStreamRange();

            long p = _underlyingStream.Position;

            //if SetLength is supported, call if the stream was too short
            if (_underlyingStream.Length < _position && _underlyingStream.CanWrite && _underlyingStream.CanSeek)
                try
                {
                    _underlyingStream.SetLength(_position + count);
                }
                catch (NotSupportedException)
                { }

            if (p != _position)
                _underlyingStream.Position = _position;

            int timeout = 0;
            bool timeoutFailed = false;

            if ((_flags & Flags.OverrideTimeout) == Flags.OverrideTimeout)
                try
                {
                    timeout = _underlyingStream.WriteTimeout;
                }
                catch
                {
                    timeoutFailed = true;
                }

            try
            {
                _underlyingStream.Write(buffer, offset, count);
            }
            finally
            {
                if (ResetPosition)
                    try
                    {
                        _underlyingStream.Position = p;
                    }
                    catch { }

                if ((_flags & Flags.OverrideTimeout) == Flags.OverrideTimeout && !timeoutFailed)
                    _underlyingStream.WriteTimeout = timeout;
            }

            _length = Math.Max(_length, _position + count);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (_flags & Flags.DisposeUnderlyingStream) == Flags.DisposeUnderlyingStream)
                _underlyingStream.Dispose();

            base.Dispose(disposing);
        }

        public override void Close()
        {
            if ((_flags & Flags.DisposeUnderlyingStream) == Flags.DisposeUnderlyingStream)
                _underlyingStream.Close();

            base.Close();
        }

        private void ValidateTimeout()
        {
            if (!CanTimeout)
                throw new InvalidOperationException("Timeout not supported.");
        }

        public override bool CanTimeout => (_flags & Flags.OverrideTimeout) == Flags.OverrideTimeout && _underlyingStream.CanTimeout;

        public override int ReadTimeout
        {
            get => _readTimeout;
            set
            {
                ValidateTimeout();

                _readTimeout = value;
            }
        }

        public override int WriteTimeout
        {
            get => _writeTimeout;
            set
            {
                ValidateTimeout();

                _writeTimeout = value;
            }
        }

        private enum Flags
        {
            Read = 1,
            Write = 2,
            OverrideTimeout = 4,
            ResetPosition = 8,
            FixedSize = 16,
            DisposeUnderlyingStream = 32,
        }
    }
}
