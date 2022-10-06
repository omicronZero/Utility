using System;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    [Serializable]
    public struct WeightedEdge<TNode, TWeight> : IEquatable<WeightedEdge<TNode, TWeight>>
    {
        public TNode Node1 { get; }
        public TNode Node2 { get; }
        public TWeight Weight { get; }

        public WeightedEdge(TNode node1, TNode node2, TWeight weight)
        {
            Node1 = node1;
            Node2 = node2;
            Weight = weight;
        }

        public WeightedEdge(Edge<TNode> edge, TWeight weight)
            : this(edge.Node1, edge.Node2, weight)
        { }

        public WeightedEdge<TNode, TWeight> Reversed() => new WeightedEdge<TNode, TWeight>(Node2, Node1, Weight);

        public Edge<TNode> GetEdge()
        {
            return new Edge<TNode>(Node1, Node2);
        }

        public override int GetHashCode()
        {
            return (Node1?.GetHashCode() ?? 0) ^ (Node2?.GetHashCode() ?? 0) ^ ~(Weight?.GetHashCode() ?? 0);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var e = obj as WeightedEdge<TNode, TWeight>?;

            if (e == null)
                return false;

            return Equals(e.Value);
        }

        public bool Equals(WeightedEdge<TNode, TWeight> other)
        {
            return EqualityComparer<TNode>.Default.Equals(Node1, other.Node1) &&
                    EqualityComparer<TNode>.Default.Equals(Node2, other.Node2) &&
                    EqualityComparer<TWeight>.Default.Equals(Weight, other.Weight);
        }

        public override string ToString()
        {
            return $"Node1: { Node1 }, Node2: { Node2 }, Weight: { Weight }";
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public void Deconstruct(out TNode node1, out TNode node2, out TWeight weight)
        {
            node1 = Node1;
            node2 = Node2;
            weight = Weight;
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public void Deconstruct(out Edge<TNode> edge, out TWeight weight)
        {
            edge = (Node1, Node2);
            weight = Weight;
        }

        public static implicit operator WeightedEdge<TNode, TWeight>((TNode, TNode, TWeight) node)
        {
            return new WeightedEdge<TNode, TWeight>(node.Item1, node.Item2, node.Item3);
        }

        public static implicit operator WeightedEdge<TNode, TWeight>((Edge<TNode>, TWeight) node)
        {
            return new WeightedEdge<TNode, TWeight>(node.Item1.Node1, node.Item1.Node2, node.Item2);
        }

        public static bool operator ==(WeightedEdge<TNode, TWeight> left, WeightedEdge<TNode, TWeight> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(WeightedEdge<TNode, TWeight> left, WeightedEdge<TNode, TWeight> right)
        {
            return !(left == right);
        }

        public static WeightedEdge<TNode, TWeight> operator ~(WeightedEdge<TNode, TWeight> value) => value.Reversed();
    }
}
