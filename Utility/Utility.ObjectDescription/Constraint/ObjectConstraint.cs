using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Utility;

namespace Utility.ObjectDescription.Constraint
{
    [Serializable]
    public sealed class ObjectConstraint : ISerializable, IEquatable<ObjectConstraint>
    {
        public static ObjectConstraint None { get; } = new ObjectConstraint(null, EmptyRule, false, false);
        public static ObjectConstraint All { get; } = new ObjectConstraint();

        private static readonly ObjectConstraintRule[] EmptyRule = new ObjectConstraintRule[] { DisjointRule.Instance };

        private readonly ObjectConstraintRule[] _rules;
        private int _hashCode;

        public ObjectConstraint(params ObjectConstraintRule[] rules)
            : this(null, rules)
        { }

        private ObjectConstraint(ObjectConstraint previous, params ObjectConstraintRule[] rules)
            : this(previous, rules ?? throw new ArgumentNullException(nameof(rules)), true, true)
        { }

        public ObjectConstraint(IEnumerable<ObjectConstraintRule> ruleSet)
            : this(null, ruleSet, true, true)
        { }

        public ObjectConstraint(ObjectConstraint previous, IEnumerable<ObjectConstraintRule> ruleSet)
            : this(previous, ruleSet, true, true)
        { }

        private ObjectConstraint(ObjectConstraint previous, IEnumerable<ObjectConstraintRule> ruleSet, bool filter, bool clone)
        {
            if (ruleSet == null)
                throw new ArgumentNullException(nameof(ruleSet));

            if (previous != null && previous.IsEmptySet)
            {
                _hashCode = ~0;
                _rules = EmptyRule;
            }
            else
            {
                if (filter)
                {
                    List<ObjectConstraintRule> rules;

                    if (previous == null)
                        rules = new List<ObjectConstraintRule>();
                    else
                        rules = new List<ObjectConstraintRule>(previous._rules);

                    foreach (ObjectConstraintRule r in ruleSet)
                    {
                        if (r == null)
                            throw new ArgumentException("The rules of the rule set must not be null.", nameof(ruleSet));

                        bool skip = false;

                        //transitive relationship assumed

                        for (int i = 0; i < rules.Count; i++)
                        {
                            ObjectConstraintRuleRelation relation = rules[i].GetRelation(r);

                            if (relation == ObjectConstraintRuleRelation.LeftImpliesRight || relation == ObjectConstraintRuleRelation.Equal)
                            {
                                skip = true;
                                break;
                            }
                            else if (relation == ObjectConstraintRuleRelation.RightImpliesLeft)
                            {
                                i--;
                                rules.RemoveAt(i);
                            }
                            else if (relation == ObjectConstraintRuleRelation.Disjoint)
                            {
                                _rules = EmptyRule;
                                break;
                            }
                        }

                        if (!skip)
                            rules.Add(r);
                    }

                    _rules = rules.ToArray();

                    int hc = 0;
                    for (int i = 0; i < _rules.Length; i++)
                    {
                        hc ^= _rules[i].GetHashCode();
                    }
                    _hashCode = hc;
                }
                else
                {
                    if (previous == null)
                    {
                        if (ruleSet is ObjectConstraintRule[] r)
                        {
                            _rules = clone ? (ObjectConstraintRule[])r.Clone() : r;
                        }
                        else
                        {
                            _rules = ruleSet.ToArray();
                        }
                    }
                    else
                    {
                        //speed up process for...
                        if (ruleSet is ObjectConstraintRule[] r) //... arrays: merge both arrays
                        {
                            ObjectConstraintRule[] newRules = new ObjectConstraintRule[previous._rules.Length + r.Length];

                            Array.Copy(previous._rules, 0, newRules, 0, previous._rules.Length);
                            Array.Copy(r, 0, newRules, previous._rules.Length, r.Length);

                            _rules = newRules;
                        }
                        else if (ruleSet is ICollection<ObjectConstraintRule> ruleCollection) //... collections: use CopyTo and Array.Copy
                        {
                            ObjectConstraintRule[] newRules = new ObjectConstraintRule[previous._rules.Length + ruleCollection.Count];

                            //reversed order to establish array integrity
                            ruleCollection.CopyTo(newRules, previous._rules.Length);
                            Array.Copy(previous._rules, 0, newRules, 0, previous._rules.Length);

                            _rules = newRules;
                        }
                        else
                        {
                            //... readonly collections, if necessary. CopyTo is not supported on IReadOnlyCollections and therefore copying will be performed on IEnumerable<T>
                            int c = (ruleSet as IReadOnlyCollection<ObjectConstraintRule>)?.Count ?? 0;
                            int offset = previous._rules.Length;

                            ObjectConstraintRule[] newRules = new ObjectConstraintRule[offset + c];

                            int i = 0;

                            foreach (ObjectConstraintRule rule in ruleSet)
                            {
                                if (i > c && i >= newRules.Length)
                                {
                                    Array.Resize(ref newRules, newRules.Length * 2);
                                }

                                newRules[offset + i] = rule;
                                i++;
                            }

                            Array.Copy(previous._rules, 0, newRules, 0, previous._rules.Length);

                            //reestablish integrity, if necessary
                            if (newRules.Length != offset + i)
                            {
                                Array.Resize(ref newRules, offset + i);
                            }

                            _rules = newRules;
                        }
                    }

                    int hc = previous._hashCode;

                    for (int i = previous._rules.Length; i < _rules.Length; i++)
                    {
                        ObjectConstraintRule rule = _rules[i];

                        if (rule == null)
                            throw new ArgumentException("The rules of the rule set must not be null.", nameof(ruleSet));

                        hc ^= rule.GetHashCode();
                    }
                }
            }
        }

        private ObjectConstraint(SerializationInfo info, StreamingContext context)
            : this(null, (info ?? throw new ArgumentNullException(nameof(info))).GetValue<ObjectConstraintRule[]>("Rules"), false, false)
        { }

        public bool IsEmptySet => _rules.Length == 1 && _rules[0] == DisjointRule.Instance;
        public bool ContainsConstraint => _rules.Length != 0;

        public IList<ObjectConstraintRule> GetRules()
        {
            return _rules.AsReadonlyList();
        }

        public IEnumerable<T> GetConstraints<T>()
            where T : ObjectConstraintRule
        {
            foreach (ObjectConstraintRule r in _rules)
            {
                if (r is T v)
                {
                    if (r.IsInherited || r.GetType() == typeof(T))
                    {
                        yield return v;
                    }
                }
            }
        }

        public IEnumerable<ObjectConstraintRule> GetConstraints(Type constraintType)
        {
            if (constraintType == null)
                throw new ArgumentNullException(nameof(constraintType));

            if (!typeof(ObjectConstraintRule).IsAssignableFrom(constraintType))
                throw new ArgumentException("Constraint type expected.", nameof(constraintType));

            foreach (ObjectConstraintRule r in _rules)
            {
                if (r.IsInherited ? constraintType.IsInstanceOfType(r) : r.GetType() == constraintType)
                    yield return r;
            }
        }

        public T GetConstraint<T>()
            where T : ObjectConstraintRule
        {
            foreach (ObjectConstraintRule r in _rules)
            {
                if (r is T v)
                {
                    if (r.IsInherited || r.GetType() == typeof(T))
                    {
                        return v;
                    }
                }
            }

            return null;
        }

        public ObjectConstraintRule GetConstraint(Type constraintType)
        {
            if (constraintType == null)
                throw new ArgumentNullException(nameof(constraintType));

            foreach (ObjectConstraintRule r in _rules)
            {
                if (r.IsInherited ? constraintType.IsInstanceOfType(r) : r.GetType() == constraintType)
                    return r;
            }

            if (!typeof(ObjectConstraintRule).IsAssignableFrom(constraintType))
                throw new ArgumentException("Constraint type expected.", nameof(constraintType));

            return null;
        }

        public bool IsValid(object instance)
        {
            foreach (ObjectConstraintRule c in _rules)
            {
                if (!c.IsValid(instance))
                    return false;
            }

            return true;
        }

        public bool IsValid<T>(T instance)
        {
            foreach (ObjectConstraintRule c in _rules)
            {
                if (!c.IsValid(instance))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks, whether the supplied constraint defines a subset of the current constraint, i.e.,
        /// the supplied constraint is accepted by all constraint rules defined on the current constraint.
        /// See <see cref="ObjectConstraintRule.IsValidConstraint(ObjectConstraint)"/> for further information.
        /// </summary>
        /// <param name="constraint">The constraint to test.</param>
        /// <returns>Returns, whether the constraint is accepted by all constraint rules on the current constraint.</returns>
        public bool IsValidConstraint(ObjectConstraint constraint)
        {
            foreach (ObjectConstraintRule c in _rules)
            {
                if (!c.IsValidConstraint(constraint))
                    return false;
            }

            return true;
        }


        /// <summary>
        /// Checks, whether the supplied constraint rule defines a subset of the current constraint, i.e.,
        /// the supplied constraint rule is accepted by all constraint rules defined on the current constraint.
        /// See <see cref="ObjectConstraintRule.IsValidConstraint(ObjectConstraintRule)"/> for further information.
        /// </summary>
        /// <param name="constraint">The constraint rule to test.</param>
        /// <returns>Returns, whether the constraint rule is accepted by all constraint rules on the current constraint.</returns>
        public bool IsValidConstraint(ObjectConstraintRule constraintRule)
        {
            foreach (ObjectConstraintRule c in _rules)
            {
                if (!c.IsValidConstraint(constraintRule))
                    return false;
            }

            return true;
        }

        public void PassArgument(object instance)
        {
            foreach (ObjectConstraintRule c in _rules)
            {
                c.PassArgument(instance);
            }
        }

        public void PassArgument<T>(T instance)
        {
            foreach (ObjectConstraintRule c in _rules)
            {
                c.PassArgument<T>(instance);
            }
        }

        public bool HasConstraint<T>()
            where T : ObjectConstraintRule
        {
            return GetConstraint<T>() != null;
        }

        public bool HasConstraint(Type constraintType)
        {
            return GetConstraint(constraintType) != null;
        }

        private void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            info.AddValue("Rules", _rules);
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(info, context);
        }

        public static ObjectConstraint Combine(ObjectConstraint previous, params ObjectConstraintRule[] rules)
        {
            return Combine(previous, rules);
        }

        public static ObjectConstraint Combine(ObjectConstraint previous, IEnumerable<ObjectConstraintRule> rules)
        {
            if (previous == null)
            {
                if (rules == null)
                {
                    return null;
                }
                else
                {
                    return new ObjectConstraint(rules);
                }
            }
            else if (rules == null)
            {
                return previous;
            }
            else
            {
                ObjectConstraintRule[] rulesArray = rules is ObjectConstraintRule[] r ? (ObjectConstraintRule[])r.Clone() : rules.ToArray();

                if (rulesArray.Length == 0)
                {
                    return previous;
                }

                return new ObjectConstraint(previous, rules);
            }
        }

        public static ObjectConstraint And(ObjectConstraint left, ObjectConstraint right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));

            return new ObjectConstraint(left, right._rules, true, true);
        }

        public bool Equals(ObjectConstraint other)
        {
            if (other == null)
                return false;

            if (_hashCode != other._hashCode)
                return false;

            int otherLength = other._rules.Length;
            int thisLength = _rules.Length;

            if (_rules.Length != otherLength)
                return false;

            var otherHandled = new System.Collections.BitArray(otherLength);
            int remainder = otherLength;

            for (int i = 0; i < thisLength; i++)
            {
                bool handled = false;
                for (int j = 0; j < otherLength; j++)
                {
                    if (otherHandled[j])
                        continue;

                    if (_rules[i].Equals(other._rules[j]))
                    {
                        otherHandled[j] = true;
                        handled = true;
                        remainder--;
                        break;
                    }

                    if (!handled)
                        return false;
                }
            }

            for (int j = 0; remainder > 0 && j < otherLength; j++)
            {
                if (otherHandled[j])
                    continue;

                remainder--;

                for (int i = 0; i < thisLength; i++)
                {
                    if (otherHandled[j])
                        continue;

                    bool handled = false;

                    if (_rules[j].Equals(other._rules[i]))
                    {
                        otherHandled[j] = true;
                        handled = true;
                        remainder--;
                        break;
                    }

                    if (!handled)
                        return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is ObjectConstraint constr && Equals(constr);
        }

        private sealed class DisjointRule : ObjectConstraintRule
        {
            public static DisjointRule Instance { get; } = new DisjointRule();

            private DisjointRule() { }

            public override bool IsInherited => false;

            public override bool Equals(ObjectConstraintRule other)
            {
                if (other == null)
                    return false;

                return other is DisjointRule;
            }

            public override int GetHashCode()
            {
                return ~0;
            }

            public override ObjectConstraintRuleRelation GetRelation(ObjectConstraintRule other)
            {
                return ObjectConstraintRuleRelation.Disjoint;
            }

            public override bool IsValid(object instance)
            {
                return false;
            }

            public override bool IsValid<T>(T instance)
            {
                return false;
            }

            public override bool IsValidConstraint(ObjectConstraint other)
            {
                if (other == null)
                    throw new ArgumentNullException(nameof(other));

                return false;
            }

            public override bool IsValidConstraint(ObjectConstraintRule other)
            {
                if (other == null)
                    throw new ArgumentNullException(nameof(other));

                return false;
            }
        }
    }
}
