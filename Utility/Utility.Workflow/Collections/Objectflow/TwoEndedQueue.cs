using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Collections.Objectflow
{
    public class TwoEndedQueue<T> : Queue<T>
    {
        private QueueFront<T> _front;
        private QueueBack<T> _back;

        public QueueFront<T> Front
        {
            get
            {
                EnsureFrontInitialized();

                return _front;
            }
        }

        public QueueBack<T> Back
        {
            get
            {
                EnsureBackInitialized();

                return _back;
            }
        }

        private void EnsureFrontInitialized()
        {
            if (_front == null)
                _front = new QueueFront<T>(this);
        }

        private void EnsureBackInitialized()
        {
            if (_back == null)
                _back = new QueueBack<T>(this);
        }

        /// <summary>
        /// Ensures that the queue's front and back are both initialized.
        /// </summary>
        public void EnsureInitialized()
        {
            EnsureFrontInitialized();
            EnsureBackInitialized();
        }

        public TwoEndedQueue()
        { }

        public TwoEndedQueue(int capacity)
            : base(capacity)
        { }

        public TwoEndedQueue(IEnumerable<T> collection)
            : base(collection)
        { }
    }
}
