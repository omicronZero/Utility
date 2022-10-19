using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Serialization
{

    [Serializable]
    public class BindingException : Exception
    {
        public BindingException()
            : base("An exception occurred during binding.")
        { }

        public BindingException(string message)
            : base(message)
        { }

        public BindingException(string message, Exception inner)
            : base(message, inner)
        { }

        protected BindingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
