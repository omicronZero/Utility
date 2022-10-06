using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Data
{
    public abstract class ReadonlyObjectStream<T> : ObjectStream<T>
    {
        public override bool CanRead => true;
        public sealed override bool CanWrite => false;

        protected sealed override void SetLengthCore(long length)
        {
            //should not be reached unless called from inheriting type
            throw new NotSupportedException();
        }

        protected sealed override void WriteCore(ReadOnlySpan<T> span)
        {
            //should not be reached unless called from inheriting type
            throw new NotSupportedException();
        }

        public static ReadonlyObjectStream<T> FromDelegate(Func<T> entryFactory)
        {
            return FromDelegate(entryFactory, null);
        }

        public static ReadonlyObjectStream<T> FromDelegate(Func<T> entryFactory, Action disposeAction)
        {
            if (entryFactory == null)
                throw new ArgumentNullException(nameof(entryFactory));

            return new DelegateObjectStream(null, (l) => entryFactory(), disposeAction);
        }

        public static ReadonlyObjectStream<T> FromDelegate(Func<long> entryCountEvaluator, Func<long, T> entryFactory)
        {
            return FromDelegate(entryCountEvaluator, entryFactory, null);
        }

        public static ReadonlyObjectStream<T> FromDelegate(Func<long> entryCountEvaluator, Func<long, T> entryFactory, Action disposeAction)
        {
            if (entryCountEvaluator == null)
                throw new ArgumentNullException(nameof(entryCountEvaluator));
            if (entryFactory == null)
                throw new ArgumentNullException(nameof(entryFactory));

            return new DelegateObjectStream(entryCountEvaluator, entryFactory, disposeAction);
        }

        public static ReadonlyObjectStream<T> Concatenate(IEnumerable<ReadonlyObjectStream<T>> streams)
        {
            if (streams == null)
                throw new ArgumentNullException(nameof(streams));

            return new ConcatenateStream(streams);
        }

        private sealed class DelegateObjectStream : ReadonlyObjectStream<T>
        {
            public Func<long> EntryCountEvaluator { get; private set; }
            public Func<long, T> EntryFactory { get; private set; }
            public Action DisposeAction { get; private set; }

            private long _position;

            public DelegateObjectStream(Func<long> entryCountEvaluator, Func<long, T> entryFactory, Action disposeAction)
            {
                if (entryCountEvaluator == null)
                    throw new ArgumentNullException(nameof(entryCountEvaluator));
                if (EntryFactory == null)
                    throw new ArgumentNullException(nameof(entryFactory));

                EntryCountEvaluator = entryCountEvaluator;
                EntryFactory = entryFactory;
                DisposeAction = disposeAction;
            }

            public override bool CanSeek => EntryCountEvaluator != null;

            public override long Position
            {
                get
                {
                    if (!CanSeek)
                        throw new NotSupportedException("The stream is not seekable.");

                    return _position;
                }
                set
                {
                    if (!CanSeek)
                        throw new NotSupportedException("The stream is not seekable.");

                    _position = value;
                }
            }

            public override long Length => EntryCountEvaluator?.Invoke() ?? throw new NotSupportedException("The stream is not seekable.");

            protected override int ReadCore(Span<T> span)
            {
                long l = Length;
                long pos = Position;

                if (pos < 0 || pos > l)
                    throw new ArgumentOutOfRangeException(nameof(Position), "Stream position does not fall into the range of the stream.");

                int count = (int)Math.Min(span.Length, l - pos);

                Func<long, T> f = EntryFactory;
                try
                {
                    for (int i = 0; i < count; i++, pos++)
                    {
                        span[i] = EntryFactory(pos);
                    }

                }
                finally
                {
                    Position = pos;
                }

                return count;
            }

            protected override void Dispose(bool disposing)
            {
                if (!IsDisposed)
                {
                    EntryCountEvaluator = null;
                    EntryFactory = null;

                    try
                    {
                        DisposeAction?.Invoke();
                    }
                    finally
                    {
                        base.Dispose(disposing);
                    }
                    DisposeAction = null;
                }
                else
                {
                    base.Dispose();
                }
            }
        }

        private sealed class ConcatenateStream : ReadonlyObjectStream<T>
        {
            private IEnumerator<ObjectStream<T>> _streams;
            private bool _hasItem;

            public ConcatenateStream(IEnumerable<ObjectStream<T>> streams)
            {
                if (streams == null)
                    throw new ArgumentNullException(nameof(streams));

                _streams = streams.GetEnumerator();
                MoveNext();
            }

            private void MoveNext()
            {
                while ((_hasItem = _streams.MoveNext()) && !_streams.Current.CanRead) ;
            }

            public override bool CanSeek
            {
                get => false;
            }

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override long Length => throw new NotSupportedException();

            protected override int ReadCore(Span<T> span)
            {
                int c = 0;
                int index = 0;
                int count = span.Length;

                while (_hasItem && count > 0)
                {
                    int tc = _streams.Current.Read(span.Slice(index, count));

                    if (tc == 0)
                    {
                        MoveNext();
                        continue;
                    }

                    index += tc;
                    count -= tc;
                    c += tc;
                }

                return c;
            }

            protected override void Dispose(bool disposing)
            {
                var s = _streams;

                if (s != null)
                {
                    s.Dispose();
                    _streams = null;
                }
                base.Dispose(disposing);
            }
        }
    }
}
