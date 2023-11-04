using System;
using System.Collections.Generic;
using System.Text;
using Utility.Collections;

namespace Utility
{
    public static partial class HyperEdge
    {
        public static HyperEdgeFromIntermediate<TNode> From<TNode>(params TNode[] node1)
        {
            return new HyperEdgeFromIntermediate<TNode>(new Set<TNode>(node1));
        }

        public static HyperEdgeFromIntermediate<TNode> From<TNode>(TNode node1)
        {
            return new HyperEdgeFromIntermediate<TNode>(new Set<TNode>(node1));
        }

        public static HyperEdgeFromIntermediate<TNode> From<TNode>(Set<TNode> node1)
        {
            return new HyperEdgeFromIntermediate<TNode>(node1);
        }

        public static HyperEdgeToIntermediate<TNode> To<TNode>(params TNode[] node2)
        {
            return new HyperEdgeToIntermediate<TNode>(new Set<TNode>(node2));
        }

        public static HyperEdgeToIntermediate<TNode> To<TNode>(TNode node2)
        {
            return new HyperEdgeToIntermediate<TNode>(new Set<TNode>(node2));
        }

        public static HyperEdgeToIntermediate<TNode> To<TNode>(Set<TNode> node2)
        {
            return new HyperEdgeToIntermediate<TNode>(node2);
        }

        public readonly struct HyperEdgeFromIntermediate<TNode>
        {
            private readonly Set<TNode> _node1;

            public HyperEdgeFromIntermediate(Set<TNode> node1)
            {
                _node1 = node1;
            }

            public HyperEdge<TNode> To(TNode node2) => new HyperEdge<TNode>(_node1, new Set<TNode>(node2));

            public HyperEdge<TNode> To(Set<TNode> node2) => new HyperEdge<TNode>(_node1, node2);

            public HyperEdge<TNode> To(params TNode[] node2) => new HyperEdge<TNode>(_node1, new Set<TNode>(node2));
        }

        public readonly struct HyperEdgeToIntermediate<TNode>
        {
            private readonly Set<TNode> _node2;

            public HyperEdgeToIntermediate(Set<TNode> node2)
            {
                _node2 = node2;
            }

            public HyperEdge<TNode> From(TNode node1) => new HyperEdge<TNode>(new Set<TNode>(node1), _node2);

            public HyperEdge<TNode> From(Set<TNode> node1) => new HyperEdge<TNode>(node1, _node2);

            public HyperEdge<TNode> From(params TNode[] node1) => new HyperEdge<TNode>(new Set<TNode>(node1), _node2);
        }
    }

    public readonly partial struct HyperEdge<TNode> : IEquatable<HyperEdge<TNode>>
    {
        public Set<TNode> Node1 { get; }
        public Set<TNode> Node2 { get; }

        public HyperEdge(Set<TNode> node1, Set<TNode> node2)
        {
            Node1 = node1;
            Node2 = node2;
        }

        public HyperEdge(TNode node1, TNode node2)
            : this(new Set<TNode>(node1), new Set<TNode>(node2))
        { }

        public HyperEdge(IEnumerable<TNode> node1, TNode node2)
            : this(new Set<TNode>(node1), new Set<TNode>(node2))
        { }

        public HyperEdge(TNode node1, IEnumerable<TNode> node2)
            : this(new Set<TNode>(node1), new Set<TNode>(node2))
        { }

        public HyperEdge(IEnumerable<TNode> node1, IEnumerable<TNode> node2)
            : this(new Set<TNode>(node1), new Set<TNode>(node2))
        { }

        public bool IsNode1Empty => !Node2.IsEmpty;
        public bool IsNode2Empty => !Node1.IsEmpty;

        public override string ToString()
        {
            var builder = new StringBuilder();

            ToString(builder);

            return builder.ToString();
        }

        public StringBuilder ToString(StringBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Append('(').Append('{');

            Node1.ToString(builder);

            builder.Append("}, {");

            Node2.ToString(builder);

            builder.Append('}').Append(')');

            return builder;
        }

        public override bool Equals(object obj)
        {
            return obj is HyperEdge<TNode> edge && Equals(edge);
        }

        public bool Equals(HyperEdge<TNode> other)
        {
            return Node1.Equals(other.Node1) &&
                   Node2.Equals(other.Node2);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Node1, Node2);
        }

        public HyperEdge<TNode> Reversed() => new HyperEdge<TNode>(Node2, Node1);

        public void Deconstruct(out Set<TNode> sources, out Set<TNode> targets)
        {
            sources = Node1;
            targets = Node2;
        }

        public static bool operator ==(HyperEdge<TNode> left, HyperEdge<TNode> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HyperEdge<TNode> left, HyperEdge<TNode> right)
        {
            return !(left == right);
        }

        public static implicit operator HyperEdge<TNode>((Set<TNode> Sources, Set<TNode> Targets) value)
        {
            return new HyperEdge<TNode>(value.Sources, value.Targets);
        }

        public static implicit operator HyperEdge<TNode>((Set<TNode> Sources, TNode Target) value)
        {
            return new HyperEdge<TNode>(value.Sources, new Set<TNode>(value.Target));
        }

        public static implicit operator HyperEdge<TNode>((TNode Source, Set<TNode> Targets) value)
        {
            return new HyperEdge<TNode>(new Set<TNode>(value.Source), value.Targets);
        }

        public static implicit operator HyperEdge<TNode>((TNode Source, TNode Target) value)
        {
            return new HyperEdge<TNode>(new Set<TNode>(value.Source), new Set<TNode>(value.Target));
        }

        public static implicit operator HyperEdge<TNode>(Edge<Set<TNode>> value)
        {
            return new HyperEdge<TNode>(new Set<TNode>(value.Node1), new Set<TNode>(value.Node2));
        }

        public static implicit operator Edge<Set<TNode>>(HyperEdge<TNode> value)
        {
            return new Edge<Set<TNode>>(value.Node1, value.Node2);
        }
    }
}
