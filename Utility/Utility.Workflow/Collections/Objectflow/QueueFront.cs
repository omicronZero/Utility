using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Utility.Collections.Objectflow
{
    public class QueueFront<T> : IEnumerable<T>
    {
        private readonly Queue<T> _queue;

        public QueueFront(Queue<T> queue)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            _queue = queue;
        }

        public int Count
        {
            get { return _queue.Count; }
        }

        public T Dequeue()
        {
            return _queue.Dequeue();
        }

        public T Peek()
        {
            return _queue.Peek();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        public T[] ToArray()
        {
            return _queue.ToArray();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _queue.CopyTo(array, arrayIndex);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_queue).GetEnumerator();
        }
    }
}
