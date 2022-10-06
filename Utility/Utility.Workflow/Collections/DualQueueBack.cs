using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Collections
{
    public class DualQueueBack<T>
    {
        private readonly Queue<T> _queue;

        public DualQueueBack(Queue<T> queue)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            _queue = queue;
        }

        public int Count
        {
            get { return _queue.Count; }
        }

        public void Enqueue(T item)
        {
            _queue.Enqueue(item);
        }

        public void TrimExcess()
        {
            _queue.TrimExcess();
        }
    }
}
