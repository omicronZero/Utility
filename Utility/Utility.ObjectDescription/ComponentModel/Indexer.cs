using System;
using System.Collections.Generic;
using System.Text;
using Utility.ObjectDescription.Constraint;

namespace Utility.ObjectDescription.ComponentModel
{
    public abstract class Indexer : PropertyDescriptor
    {
        public IList<ParameterDescriptor> IndexerParameters { get; }

        public Indexer(ObjectConstraint constraint, MethodDescriptor getMethod, MethodDescriptor setMethod, SetPropertyConventions setMethodConventions)
            : base(constraint, getMethod, setMethod, setMethodConventions)
        {
            if (getMethod == null)
            {
                if (setMethod == null)
                {
                    IndexerParameters = Array.Empty<ParameterDescriptor>();
                }
                else
                {
                    var param = new List<ParameterDescriptor>(setMethod.Parameters);

                    if (setMethodConventions == SetPropertyConventions.First)
                        param.RemoveAt(0);
                    else if (setMethodConventions == SetPropertyConventions.Last)
                        param.RemoveAt(param.Count - 1);
                    else
                        throw new ArgumentException("Unsupported convention.", nameof(setMethodConventions));

                    IndexerParameters = param.AsReadOnly();
                }
            }
            else
            {
                IndexerParameters = getMethod.Parameters;

                if (setMethod != null)
                {
                    if (setMethod.ParameterCount != getMethod.ParameterCount + 1)
                        throw new ArgumentException("Mismatch of parameter count of get- and set-method.");

                    int baseValue;

                    if (setMethodConventions == SetPropertyConventions.First)
                        baseValue = 1;
                    else if (setMethodConventions == SetPropertyConventions.Last)
                        baseValue = 0;
                    else
                        throw new ArgumentException("Unsupported convention.", nameof(setMethodConventions));


                    for (int i = 0; i < getMethod.ParameterCount; i++)
                    {
                        if (!setMethod[i + baseValue].ParameterConstraint.Equals(getMethod[i].ParameterConstraint))
                            throw new ArgumentException("Incompatible indexer types for get- and set-method.");
                    }
                }
            }
        }
    }
}
