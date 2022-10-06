using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Utility.ObjectDescription.ComponentModel
{
    public abstract class MethodGroupDescriptor : MemberDescriptor
    {
        public abstract IList<MethodDescriptor> Methods { get; }
        public abstract bool HasMethodRoles { get; }

        public virtual IEnumerable<MethodDescriptor> GetMethods(params ParameterDescriptor[] parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            foreach (MethodDescriptor d in Methods)
            {
                if (d.ParameterCount != parameters.Length)
                    continue;

                bool isValid = true;

                for (int i = 0; i < parameters.Length; i++)
                {
                    ParameterDescriptor lparam = d[i];
                    ParameterDescriptor rparam = parameters[i];

                    if (lparam.IsIn != rparam.IsIn || lparam.IsRef != rparam.IsRef || lparam.IsOut != rparam.IsOut || lparam.IsReturn != rparam.IsReturn)
                    {
                        isValid = false;
                        break;
                    }

                    if ((lparam.IsIn || lparam.IsRef) && !lparam.ParameterConstraint.IsValidConstraint(rparam.ParameterConstraint))
                    {
                        isValid = false;
                        break;
                    }

                    if ((lparam.IsOut || lparam.IsRef) && !rparam.ParameterConstraint.IsValidConstraint(lparam.ParameterConstraint))
                    {
                        isValid = false;
                        break;
                    }
                }

                if (isValid)
                {
                    yield return d;
                }
            }
        }
    }
}
