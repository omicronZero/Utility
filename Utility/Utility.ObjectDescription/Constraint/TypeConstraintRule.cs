using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Utility;

namespace Utility.ObjectDescription.Constraint
{
    [Serializable]
    public abstract class TypeConstraintRule : ObjectConstraintRule
    {
        public Type Type { get; }

        public override bool IsInherited => true;

        internal TypeConstraintRule(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            Type = type;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2229:Implement serialization constructors", Justification = "We do not want others to implement subtypes of this.")]
        internal TypeConstraintRule(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (SerializeType)
            {
                Type = info.GetValue<Type>("Type");
            }
        }

        public override bool IsValid(object instance)
        {
            return Type.IsInstanceOfType(instance);
        }

        public override bool IsValid<T>(T instance)
        {
            return IsValid((object)instance);
        }

        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (SerializeType)
            {
                info.AddValue("Type", Type);
            }
        }

        protected virtual bool SerializeType => true;

        public virtual bool IsValidType(Type instanceType)
        {
            if (instanceType == null)
                throw new ArgumentNullException(nameof(instanceType));

            return Type.IsAssignableFrom(instanceType);
        }

        //TODO: test
        public static TypeConstraintRule Get(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return (TypeConstraintRule)typeof(TypeConstraintRule<>).MakeGenericType(type).InvokeMember(
                "Instance", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
                null,
                null,
                null);
        }

        public static ObjectConstraint CreateConstraint(Type type)
        {
            return new ObjectConstraint(Get(type));
        }

        public static ObjectConstraint CreateConstraint<T>()
        {
            return new ObjectConstraint(TypeConstraintRule<T>.Instance);
        }
    }

    [Serializable]
    public class TypeConstraintRule<TType> : TypeConstraintRule, IObjectReference
    {
        private static readonly TypeConstraintRule<TType> _instance = new TypeConstraintRule<TType>();

        public static TypeConstraintRule<TType> Instance { get; }

        protected TypeConstraintRule() : base(typeof(TType))
        { }

        protected TypeConstraintRule(SerializationInfo info, StreamingContext context) : base(info, context)
        { }

        protected override bool SerializeType => false;

        public override bool IsValid(object instance)
        {
            return instance is TType;
        }

        public override bool IsValid<T>(T instance)
        {
            return instance is TType;
        }

        object IObjectReference.GetRealObject(StreamingContext context)
        {
            return Instance;
        }

        public static ObjectConstraint CreateConstraint()
        {
            return new ObjectConstraint(Instance);
        }

        public override bool IsValidConstraint(ObjectConstraint other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            TypeConstraintRule tc = other.GetConstraint<TypeConstraintRule>();

            return Type.IsAssignableFrom(tc.Type);
        }

        public override bool IsValidConstraint(ObjectConstraintRule other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (other is TypeConstraintRule tc)
            {
                if (!Type.IsAssignableFrom(tc.Type))
                    return false;
            }

            return true;
        }

        public override bool Equals(ObjectConstraintRule other)
        {
            return object.ReferenceEquals(this, other);
        }

        public override int GetHashCode()
        {
            return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this);
        }

        public override ObjectConstraintRuleRelation GetRelation(ObjectConstraintRule other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (!(other is TypeConstraintRule r))
                return ObjectConstraintRuleRelation.Unknown;

            if (Type == r.Type)
                return ObjectConstraintRuleRelation.Equal;
            else if (Type.IsAssignableFrom(r.Type))
                return ObjectConstraintRuleRelation.LeftImpliesRight;
            else if (r.Type.IsAssignableFrom(Type))
                return ObjectConstraintRuleRelation.RightImpliesLeft;
            else
                return ObjectConstraintRuleRelation.Intersection;
        }
    }
}
