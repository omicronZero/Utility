using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Collections
{
    public class DualQueue<T> : Queue<T>
    {
        private DualQueueFront<T> _front;
        private DualQueueBack<T> _back;

        public DualQueueFront<T> Front
        {
            get
            {
                EnsureFrontInitialized();

                return _front;
            }
        }

        public DualQueueBack<T> Back
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
                _front = new DualQueueFront<T>(this);
        }

        private void EnsureBackInitialized()
        {
            if (_back == null)
                _back = new DualQueueBack<T>(this);
        }

        /// <summary>
        /// Ensures that the queue's front and back are both initialized.
        /// </summary>
        public void EnsureInitialized()
        {
            EnsureFrontInitialized();
            EnsureBackInitialized();
        }

        public DualQueue()
        { }

        public DualQueue(int capacity)
            : base(capacity)
        { }

        public DualQueue(IEnumerable<T> collection)
            : base(collection)
        { }
    }
}
