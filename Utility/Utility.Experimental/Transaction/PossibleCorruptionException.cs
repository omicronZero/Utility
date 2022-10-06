using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Utility.Transaction
{
    [Serializable]
    public class PossibleCorruptionException : Exception
    {
        private const string DefaultText = "A corruption may have occurred.";

        public PossibleCorruptionException()
            : base(DefaultText)
        { }

        public PossibleCorruptionException(string message) : base(message)
        { }

        public PossibleCorruptionException(string message, Exception innerException) : base(message, innerException)
        { }

        public PossibleCorruptionException(Exception innerException) : base(DefaultText, innerException)
        { }

        protected PossibleCorruptionException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}
