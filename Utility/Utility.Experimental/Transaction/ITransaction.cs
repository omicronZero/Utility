using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Transaction
{
    public interface ITransaction<in TInput>
    {
        void Handle(TInput input, TransactionHandle handle, out object state);
        void Undo(object state);
    }
}
