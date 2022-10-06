using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Utility.ObjectDescription.Constraint
{
    [Serializable]
    public abstract class ObjectConstraintRule : IEquatable<ObjectConstraintRule>, ISerializable
    {
        public abstract bool IsInherited { get; }

        public abstract bool IsValid(object instance);

        public abstract bool IsValid<T>(T instance);

        public abstract bool Equals(ObjectConstraintRule other);

        public abstract override int GetHashCode();

        public abstract ObjectConstraintRuleRelation GetRelation(ObjectConstraintRule other);

        protected ObjectConstraintRule()
        { }

        /// <summary>
        /// Returns, whether the specified constraint is compatible to the current constraint rule.
        /// By default, this is, if all rules of the supplied constraint neither define a disjoint or intersecting set nor that any of the supplied constraint's rules strictly imply the current rule
        /// See <see cref="IsValidConstraint(ObjectConstraintRule)"/>.
        /// </summary>
        /// <param name="other">The constraint to test.</param>
        /// <returns>Returns, whether the constraint is compatible to the current constraint rule.</returns>
        public virtual bool IsValidConstraint(ObjectConstraint other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            foreach (ObjectConstraintRule r in other.GetRules())
            {
                ObjectConstraintRuleRelation relation = GetRelation(r);

                if (relation == ObjectConstraintRuleRelation.Disjoint || relation == ObjectConstraintRuleRelation.Intersection || relation == ObjectConstraintRuleRelation.RightImpliesLeft)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns, whether the specified constraint rule is compatible to the current constraint rule.
        /// By default, this is, if the supplied constraint rule neither defines a disjoint or intersecting set nor that the other's rule strictly implies the current rule. See remarks.
        /// </summary>
        /// <param name="other">The constraint to test.</param>
        /// <returns>Returns, whether the constraint is compatible to the current constraint rule.</returns>
        /// <remarks>
        /// A constraint rule B is considered valid in rule A, if the following holds:
        /// <list type="table">
        /// 
        /// <item><description><see cref="ObjectConstraintRuleRelation.Unknown"/>: The relationship between A and B is unknown, i.e., has not been implemented or cannot be determined.</description></item>
        /// 
        /// <item><description>
        ///     <see cref="ObjectConstraintRuleRelation.Disjoint"/>: A excludes any object contained in B and vice versa.
        ///     This, for example, is the case, if A is a number in [0, 1] and B constrains the number to the range [2, 3].
        /// </description></item>
        /// 
        /// <item><description>
        ///     <see cref="ObjectConstraintRuleRelation.Equal"/>: A and B define the same set of passing objects.
        ///     This, for example, is the case, if A is a number in [0, 1] and B, too, constrains the number to the range [0, 1].
        /// </description></item>
        /// <item><description>
        /// 
        /// <item><description>
        ///     <see cref="ObjectConstraintRuleRelation.LeftImpliesRight"/>: The set of objects valid on A contains the set of objects valid on B: .
        ///     This is meant strictly, i.e., it holds that A and B are not equal.
        ///     For example, if A is that a number is in [0, 1], then B restricting numbers to [0.3, 1] is implied by A, while C restricting to [-1, 1] is not implied by A.
        /// </description></item>
        /// 
        /// <item><description>
        ///     <see cref="ObjectConstraintRuleRelation.RightImpliesLeft"/>: The set of objects valid on A is contained in the set of objects valid on B: .
        ///     This is meant strictly, i.e., it holds that A and B are not equal.
        ///     For example, if A is that a number is in [0, 1], then B restricting numbers to [-1, 1] is implied by A, while C restricting to [0.3, 1] is not implied by A.
        /// </description></item>
        /// </list>
        /// </remarks>
        public virtual bool IsValidConstraint(ObjectConstraintRule other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            ObjectConstraintRuleRelation relation = GetRelation(other);

            return relation != ObjectConstraintRuleRelation.Disjoint && relation != ObjectConstraintRuleRelation.Intersection && relation != ObjectConstraintRuleRelation.RightImpliesLeft;
        }

        protected ObjectConstraintRule(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
        }

        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(info, context);
        }

        public virtual void PassArgument(object instance)
        {
            if (!IsValid(instance))
                ThrowInvalidArgument();
        }

        public virtual void PassArgument<T>(T instance)
        {
            if (!IsValid<T>(instance))
                ThrowInvalidArgument();
        }

        private static void ThrowInvalidArgument()
        {
            throw new ArgumentException("The specified argument is not valid.");
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is ObjectConstraintRule r && Equals(r);
        }

        public static bool operator ==(ObjectConstraintRule left, ObjectConstraintRule right)
        {
            if (left == null)
                return right == null;
            else if (right == null)
                return false;

            return object.ReferenceEquals(left, right) || left.Equals(right);
        }

        public static bool operator !=(ObjectConstraintRule left, ObjectConstraintRule right)
        {
            if (left == null)
                return right != null;
            else if (right == null)
                return true;

            return !(object.ReferenceEquals(left, right) || left.Equals(right));
        }
    }
}
