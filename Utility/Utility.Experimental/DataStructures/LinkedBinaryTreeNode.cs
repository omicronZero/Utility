using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.DataStructures
{
    internal class LinkedBinaryTreeNode<T>
    {
        public LinkedBinaryTreeNode<T> Parent { get; private set; }
        public LinkedBinaryTreeNode<T> Left { get; private set; }
        public LinkedBinaryTreeNode<T> Right { get; private set; }

        public LinkedBinaryTreeNode<T> GetSibling() => Parent == null ? null : (IsLeftChild ? Parent.Right : Parent.Left);

        public bool IsLeftChild => Parent != null && Parent.Left == this;
        public bool IsRightChild => Parent != null && Parent.Right == this;
        public bool IsDisconnected => Parent == null && Left == null && Right == null;
        public bool IsRoot => Parent == null;
        public bool IsLeaf => Left == null && Right == null;

        public LinkedBinaryTreeNode<T> GetRightmostDescendent()
        {
            var c = this;

            while (c.Right != null)
                c = c.Right;

            return c;
        }

        public LinkedBinaryTreeNode<T> GetLeftmostDescendent()
        {
            var c = this;

            while (c.Left != null)
                c = c.Left;

            return c;
        }

        public LinkedBinaryTreeNode<T> GetSuccessor()
        {
            if (Right != null)
                return Right.GetLeftmostDescendent();

            var c = this;

            do
            {
                if (c.IsLeftChild)
                {
                    if (c.Parent.Right == null)
                        return c.Parent;
                    else
                        return c.Parent.Right.GetLeftmostDescendent();
                }

                c = c.Parent;
            } while (c != null);

            return null;
        }

        public LinkedBinaryTreeNode<T> GetPredecessor()
        {
            if (Left != null)
                return Left.GetRightmostDescendent();

            var c = this;

            do
            {
                if (c.IsRightChild)
                {
                    if (c.Parent.Left == null)
                        return c.Parent;
                    else
                        return c.Parent.Left.GetRightmostDescendent();
                }

                c = c.Parent;
            } while (c != null);

            return null;
        }

        public int GetCount()
        {
            var stack = new Stack<LinkedBinaryTreeNode<T>>();

            stack.Push(this);

            int c = 0;

            do
            {
                c++;
                var current = stack.Pop();

                if (current.Left != null)
                    stack.Push(current.Left);
                if (current.Right != null)
                    stack.Push(current.Right);

            } while (stack.Count > 0);

            return c;
        }

        public int GetChildCount()
        {
            int c = 0;

            if (Left != null)
                c++;

            if (Right != null)
                c++;

            return c;
        }

        public int GetDepth()
        {
            int d = 0;

            for (var c = this; c != null; c = c.Parent)
                d++;

            return d;
        }

        public int GetHeight()
        {
            if (Left == null && Right == null)
                return 0;

            var q = new Queue<(LinkedBinaryTreeNode<T>, int)>();

            int maxHeight = 0;

            q.Enqueue((this, 0));

            while (q.Count > 0)
            {
                var (node, depth) = q.Dequeue();

                if (node.Left == null && node.Right == null)
                {
                    if (depth > maxHeight)
                        maxHeight = depth;
                }

                if (node.Left != null)
                    q.Enqueue((node.Left, depth + 1));

                if (node.Right != null)
                    q.Enqueue((node.Right, depth + 1));
            }

            return maxHeight;
        }

        public LinkedBinaryTreeNode<T> GetChild(int index)
        {
            if (index < 0 || index >= 2)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be 0 (left) or 1 (right).");

            return index == 0 ? Left : Right;
        }

        public LinkedBinaryTreeNode<T> GetGrandparent() => Parent?.Parent;

        public LinkedBinaryTreeNode<T> GetUncle() => Parent.IsLeftChild ? GetGrandparent().Right : GetGrandparent()?.Left;

        public LinkedBinaryTreeNode<T> GetRoot()
        {
            var current = this;

            while (current.Parent != null)
                current = current.Parent;

            return current;
        }

        public IEnumerable<LinkedBinaryTreeNode<T>> PathToRoot()
        {
            for (var c = this; c != null; c = c.Parent)
                yield return c;
        }

        public IEnumerable<LinkedBinaryTreeNode<T>> PathToCurrent()
        {
            var stack = new Stack<LinkedBinaryTreeNode<T>>();

            for (var c = this; c != null; c = c.Parent)
                stack.Push(c);

            return stack;
        }

        public void InsertLeft(LinkedBinaryTreeNode<T> value)
        {
            Insert(value, null);
        }

        public void InsertRight(LinkedBinaryTreeNode<T> value)
        {
            Insert(null, value);
        }

        public void Insert(LinkedBinaryTreeNode<T> left, LinkedBinaryTreeNode<T> right)
        {
            if (left != null)
            {
                if (left.Parent != null)
                    throw new ArgumentException("The indicated left node is not a tree root.", nameof(left));

                if (Left != null)
                    throw new InvalidOperationException("The left node is already set to an instance.");
            }

            if (right != null)
            {
                if (right.Parent != null)
                    throw new ArgumentException("The indicated right node is not a tree root.", nameof(right));

                if (Right != null)
                    throw new InvalidOperationException("The right node is already set to an instance.");
            }

            if (left != null)
            {
                Left = left;
                left.Parent = this;
            }

            if (right != null)
            {
                Right = right;
                right.Parent = this;
            }
        }

        public void InsertBetweenLeft(LinkedBinaryTreeNode<T> node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.Parent != null)
                throw new ArgumentException("The inserted node must not have a parent.", nameof(node));
            if (node.Left != null)
                throw new ArgumentException("The inserted node must not have a left child.", nameof(node));

            node.Left = Left;

            if (Left != null)
                Left.Parent = node;

            node.Parent = this;
            Left = node;
        }

        public void InsertBetweenRight(LinkedBinaryTreeNode<T> node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.Parent != null)
                throw new ArgumentException("The inserted node must not have a parent.", nameof(node));
            if (node.Right != null)
                throw new ArgumentException("The inserted node must not have a right child.", nameof(node));

            node.Right = Right;

            if (Right != null)
                Right.Parent = node;

            node.Parent = this;
            Right = node;
        }

        public void RotateLeft()
        {
            var right = Right;

            if (right == null)
                throw new InvalidOperationException("The right node is not set to a child.");

            //we have a tree (L, x, (M, y, R)) and transform it to ((L, x, M), y, R)
            //(we are at x)
            /* the following changes are being made:
             * - M is attached to x as its new child
             * - y is attached to the parent of x
             * - x is attached to y
             * 
             * So, L and R remain in the same parent
             */

            //we first update the pointers pointing to the local change
            if (Parent != null) // p -> y
            {
                if (Parent.Left == this)
                    Parent.Left = right;
                else
                    Parent.Right = right;
            }

            var m = right.Left;

            if (m != null) // x -> M
                m.Parent = this;

            //now we update the element pointers
            right.Parent = Parent;
            Parent = right;
            Right = m;
            right.Left = this;


            /*
             * So, in total we have (with parent p, left node l, right node r)
             * - x.p = y, x.l = L, x.r = M
             * - y.p = p, y.l = x, y.r = R
             * - M.p = x
             * - L.p = x (unchanged)
             * - R.p = y (unchanged)
             */
        }

        public void RotateRight()
        {
            var left = Left;

            if (left == null)
                throw new InvalidOperationException("The left node is not set to a child.");

            //symmetric to RotateLeft
            //we have a tree ((L, y, M), x, R) and transform it to (L, y, (M, x, R))
            //(we are at x)

            if (Parent != null)
            {
                if (Parent.Left == this)
                    Parent.Left = left;
                else
                    Parent.Right = left;
            }

            var m = left.Right;

            if (m != null)
                m.Parent = this;

            left.Parent = Parent;
            Parent = left;
            Left = m;
            left.Right = this;


            /*
             * So, in total we have (with parent p, left node l, right node r)
             * - x.p = y, x.l = M, x.r = R
             * - y.p = p, y.l = L, y.r = x
             * - M.p = x
             * - L.p = y (unchanged)
             * - R.p = x (unchanged)
             */
        }

        public void SwapChildren()
        {
            (Right, Left) = (Left, Right);
        }

        public void SwapWith(LinkedBinaryTreeNode<T> node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var parent = node.Parent;
            var left = node.Left;
            var right = node.Right;

            //first, we switch the pointers to the current instance to that of the node
            if (Parent != null)
            {
                if (Parent.Left == this)
                    Parent.Left = node;
                else
                    Parent.Right = node;
            }

            if (Left != null)
                Left.Parent = node;
            if (Right != null)
                Right.Parent = node;

            //then, we switch the pointers to the node to that of the current instance
            if (parent != null)
            {
                if (parent.Left == node)
                    parent.Left = this;
                else
                    parent.Right = this;
            }

            if (left != null)
                left.Parent = this;
            if (right != null)
                right.Parent = this;

            //now we edit node and the current instance
            node.Left = Left;
            node.Right = Right;
            node.Parent = Parent;

            Left = left;
            Right = right;
            Parent = parent;
        }

        public void Delete()
        {
            if (Left != null && Right != null)
                throw new InvalidOperationException("For a successful deletion, at least the left or right node must be null.");

            var onlyChild = Left ?? Right;

            if (Parent != null)
            {
                if (Parent.Left == this)
                    Parent.Left = onlyChild;
                else
                    Parent.Right = onlyChild;
            }

            if (onlyChild != null)
                onlyChild.Parent = Parent;

            Parent = null;
            Left = Right = null;
        }

        public void RemoveFromParent()
        {
            if (Parent != null)
            {
                if (Parent.Left == this)
                    Parent.Left = null;
                else
                    Parent.Right = null;

                Parent = null;
            }
        }

        public void RemoveChildren()
        {
            if (Left != null)
                Left.Parent = null;
            if (Right != null)
                Right.Parent = null;

            Left = Right = null;
        }

        public (LinkedBinaryTreeNode<T>, LinkedBinaryTreeNode<T>, LinkedBinaryTreeNode<T>) Detach()
        {
            var state = (Left, Parent, Right);

            RemoveFromParent();
            RemoveChildren();

            return state;
        }

        public LinkedBinaryTreeNode<T> Find(Func<LinkedBinaryTreeNode<T>, int> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            var current = this;

            do
            {
                var comp = comparer(current);

                if (comp < 0)
                    current = current.Left;
                else if (comp > 0)
                    current = current.Right;
                else
                    break;
            } while (current != null);

            return current;
        }

        public LinkedBinaryTreeNode<T> FindLast(Func<LinkedBinaryTreeNode<T>, int> comparer, out int comparisonValue)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            var current = this;
            LinkedBinaryTreeNode<T> last;

            do
            {
                last = current;
                comparisonValue = comparer(current);

                if (comparisonValue < 0)
                    current = current.Left;
                else if (comparisonValue > 0)
                    current = current.Right;
                else
                    break;
            } while (current != null);

            return last;
        }

        public IEnumerable<LinkedBinaryTreeNode<T>> FindPath(Func<LinkedBinaryTreeNode<T>, int> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            var current = this;

            int comp;

            do
            {
                yield return current;

                comp = comparer(current);

                if (comp < 0)
                    current = current.Left;
                else if (comp > 0)
                    current = current.Right;
                else
                    break;
            } while (current != null);
        }

        public IEnumerable<LinkedBinaryTreeNode<T>> BreadthFirstSearch(bool rightBeforeLeft = false, bool leavesOnly = false)
        {
            Queue<LinkedBinaryTreeNode<T>> nodes = new Queue<LinkedBinaryTreeNode<T>>();

            nodes.Enqueue(this);

            if (rightBeforeLeft)
            {
                while (nodes.Count > 0)
                {
                    var node = nodes.Dequeue();

                    if (!leavesOnly || node.Left == null && node.Right == null)

                        if (node.Right != null)
                            nodes.Enqueue(node.Right);

                    if (node.Left != null)
                        nodes.Enqueue(node.Left);
                }
            }
            else
            {
                while (nodes.Count > 0)
                {
                    var node = nodes.Dequeue();

                    if (!leavesOnly || node.Left == null && node.Right == null)
                        yield return node;

                    if (node.Left != null)
                        nodes.Enqueue(node.Left);

                    if (node.Right != null)
                        nodes.Enqueue(node.Right);
                }
            }
        }

        public IEnumerable<LinkedBinaryTreeNode<T>> DepthFirstSearch(bool rightBeforeLeft = false, bool leavesOnly = false)
        {
            Stack<LinkedBinaryTreeNode<T>> nodes = new Stack<LinkedBinaryTreeNode<T>>();

            nodes.Push(this);

            if (rightBeforeLeft)
            {
                while (nodes.Count > 0)
                {
                    var node = nodes.Pop();

                    if (!leavesOnly || node.Left == null && node.Right == null)

                        if (node.Right != null)
                            nodes.Push(node.Right);

                    if (node.Left != null)
                        nodes.Push(node.Left);
                }
            }
            else
            {
                while (nodes.Count > 0)
                {
                    var node = nodes.Pop();

                    if (!leavesOnly || node.Left == null && node.Right == null)
                        yield return node;

                    if (node.Left != null)
                        nodes.Push(node.Left);

                    if (node.Right != null)
                        nodes.Push(node.Right);
                }
            }
        }
    }
}
