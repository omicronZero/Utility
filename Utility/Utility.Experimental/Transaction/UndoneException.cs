using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Utility.Transaction
{
    [Serializable]
    public class UndoneException : Exception
    {
        private const string DefaultText = "A transaction has been aborted and was successfully undone.";

        public UndoneException()
            : base(DefaultText)
        { }

        public UndoneException(string message) : base(message)
        { }

        public UndoneException(string message, Exception innerException) : base(message, innerException)
        { }

        public UndoneException(Exception innerException) : base(DefaultText, innerException)
        { }

        protected UndoneException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}
