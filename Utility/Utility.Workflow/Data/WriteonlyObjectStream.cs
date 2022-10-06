using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Utility.Data
{
    public abstract class WriteonlyObjectStream<T> : ObjectStream<T>
    {
        public sealed override bool CanRead => false;
        public override bool CanWrite => true;

        protected sealed override int ReadCore(Span<T> span)
        {
            //should not be reached unless called from inheriting type
            throw new NotSupportedException();
        }

        public static WriteonlyObjectStream<T> FromDelegate(Action<T> writer)
        {
            return FromDelegate(writer, null);
        }

        public static WriteonlyObjectStream<T> FromDelegate(Action<T> writer, Action disposeAction)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            return new DelegateObjectStream(null, (l, v) => writer(v), disposeAction);
        }

        public static WriteonlyObjectStream<T> FromDelegate(Func<long> lengthEvaluator, Action<long, T> writer)
        {
            return FromDelegate(lengthEvaluator, writer, null);
        }

        public static WriteonlyObjectStream<T> FromDelegate(Func<long> lengthEvaluator, Action<long, T> writer, Action disposeAction)
        {
            if (lengthEvaluator == null)
                throw new ArgumentNullException(nameof(lengthEvaluator));
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            return new DelegateObjectStream(lengthEvaluator, writer, disposeAction);
        }

        public static WriteonlyObjectStream<T> ToMany(IEnumerable<ObjectStream<T>> streams, bool parallel)
        {
            if (streams == null)
                throw new ArgumentNullException(nameof(streams));

            return new WriteToMany(streams, parallel);
        }

        private sealed class DelegateObjectStream : WriteonlyObjectStream<T>
        {
            public Func<long> EntryCountEvaluator { get; private set; }
            public Action<long, T> Writer { get; private set; }
            public Action DisposeAction { get; private set; }

            private long _position;

            public DelegateObjectStream(Func<long> lengthEvaluator, Action<long, T> writer, Action disposeAction)
            {
                if (lengthEvaluator == null)
                    throw new ArgumentNullException(nameof(lengthEvaluator));
                if (Writer == null)
                    throw new ArgumentNullException(nameof(writer));

                EntryCountEvaluator = lengthEvaluator;
                Writer = writer;
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

            protected override void WriteCore(ReadOnlySpan<T> span)
            {
                long l = Length;
                long pos = Position;

                if (pos < 0 || pos > l)
                    throw new ArgumentOutOfRangeException(nameof(Position), "Stream position does not fall into the range of the stream.");

                for (int i = 0; i < span.Length; i++, pos++)
                {
                    Writer(pos, span[i]);
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (!IsDisposed)
                {
                    EntryCountEvaluator = null;
                    Writer = null;

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

        private sealed class WriteToMany : WriteonlyObjectStream<T>
        {
            private IEnumerable<ObjectStream<T>> _streams;
            private bool _parallel;

            public WriteToMany(IEnumerable<ObjectStream<T>> streams, bool parallel)
            {
                if (streams == null)
                    throw new ArgumentNullException(nameof(streams));

                _streams = streams;
                _parallel = parallel;
            }

            public override bool CanSeek => false;

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override long Length => throw new NotSupportedException();

            protected override void WriteCore(ReadOnlySpan<T> span)
            {
                if (_parallel)
                {
                    var intermediateBuffer = span.ToArray(); //TODO: if, one day, span somehow can be supplied to the body, use that instead of using the copy supplied by ToArray
                    System.Threading.Tasks.Parallel.ForEach(_streams, (s) => s.Write(intermediateBuffer));
                }
                else
                {
                    foreach (ObjectStream<T> s in _streams)
                        s.Write(span);
                }
            }

            protected override void WriteCore(T[] buffer, int index, int count)
            {
                if (_parallel)
                {
                    System.Threading.Tasks.Parallel.ForEach(_streams, (s) => s.Write(buffer, index, count));
                }
                else
                    base.WriteCore(buffer, index, count);
            }
        }
    }
}
