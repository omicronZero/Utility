using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Transaction
{
    public class TransactionState
    {
        internal object Transaction { get; set; }

        public TransactionToken Token { get; internal set; }
    }
}
