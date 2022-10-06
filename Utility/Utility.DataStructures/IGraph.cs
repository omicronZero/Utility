using System;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    public interface IGraph<TNode>
    {
        ISetCollection<TNode> Nodes { get; }
        ISetCollection<Edge<TNode>> Edges { get; }
        bool IsDirected { get; }

        IEnumerable<TNode> GetIns(TNode node);
        IEnumerable<TNode> GetOuts(TNode node);

    }
}
