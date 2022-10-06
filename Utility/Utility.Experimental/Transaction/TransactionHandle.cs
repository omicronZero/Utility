using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Transaction
{
    public class TransactionHandle
    {
        private long _generation;

        //CRITICAL: ambiguous generations (e.g. Increment, Decrement, Increment)

        public TransactionToken Increment()
        {
            return new TransactionToken(_generation++);
        }

        public void Decrement(TransactionToken token)
        {
            if (!TryDecrement(token))
                throw new InvalidOperationException("Order of operations invalid.");
        }

        public bool TryDecrement(TransactionToken token)
        {
            if (_generation != token.Generation)
                return false;

            _generation--;

            return true;
        }
    }
}
