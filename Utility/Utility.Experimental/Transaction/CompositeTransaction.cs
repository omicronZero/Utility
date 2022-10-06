using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Transaction
{
    public class CompositeTransaction<TInput> : Transaction<TInput>
    {
        private readonly Func<ITransaction<TInput>, ITransaction<TInput>> _nextTransactionEvaluator;

        public CompositeTransaction(Func<ITransaction<TInput>, ITransaction<TInput>> nextTransactionEvaluator)
        {
            if (nextTransactionEvaluator == null)
                throw new ArgumentNullException(nameof(nextTransactionEvaluator));

            _nextTransactionEvaluator = nextTransactionEvaluator;
        }

        protected override void UndoCore(TransactionState state)
        {
            State internalState = (State)state;

            for (int i = internalState.States.Count - 1; i >= 0; i--)
            {
                internalState.States[i].Transaction.Undo(internalState.States[i].State);
            }
        }

        protected override void HandleCore(TInput input, TransactionHandle handle, out TransactionState state)
        {
            var resultState = new State();

            state = resultState;

            try
            {
                for (ITransaction<TInput> currentTransaction = _nextTransactionEvaluator(null); currentTransaction != null; currentTransaction = _nextTransactionEvaluator(currentTransaction))
                {
                    object currentState;

                    currentTransaction.Handle(input, handle, out currentState);

                    resultState.States.Add((currentTransaction, currentState));
                }
            }
            catch (Exception ex)
            {
                try
                {
                    for (int i = resultState.States.Count - 1; i >= 0; i--)
                    {
                        resultState.States[i].Transaction.Undo(resultState.States[i].State);
                    }
                }
                catch (Exception corruptionEx)
                {
                    throw new PossibleCorruptionException(corruptionEx);
                }

                throw new UndoneException(ex);
            }
        }

        private sealed class State : TransactionState
        {
            public List<(ITransaction<TInput> Transaction, object State)> States { get; }

            public State()
            {
                States = new List<(ITransaction<TInput>, object)>();
            }
        }
    }
}
