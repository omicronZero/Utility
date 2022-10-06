using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Utility.Dynamic
{
    public struct ILSourceDescription
    {
        public RoutingSource Source { get; }
        internal object Member { get; }

        public bool IsField => Source == RoutingSource.Field;
        public bool IsParameter => Source == RoutingSource.OutParameter || Source == RoutingSource.ReturnParameter;
        public bool IsOutParameter => Source == RoutingSource.OutParameter;
        public bool IsReturnParameter => Source == RoutingSource.ReturnParameter;
        public bool IsLocal => Source == RoutingSource.Local;

        public FieldInfo Field
        {
            get
            {
                if (Source != RoutingSource.Field)
                    throw new InvalidOperationException("Output not a field.");

                return (FieldInfo)Member;
            }
        }

        public ParameterInfo Parameter
        {
            get
            {
                if (Source != RoutingSource.OutParameter || Source == RoutingSource.ReturnParameter)
                    throw new InvalidOperationException("Output not a parameter.");

                return (ParameterInfo)Member;
            }
        }

        public LocalVariableInfo Variable
        {
            get
            {
                if (Source != RoutingSource.Local)
                    throw new InvalidOperationException("Output not a local variable.");

                return (LocalVariableInfo)Member;
            }
        }

        internal ILSourceDescription(object member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            if (member is FieldInfo)
                Source = RoutingSource.Field;
            else if (member is ParameterInfo p)
                Source = p.IsRetval ? RoutingSource.ReturnParameter : (p.IsOut ? RoutingSource.OutParameter : throw new ArgumentException("Return or out parameter expected.", nameof(member)));
            else if (member is LocalVariableInfo)
                Source = RoutingSource.Local;
            else
                throw new ArgumentException("Unsupported member.", nameof(member));

            Member = member;
        }
    }
}
