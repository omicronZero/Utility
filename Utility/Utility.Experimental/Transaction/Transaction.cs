using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Utility.Transaction
{
    public abstract class Transaction<TInput> : ITransaction<TInput>
    {
        protected abstract void HandleCore(TInput input, TransactionHandle handle, out TransactionState state);

        protected abstract void UndoCore(TransactionState state);

        public void Handle(TInput input, TransactionHandle handle, out TransactionState state)
        {
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));

            HandleCore(input, handle, out state);

            if (state != null)
            {
                state.Transaction = this;
                state.Token = handle.Increment();
            }
        }

        public void Undo(TransactionState state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            if (state.Transaction != this)
                throw new ArgumentException("The specified state is not associated with the current transaction.", nameof(state));

            UndoCore(state);
        }

            void ITransaction<TInput>.Handle(TInput input, TransactionHandle handle, out object state)
        {
            TransactionState s;

            Handle(input, handle, out s);

            state = s;
        }

        void ITransaction<TInput>.Undo(object state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            if (!(state is TransactionState s))
                throw new ArgumentException("Transaction state expected.", nameof(state));

            UndoCore(s);
        }
    }
}
