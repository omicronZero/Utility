using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Utility.Workflow.Contract
{
    [Serializable]
    public class BadContractActionException : Exception
    {
        public BadContractActionException()
            : base("The contract action was in a state that disallows the performed action.")
        { }

        public BadContractActionException(string message) : base(message)
        {
        }

        public BadContractActionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BadContractActionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
