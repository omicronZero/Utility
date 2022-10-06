using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Collections
{
    public class DualStack<T> : Stack<T>
    {
        private DualStackFront<T> _front;
        private DualStackBack<T> _back;

        public DualStackFront<T> Front
        {
            get
            {
                EnsureFrontInitialized();

                return _front;
            }
        }

        public DualStackBack<T> Back
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
                _front = new DualStackFront<T>(this);
        }

        private void EnsureBackInitialized()
        {
            if (_back == null)
                _back = new DualStackBack<T>(this);
        }

        /// <summary>
        /// Ensures that the queue's front and back are both initialized.
        /// </summary>
        public void EnsureInitialized()
        {
            EnsureFrontInitialized();
            EnsureBackInitialized();
        }

        public DualStack()
        { }

        public DualStack(int capacity)
            : base(capacity)
        { }

        public DualStack(IEnumerable<T> collection)
            : base(collection)
        { }
    }
}
