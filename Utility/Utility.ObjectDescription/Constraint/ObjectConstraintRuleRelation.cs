using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.ObjectDescription.Constraint
{
    public enum ObjectConstraintRuleRelation
    {
        /// <summary>
        /// No specific relation could be retrieved.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The set defined by the left constraint contains all elements of the set defined by the right constraint.
        /// </summary>
        LeftImpliesRight,

        /// <summary>
        /// The set defined by the right constraint contains all elements of the set defined by the left constraint.
        /// </summary>
        RightImpliesLeft,

        /// <summary>
        /// The sets defined by the constraints are equal.
        /// </summary>
        Equal,

        /// <summary>
        /// The sets defined by the constraints intersect.
        /// </summary>
        Intersection,

        /// <summary>
        /// The sets defined by the constraints are disjoint.
        /// </summary>
        Disjoint,
    }
}
