using System;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    public interface ITree
    {
        ITreeNode Root { get; }

        IEnumerable<ITreeNode> Nodes { get; }
    }

    public interface ITree<T> : ITree
    {
        new ITreeNode<T> Root { get; }

        ITreeNode ITree.Root => Root;
    }
}
