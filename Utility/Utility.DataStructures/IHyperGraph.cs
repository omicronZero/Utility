using System;
using System.Collections.Generic;
using System.Text;
using Utility.Collections;

namespace Utility
{
    public interface IHyperGraph<TNode>
    {
        ISetCollection<TNode> Nodes { get; }
        ISetCollection<HyperEdge<TNode>> Edges { get; }

        bool IsDirected { get; }

        IEnumerable<HyperEdge<TNode>> GetOutEdges(TNode node);
        IEnumerable<HyperEdge<TNode>> GetInEdges(TNode node);
    }
}
