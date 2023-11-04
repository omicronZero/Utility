using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Collections.Objectflow
{
    public class TwoEndedStack<T> : Stack<T>
    {
        private StackFront<T> _front;
        private StackBack<T> _back;

        public StackFront<T> Front
        {
            get
            {
                EnsureFrontInitialized();

                return _front;
            }
        }

        public StackBack<T> Back
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
                _front = new StackFront<T>(this);
        }

        private void EnsureBackInitialized()
        {
            if (_back == null)
                _back = new StackBack<T>(this);
        }

        /// <summary>
        /// Ensures that the queue's front and back are both initialized.
        /// </summary>
        public void EnsureInitialized()
        {
            EnsureFrontInitialized();
            EnsureBackInitialized();
        }

        public TwoEndedStack()
        { }

        public TwoEndedStack(int capacity)
            : base(capacity)
        { }

        public TwoEndedStack(IEnumerable<T> collection)
            : base(collection)
        { }
    }
}
