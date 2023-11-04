using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xunit;

namespace Utility.DataStructures.Tests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>")]
    public class HypergraphTests
    {
        [DebuggerHidden]
        private static void AssertEmpty<T>(ICollection<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            Assert.False(collection.Any());
            Assert.Equal(0, collection.Count);
        }

        [DebuggerHidden]
        private static void AssertNotEmpty<T>(ICollection<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            Assert.True(collection.Any());
            Assert.NotEqual(0, collection.Count);
        }

        [DebuggerHidden]
        private static void AssertSetEquals<T>(IEnumerable<T> expected, ICollection<T> collection)
        {
            if (expected == null)
                throw new ArgumentNullException(nameof(expected));
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            Assert.True(expected.ToHashSet().SetEquals(expected));
        }

        [Fact]
        public void TestEmptyDirected()
        {
            foreach (var directed in new bool[] { true, false })
            {
                var hyperGraph = new HyperGraph<int>(directed);

                AssertEmpty(hyperGraph.Edges);
                AssertEmpty(hyperGraph.Nodes);
            }
        }

        [Fact]
        public void TestAddNodesDirected()
        {
            foreach (var directed in new bool[] { true, false })
            {
                var hyperGraph = new HyperGraph<int>(directed);

                Assert.True(hyperGraph.Nodes.Add(1));

                Assert.Single(hyperGraph.Nodes);
                AssertEmpty(hyperGraph.Edges);
                AssertSetEquals(new int[] { 1 }, hyperGraph.Nodes);

                Assert.False(hyperGraph.Nodes.Add(1));
                Assert.Single(hyperGraph.Nodes);

                Assert.True(hyperGraph.Nodes.Add(3));
                Assert.False(hyperGraph.Nodes.Add(3));
                Assert.Equal(2, hyperGraph.Nodes.Count);
            }
        }

        [Fact]
        public void TestNodesEnumerable()
        {
            foreach (var directed in new bool[] { true, false })
            {
                var hyperGraph = new HyperGraph<int>(directed);

                hyperGraph.Nodes.AddSet(1, 2, 3, 4);

                Assert.True(hyperGraph.Nodes.ToHashSet().SetEquals(new int[] { 1, 2, 3, 4 }));
            }
        }

        [Fact]
        public void TestRemoveNodeEmptyEdges()
        {
            foreach (var directed in new bool[] { true, false })
            {
                var hyperGraph = new HyperGraph<int>(directed);

                hyperGraph.Nodes.AddSet(1, 2, 3, 4);

                Assert.True(hyperGraph.Nodes.Remove(2));

                Assert.DoesNotContain(2, hyperGraph.Nodes);

                AssertSetEquals(new int[] { 1, 3, 4 }, hyperGraph.Nodes);
            }
        }

        //edges
        [Fact]
        public void TestAddEdgeDirected()
        {
            var hyperGraph = new HyperGraph<int>(true);

            hyperGraph.Nodes.AddSet(1, 2, 3, 4);

            Assert.True(hyperGraph.Edges.Add(HyperEdge.From(1, 4).To(2)));
            Assert.Single(hyperGraph.Edges);

            Assert.True(hyperGraph.Edges.Add(HyperEdge.From(1, 4).To(2, 3)));
            Assert.Equal(2, hyperGraph.Edges.Count);

            Assert.False(hyperGraph.Edges.Add(HyperEdge.From(1, 4).To(2, 3)));
            Assert.Equal(2, hyperGraph.Edges.Count);

            Assert.True(hyperGraph.Edges.Add(HyperEdge.From(4, 2).To(2, 3)));
            Assert.Equal(3, hyperGraph.Edges.Count);

            Assert.True(hyperGraph.Edges.Add(HyperEdge.To(1, 4).From(2, 3)));
            Assert.Equal(4, hyperGraph.Edges.Count);
        }

        [Fact]
        public void TestAddEdgeUndirected()
        {
            var hyperGraph = new HyperGraph<int>(false);

            hyperGraph.Nodes.AddSet(1, 2, 3, 4);

            Assert.True(hyperGraph.Edges.Add(HyperEdge.Create((1, 4), 2)));
            Assert.Single(hyperGraph.Edges);

            Assert.True(hyperGraph.Edges.Add(HyperEdge.Create((1, 4), (2, 3))));
            Assert.Equal(2, hyperGraph.Edges.Count);

            Assert.False(hyperGraph.Edges.Add(HyperEdge.Create((1, 4), (2, 3))));
            Assert.Equal(2, hyperGraph.Edges.Count);

            Assert.False(hyperGraph.Edges.Add(HyperEdge.Create((4, 1), (2, 3))));
            Assert.Equal(2, hyperGraph.Edges.Count);

            Assert.False(hyperGraph.Edges.Add(HyperEdge.Create((1, 4), (2, 3))));
            Assert.Equal(2, hyperGraph.Edges.Count);
        }

        [Fact]
        public void TestRemoveDirected()
        {
            var hyperGraph = new HyperGraph<int>(false);

            hyperGraph.Nodes.AddSet(1, 2, 3, 4);
            hyperGraph.Edges.AddSet(
                HyperEdge.From(1, 2).To(3, 4),
                HyperEdge.From(1, 2).To(3),
                HyperEdge.From(1, 3).To(3)
                );

            Assert.True(hyperGraph.Edges.Remove(HyperEdge.From(1, 2).To(3)));
            Assert.Equal(2, hyperGraph.Edges.Count);
            Assert.False(hyperGraph.Edges.Remove(HyperEdge.From(1, 2).To(3)));
            Assert.Equal(2, hyperGraph.Edges.Count);
        }

        [Fact]
        public void TestRemoveUndirected()
        {
            var hyperGraph = new HyperGraph<int>(false);

            hyperGraph.Nodes.AddSet(1, 2, 3, 4);
            hyperGraph.Edges.AddSet(
                HyperEdge.Create((1, 2), (3, 4)),
                HyperEdge.Create((1, 2), 3),
                HyperEdge.Create((1, 3), 3)
                );

            Assert.True(hyperGraph.Edges.Remove(HyperEdge.Create((1, 2), 3)));
            Assert.Equal(2, hyperGraph.Edges.Count);
            Assert.False(hyperGraph.Edges.Remove(HyperEdge.Create((1, 2), 3)));
            Assert.Equal(2, hyperGraph.Edges.Count);
            Assert.True(hyperGraph.Edges.Remove(HyperEdge.Create(3, (1, 3))));
            Assert.Single(hyperGraph.Edges);
        }

        [Fact]
        public void TestClearEdges()
        {
            foreach (bool directed in new bool[] { true, false })
            {
                var hyperGraph = new HyperGraph<int>(directed);

                hyperGraph.Nodes.AddSet(1, 2, 3, 4);
                hyperGraph.Edges.AddSet(
                    HyperEdge.Create((1, 2), (3, 4)),
                    HyperEdge.Create((1, 2), 3),
                    HyperEdge.Create((1, 3), 3)
                    );

                hyperGraph.Edges.Clear();
                AssertEmpty(hyperGraph.Edges);
            }
        }

        //Node-edge mixture
        [Fact]
        public void TestClearNodesAndSwipeEdges()
        {
            foreach (bool directed in new bool[] { true, false })
            {
                var hyperGraph = new HyperGraph<int>(directed);

                hyperGraph.Nodes.AddSet(1, 2, 3, 4);
                hyperGraph.Edges.AddSet(
                    HyperEdge.Create((1, 2), (3, 4)),
                    HyperEdge.Create((1, 2), 3),
                    HyperEdge.Create((1, 3), 3)
                    );

                hyperGraph.Nodes.Clear();
                AssertEmpty(hyperGraph.Nodes);
                AssertEmpty(hyperGraph.Edges);
            }
        }

        //Edges per node
        [Fact]
        public void TestNodeEdgesEdgeAddDirected()
        {
            var hyperGraph = new HyperGraph<int>(true);

            hyperGraph.Nodes.AddSet(1, 2, 3, 4);

            var node1Outs = hyperGraph.GetOutEdges(1);
            var node1Ins = hyperGraph.GetInEdges(1);

            var node2Outs = hyperGraph.GetOutEdges(2);
            var node2Ins = hyperGraph.GetInEdges(2);

            var node3Outs = hyperGraph.GetOutEdges(3);
            var node3Ins = hyperGraph.GetInEdges(3);

            AssertEmpty(node1Outs);
            AssertEmpty(node1Ins);

            hyperGraph.Edges.AddSet(
                HyperEdge.From(1, 2).To(3, 4),
                HyperEdge.From(1, 2).To(3),
                HyperEdge.From(1, 3).To(3),
                HyperEdge.From(3).To(3)
                );

            Assert.Contains(((1, 2), (3, 4)), node1Outs);
            Assert.Contains(((1, 2), 3), node1Outs);
            Assert.DoesNotContain(((1, 2), 3), node1Ins);
            Assert.DoesNotContain((3, 3), node1Outs);
            Assert.DoesNotContain((3, 3), node1Ins);

            Assert.DoesNotContain(((1, 2), 2), node1Ins);

            Assert.Contains(((1, 2), (3, 4)), node3Ins);
            Assert.Contains(((1, 2), 3), node3Ins);
            Assert.Contains((3, 3), node3Outs);
            Assert.Contains((3, 3), node3Ins);
        }

        [Fact]
        public void TestNodeEdgesEdgeAddUndirected()
        {
            var hyperGraph = new HyperGraph<int>(false);

            hyperGraph.Nodes.AddSet(1, 2, 3, 4);

            var node1Outs = hyperGraph.GetOutEdges(1);
            var node1Ins = hyperGraph.GetInEdges(1);

            var node2Outs = hyperGraph.GetOutEdges(2);
            var node2Ins = hyperGraph.GetInEdges(2);

            var node3Outs = hyperGraph.GetOutEdges(3);
            var node3Ins = hyperGraph.GetInEdges(3);

            AssertEmpty(node1Outs);
            AssertEmpty(node1Ins);

            hyperGraph.Edges.AddSet(
                HyperEdge.From(1, 2).To(3, 4),
                HyperEdge.From(1, 2).To(3),
                HyperEdge.From(1, 3).To(3),
                HyperEdge.From(3).To(3)
                );

            Assert.Contains(((1, 2), (3, 4)), node1Outs);
            Assert.Contains(((1, 2), 3), node1Outs);
            Assert.Contains(((1, 2), (3, 4)), node1Ins);
            Assert.Contains(((1, 2), 3), node1Ins);
            Assert.Contains(((3, 4), (1, 2)), node1Outs);
            Assert.Contains((3, (1, 2)), node1Outs);
            Assert.Contains(((3, 4), (1, 2)), node1Ins);
            Assert.Contains((3, (1, 2)), node1Ins);
            Assert.DoesNotContain((3, 3), node1Outs);
            Assert.DoesNotContain((3, 3), node1Ins);

            Assert.DoesNotContain(((1, 2), 2), node1Ins);

            Assert.Contains(((1, 2), (3, 4)), node3Ins);
            Assert.Contains(((1, 2), 3), node3Ins);
            Assert.Contains(((1, 2), (3, 4)), node3Outs);
            Assert.Contains(((1, 2), 3), node3Outs);
            Assert.Contains((3, 3), node3Outs);
            Assert.Contains((3, 3), node3Ins);
        }
    }
}
