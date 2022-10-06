using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Utility.Data
{
    // public delegate int ReadStreamEvaluator<T>(T[] buffer, int index, int count);
    // public delegate void WriteStreamEvaluator<T>(T[] buffer, int index, int count);

    public abstract class ObjectStream<T> : IDisposable
    {
        private bool _disposed;

        [ThreadStatic]
        private static T[] _singleItem;

        public abstract bool CanRead { get; }
        public abstract bool CanWrite { get; }
        public abstract bool CanSeek { get; }

        public abstract long Position { get; set; }
        public abstract long Length { get; }

        protected abstract void WriteCore(ReadOnlySpan<T> span);
        protected abstract int ReadCore(Span<T> span);

        private static T[] SingleItem
        {
            get => _singleItem ??= new T[1];
        }

        public virtual bool CanSetLength => false;

        protected virtual void SetLengthCore(long length)
        { }

        protected virtual void WriteCore(T[] buffer, int index, int count)
        {
            WriteCore(new ReadOnlySpan<T>(buffer, index, count));
        }

        protected virtual int ReadCore(T[] buffer, int index, int count)
        {
            return ReadCore(new Span<T>(buffer, index, count));
        }

        public void Write(T[] buffer) => Write(buffer, 0, buffer?.Length ?? 0);

        public void Write(T[] buffer, int index, int count)
        {
            Util.ValidateNamedRange(buffer, index, count, arrayName: nameof(buffer));

            if (IsDisposed)
                throw new ObjectDisposedException(this.GetType().Name);

            if (!CanWrite)
                throw new NotSupportedException("Writing is not supported by the current stream.");

            WriteCore(buffer, index, count);
        }

        public void Write(ReadOnlySpan<T> span)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(this.GetType().Name);

            if (!CanWrite)
                throw new NotSupportedException("Writing is not supported by the current stream.");

            WriteCore(span);
        }

        public int Read(T[] buffer) => Read(buffer, 0, buffer?.Length ?? 0);

        public int Read(T[] buffer, int index, int count)
        {
            Util.ValidateNamedRange(buffer, index, count, arrayName: nameof(buffer));

            if (IsDisposed)
                throw new ObjectDisposedException(this.GetType().Name);

            if (!CanRead)
                throw new NotSupportedException("Reading is not supported by the current stream.");

            return ReadCore(buffer, index, count);
        }

        public int Read(Span<T> span)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(this.GetType().Name);

            if (!CanRead)
                throw new NotSupportedException("Reading is not supported by the current stream.");

            return ReadCore(span);
        }

        public void SetLength(long length)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(this.GetType().Name);

            if (!CanWrite)
                throw new NotSupportedException("Writing is not supported by the current stream.");

            if (!CanSeek)
                throw new NotSupportedException("Seeking is not supported by the current stream.");

            if (!CanSetLength)
                throw new NotSupportedException("Stream does not support setting its length.");

            SetLengthCore(length);
        }

        protected bool IsPositionInsideBounds
        {
            get
            {
                long p = Position;

                return p >= 0 && p <= Length;
            }
        }

        public virtual void Write(T item)
        {
            //warning, keep actions atomic on SingleItem as it may be shared in recursive operations (no thread interference expected)
            T[] s = SingleItem;

            s[0] = item;
            Write(item);

            s[0] = default;
        }

        public virtual bool TryReadOne(out T item)
        {
            //warning, keep actions atomic on SingleItem as it may be shared in recursive operations (no thread interference expected)
            T[] s = SingleItem;

            int c = Read(s, 0, 1);

            if (c == 1)
            {
                item = s[0];
            }
            else
            {
                item = default;
            }
            s[0] = default;
            return c == 1;
        }

        public virtual T ReadOne()
        {
            T it;

            if (!TryReadOne(out it))
                throw new ArgumentException("The current stream does not contain an element.");

            return it;
        }

        public virtual long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.Begin)
                Position = offset;
            else if (origin == SeekOrigin.Current)
                Position += offset;
            else if (origin == SeekOrigin.End)
                Position = Length - offset;
            else
                throw new ArgumentException("Unknown seek origin.", nameof(origin));

            return Position;
        }

        public bool IsDisposed
        {
            get { return _disposed; }
        }

        protected virtual void Dispose(bool disposing)
        {
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
