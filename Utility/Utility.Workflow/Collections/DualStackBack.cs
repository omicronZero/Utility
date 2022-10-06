using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Collections
{
    public class DualStackBack<T>
    {
        private readonly Stack<T> _stack;

        public DualStackBack(Stack<T> stack)
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
