using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Workflow.Collections.Dataflow
{
    public class QueueBack<T>
    {
        private readonly Queue<T> _queue;

        public QueueBack(Queue<T> queue)
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
