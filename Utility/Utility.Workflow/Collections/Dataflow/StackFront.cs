using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Utility.Workflow.Collections.Dataflow
{
    public class StackFront<T> : IEnumerable<T>
    {
        private readonly Stack<T> _stack;

        public StackFront(Stack<T> stack)
        {
            if (stack == null)
                throw new ArgumentNullException(nameof(stack));

            _stack = stack;
        }

        public int Count
        {
            get { return _stack.Count; }
        }

        public T Pop()
        {
            return _stack.Pop();
        }

        public T Peek()
        {
            return _stack.Peek();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _stack.GetEnumerator();
        }

        public T[] ToArray()
        {
            return _stack.ToArray();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _stack.CopyTo(array, arrayIndex);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_stack).GetEnumerator();
        }
    }
}
