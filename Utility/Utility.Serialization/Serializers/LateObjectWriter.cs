using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Serialization.Serializers
{
    public class LateObjectWriter : IObjectWriter
    {
        private readonly IObjectWriter _innerWriter;

        private bool _isLate;
        private readonly Queue<Leaf> _leafs;

        private bool _draining;

        public LateObjectWriter(IObjectWriter innerWriter)
        {
            _innerWriter = innerWriter ?? throw new ArgumentNullException(nameof(innerWriter));
            _leafs = new Queue<Leaf>();
        }

        public void Write<T>(T instance)
        {
            //is some value still pending? We wait for its completion and put the instance into the queue
            if (_isLate)
            {
                _leafs.Enqueue(new Leaf<T>(instance));
                return;
            }

            WriteCore(instance);
        }

        private bool WriteCore<T>(T instance)
        {
            //is the next instance to write a late-evaluation object? If not, we write it to the writer
            //and are finished
            if (!(instance is LateObject lateObject))
            {
                _innerWriter.Write(instance);
                return true;
            }

            bool wasEmitted = false;

            _isLate = true;

            //we have to wait for the late value to complete its computation
            lateObject.ValueChanged += (s, e) =>
            {
                //the delegate unregisters itself upon completion
                //if the value is already computed, it gets called directly
                Helper<T>.Write(_innerWriter, instance);
                _isLate = false;
                Flush();
                wasEmitted = true;
            };

            return wasEmitted;
        }

        private void Flush()
        {
            //prevent stack overflow possibility by recursive calls in very nested scenarios
            if (_draining)
                return;

            try
            {
                _draining = true;

                while (!_isLate && _leafs.Count > 0)
                {
                    var next = _leafs.Peek();

                    next.WriteTo(this);

                    if (!_isLate)
                        _leafs.Dequeue();
                }
            }
            finally
            {
                _draining = false;
            }
        }

        private static class Helper<T>
        {
            public static Type ValueType { get; }

            private static Action<IObjectWriter, T> _writeImpl;

            static Helper()
            {
                ValueType = typeof(T).GetImplementationOf(typeof(LateObject<>)).First().GetGenericArguments()[0];

                var property = typeof(T).GetProperty(nameof(LateObject<int>.Value), ValueType)!;

                var writerParameter = Expression.Parameter(typeof(IObjectWriter));
                var instanceParameter = Expression.Parameter(typeof(T));

                var writeMethod = typeof(IObjectWriter).GetMethod("Write")!.MakeGenericMethod(ValueType);

                var valueExpression = Expression.Property(instanceParameter, property);

                var writeCall = Expression.Call(writerParameter, writeMethod, valueExpression);

                _writeImpl = Expression.Lambda<Action<IObjectWriter, T>>(writeCall, writerParameter, instanceParameter).Compile();
            }

            public static void Write(IObjectWriter innerWriter, T instance)
            {
                _writeImpl(innerWriter, instance);
            }
        }

        private abstract class Leaf
        {
            public abstract void WriteTo(LateObjectWriter writer);
        }

        private class Leaf<T> : Leaf
        {
            public T Value { get; }

            public Leaf(T value)
            {
                Value = value;
            }

            public override void WriteTo(LateObjectWriter writer)
            {
                writer.WriteCore(Value);
            }
        }
    }
}
