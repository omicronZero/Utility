using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.ObjectDescription.Constraint
{
    public struct ObjectConstraintBuilder
    {
        private readonly List<ObjectConstraintRule> _rules;

        internal ObjectConstraintBuilder(List<ObjectConstraintRule> rules)
        {
            if (rules == null)
                throw new ArgumentNullException(nameof(rules));

            _rules = rules;
        }

        public void Append(ObjectConstraintRule rule)
        {
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));
            
            _rules.Add(rule);
        }

        public ObjectConstraint Build()
        {
            if (_rules == null)
                throw new NullReferenceException();

            return new ObjectConstraint(_rules);
        }

        public static ObjectConstraintBuilder Create()
        {
            return new ObjectConstraintBuilder(new List<ObjectConstraintRule>());
        }

        public static implicit operator ObjectConstraint(ObjectConstraintBuilder builder)
        {
            if (builder._rules == null)
                return null;

            return builder.Build();
        }
    }
}
