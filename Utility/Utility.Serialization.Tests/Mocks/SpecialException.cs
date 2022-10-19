using System;
using System.Runtime.Serialization;

namespace DataSpecTests.Mocks
{
    public class SpecialException : Exception
    {
        public SpecialException()
            : base("Assertion error.")
        {
        }

        public SpecialException(string message) : base(message)
        {

        }

        public SpecialException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SpecialException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
