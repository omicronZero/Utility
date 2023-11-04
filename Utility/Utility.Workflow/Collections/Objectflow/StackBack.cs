using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Collections.Objectflow
{
    public class StackBack<T>
    {
        private readonly Stack<T> _stack;

        public StackBack(Stack<T> stack)
        {
            if (stack == null)
                throw new ArgumentNullException(nameof(stack));

            _stack = stack;
        }

        public int Count => _stack.Count;

        public void Push(T item)
        {
            _stack.Push(item);
        }

        public void TrimExcess()
        {
            _stack.TrimExcess();
        }
    }
}
