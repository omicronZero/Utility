using System;
using System.Collections.Generic;

namespace Utility
{
    [Serializable]
    public struct Edge<TNode> : IEquatable<Edge<TNode>>
    {
        public TNode Node1 { get; }
        public TNode Node2 { get; }

        public Edge(TNode node1, TNode node2)
        {
            Node1 = node1;
            Node2 = node2;
        }

        public Edge<TNode> Reversed() => (Node2, Node1);

        public override int GetHashCode()
        {
            return (Node1?.GetHashCode() ?? 0) ^ (Node2?.GetHashCode() ?? 0);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var e = obj as Edge<TNode>?;

            if (e == null)
                return false;

            return Equals(e.Value);
        }

        public bool Equals(Edge<TNode> other)
        {
            return EqualityComparer<TNode>.Default.Equals(Node1, other.Node1) &&
                    EqualityComparer<TNode>.Default.Equals(Node2, other.Node2);
        }

        public override string ToString()
        {
            return $"Node1: { Node1 }, Node2: { Node2 }";
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public void Deconstruct(out TNode node1, out TNode node2)
		{
            node1 = Node1;
            node2 = Node2;
		}

        public static implicit operator Edge<TNode>((TNode, TNode) node)
        {
            return new Edge<TNode>(node.Item1, node.Item2);
        }

        public static bool operator ==(Edge<TNode> left, Edge<TNode> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Edge<TNode> left, Edge<TNode> right)
        {
            return !(left == right);
        }

        public static Edge<TNode> operator~(Edge<TNode> value) => value.Reversed();
    }
}