using System.Collections.Generic;

namespace Utility
{
    public interface ITreeNode
    {
        IReadOnlyCollection<ITreeNode> Children { get; }
        bool IsLeaf { get; }
    }

    public interface ITreeNode<T> : ITreeNode
    {
        new IReadOnlyCollection<ITreeNode<T>> Children { get; }

        IReadOnlyCollection<ITreeNode> ITreeNode.Children => Children;

        T Value { get; }
    }
}