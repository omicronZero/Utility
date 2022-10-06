using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Utility.ObjectDescription.Constraint;

namespace Utility.ObjectDescription.ComponentModel
{
    public class ReflectionIndexer : Indexer
    {
        public PropertyInfo Indexer { get; }

        public ReflectionIndexer(PropertyInfo indexer)
            : base(TypeConstraintRule.CreateConstraint(indexer.PropertyType), new ReflectionMethodDescriptor((indexer ?? throw new ArgumentNullException(nameof(indexer))).GetGetMethod(false)),
                   new ReflectionMethodDescriptor(indexer.GetSetMethod(false)),
                   SetPropertyConventions.Last)
        {
            Indexer = indexer;
        }

        public override object[] GetAttributes(Type attributeType)
        {
            if (attributeType == null)
                throw new ArgumentNullException(nameof(attributeType));

            return Indexer.GetCustomAttributes(attributeType, true);
        }
    }
}
