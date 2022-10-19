using Utility.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataSpecTests.Mocks
{
    public sealed class QueueReaderWriter : IObjectReader, IObjectWriter, IEnumerable<(Type, object)>
    {
        private readonly Queue<(Type, object)> _values = new Queue<(Type, object)>();

        public T Read<T>()
        {
            if (_values.Count == 0)
                throw new SpecialException("Queue empty.");

            if (_values.Peek().Item1 != typeof(T))
                throw new SpecialException("Type mismatch.");

            return (T)_values.Dequeue().Item2;
        }

        public void Write<T>(T instance)
        {
            _values.Enqueue((typeof(T), instance));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<(Type, object)> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        public IEnumerable<object> GetObjects() => _values.Select((s) => s.Item2);
        public IEnumerable<Type> GetTypes() => _values.Select((s) => s.Item1);
    }

}
