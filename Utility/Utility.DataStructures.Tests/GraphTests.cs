using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xunit;

namespace Utility.DataStructures
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Style",
        "IDE0059:Unnecessary assignment of a value",
        Justification = "Unused variables are convenience for additional testing and are not interfering with legibility.")]
    public class GraphTestDirecteds
    {
        //TODO: add CopyTo-tests
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

        //Directed graph
        //Nodes
        [Fact]
        public void TestDirectedEmptyGraph()
        {
            var graph = new Graph<int>(true);

            AssertEmpty(graph.Edges);
            AssertEmpty(graph.Nodes);
        }

        [Fact]
        public void TestDirectedNodeAdd()
        {
            var graph = new Graph<int>(true);

            Assert.True(graph.Nodes.Add(1));
            Assert.Contains(1, graph.Nodes);

            Assert.True(graph.Nodes.Add(2));
            Assert.Contains(1, graph.Nodes);
            Assert.Contains(2, graph.Nodes);
        }

        [Fact]
        public void TestDirectedNodeAddRedundant()
        {
            var graph = new Graph<int>(true);

            Assert.True(graph.Nodes.Add(1));
            Assert.Contains(1, graph.Nodes);

            Assert.False(graph.Nodes.Add(1));
            Assert.Contains(1, graph.Nodes);
        }

        [Fact]
        public void TestDirectedNodeRemove()
        {
            var graph = new Graph<int>(true);

            Assert.True(graph.Nodes.Add(1));
            Assert.Contains(1, graph.Nodes);

            Assert.True(graph.Nodes.Remove(1));
            Assert.DoesNotContain(1, graph.Nodes);
        }

        [Fact]
        public void TestDirectedNodeRemoveUnavailable()
        {
            var graph = new Graph<int>(true);

            Assert.False(graph.Nodes.Remove(1));
            Assert.DoesNotContain(1, graph.Nodes);

            Assert.True(graph.Nodes.Add(1));
            Assert.Contains(1, graph.Nodes);

            Assert.False(graph.Nodes.Remove(2));
            Assert.DoesNotContain(2, graph.Nodes);
            Assert.Contains(1, graph.Nodes);
        }

        [Fact]
        public void TestDirectedNodesClear()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Clear();

            AssertEmpty(graph.Nodes);
        }

        [Fact]
        public void TestDirectedNodeOpSequence()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);
            graph.Nodes.Add(4);
            graph.Nodes.Add(5);

            AssertSetEquals(new int[] { 1, 2, 4, 5 }, graph.Nodes);

            graph.Nodes.Remove(2);

            AssertSetEquals(new int[] { 1, 4, 5 }, graph.Nodes);

            graph.Nodes.Clear();

            AssertEmpty(graph.Nodes);

            graph.Nodes.Add(1);

            AssertSetEquals(new int[] { 1 }, graph.Nodes);
        }

        [Fact]
        public void TestDirectedNodeOpSequenceCount()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            Assert.Single(graph.Nodes);

            graph.Nodes.Add(2);
            Assert.Equal(2, graph.Nodes.Count);

            graph.Nodes.Add(4);
            Assert.Equal(3, graph.Nodes.Count);

            graph.Nodes.Add(5);
            Assert.Equal(4, graph.Nodes.Count);

            graph.Nodes.Remove(2);
            Assert.Equal(3, graph.Nodes.Count);

            graph.Nodes.Clear();
            AssertEmpty(graph.Nodes);
            graph.Nodes.Add(1);
            Assert.Single(graph.Nodes);
        }

        //Edges
        [Fact]
        public void TestDirectedEdgeAddOnUnavailableNodeFailure()
        {
            var graph = new Graph<int>(true);

            Assert.Throws<ArgumentException>(() => graph.Edges.Add(1, 2));
            Assert.DoesNotContain((1, 2), graph.Edges);

            graph.Nodes.Add(1);

            Assert.Throws<ArgumentException>(() => graph.Edges.Add(1, 2));
            Assert.DoesNotContain(2, graph.Nodes);
            Assert.DoesNotContain((1, 2), graph.Edges);

        }

        [Fact]
        public void TestDirectedEdgeAdd()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            graph.Edges.Add(1, 2);
            Assert.Contains((1, 2), graph.Edges);
        }

        [Fact]
        public void TestDirectedEdgeContains()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            graph.Edges.Add(1, 2);

            Assert.Contains((1, 2), graph.Edges);
#pragma warning disable xUnit2017 // Do not use Contains() to check if a value exists in a collection
            Assert.False(graph.Edges.Contains((2, 1)));
#pragma warning restore xUnit2017 // Do not use Contains() to check if a value exists in a collection
        }

        [Fact]
        public void TestDirectedEdgeAddRedundant()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            graph.Edges.Add(1, 2);

            Assert.False(graph.Edges.Add(1, 2));
        }

        [Fact]
        public void TestDirectedEdgeRemove()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            graph.Edges.Add(1, 2);

            graph.Edges.Remove(1, 2);
            Assert.DoesNotContain((1, 2), graph.Edges);
        }

        [Fact]
        public void TestDirectedEdgeRemoveUnavailable()
        {
            var graph = new Graph<int>(true);

            Assert.False(graph.Edges.Remove(1, 2));

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            Assert.False(graph.Edges.Remove(1, 2));

            graph.Edges.Add(1, 2);

            graph.Edges.Remove(1, 2);

            Assert.False(graph.Edges.Remove(1, 2));
        }

        [Fact]
        public void TestDirectedEdgesClear()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            graph.Edges.Add(1, 2);
            graph.Edges.Add(2, 1);

            graph.Edges.Clear();

            AssertEmpty(graph.Edges);

            //nodes unaffected by clearing the edges
            AssertSetEquals(new int[] { 1, 2 }, graph.Nodes);
        }

        [Fact]
        public void TestDirectedEdgeOpSequence()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);
            graph.Nodes.Add(3);
            graph.Nodes.Add(4);

            for (int i = 1; i <= 4; i++)
                for (int j = 1; j <= 4; j++)
                    graph.Edges.Add(i, j);

            for (int i = 1; i <= 4; i++)
                for (int j = 1; j <= 4; j++)
                    Assert.Contains((i, j), graph.Edges);


            Assert.True(graph.Edges.Remove(2, 3));

            Assert.DoesNotContain((2, 3), graph.Edges);

            Assert.False(graph.Edges.Remove(2, 3));

            for (int i = 1; i <= 4; i++)
                for (int j = 1; j <= 4; j++)
                    if ((i, j) != (2, 3))
                        Assert.Contains((i, j), graph.Edges);

            Assert.True(graph.Edges.Remove(3, 2));

            graph.Edges.Clear();

            AssertEmpty(graph.Edges);

            Assert.True(graph.Edges.Add(1, 2));
            Assert.True(graph.Edges.Add(2, 1));
        }

        [Fact]
        public void TestDirectedEdgeOpSequenceCount()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);
            graph.Nodes.Add(3);
            graph.Nodes.Add(4);

            AssertEmpty(graph.Edges);

            for (int i = 1; i <= 4; i++)
                for (int j = 1; j <= 4; j++)
                    graph.Edges.Add(i, j);

            Assert.Equal(16, graph.Edges.Count);

            graph.Edges.Remove(2, 3);

            Assert.Equal(15, graph.Edges.Count);

            graph.Edges.Remove(2, 3); //false
            graph.Edges.Remove(3, 2); //true

            Assert.Equal(14, graph.Edges.Count);

            graph.Edges.Clear();
            AssertEmpty(graph.Edges);

            graph.Edges.Add(1, 2);
            Assert.Single(graph.Edges);

            graph.Edges.Add(2, 1);
            Assert.Equal(2, graph.Edges.Count);
        }

        //Edges by changes on Node-Outs
        [Fact]
        public void TestDirectedOutsInsAddForOuts()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            //test adding an item for the first time
            Assert.True(out1.Add(2));

            Assert.Contains(2, out1); //GetOuts(1).Contains(2)
            Assert.Contains(1, in2); //GetIns(2).Contains(1)

            //check for any other possible edge that may mistakingly have been added
            Assert.DoesNotContain(2, out2);
            Assert.DoesNotContain(1, out1);
            Assert.DoesNotContain(1, out2);
            Assert.DoesNotContain(2, in1);

            //check the same for graph.Edges
            Assert.Contains((1, 2), graph.Edges);
            Assert.DoesNotContain((1, 1), graph.Edges);
            Assert.DoesNotContain((2, 2), graph.Edges);
            Assert.DoesNotContain((2, 1), graph.Edges);
        }

        [Fact]
        public void TestDirectedOutsInsAddForIns()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            //test adding an item for the first time
            Assert.True(in2.Add(1));

            Assert.Contains(2, out1); //GetOuts(1).Contains(2)
            Assert.Contains(1, in2); //GetIns(2).Contains(1)

            //check for any other possible edge that may mistakingly have been added
            Assert.DoesNotContain(2, out2);
            Assert.DoesNotContain(1, out1);
            Assert.DoesNotContain(1, out2);
            Assert.DoesNotContain(2, in1);

            //check the same for graph.Edges
            Assert.Contains((1, 2), graph.Edges);
            Assert.DoesNotContain((1, 1), graph.Edges);
            Assert.DoesNotContain((2, 2), graph.Edges);
            Assert.DoesNotContain((2, 1), graph.Edges);
        }

        [Fact]
        public void TestDirectedOutsInsAddRedundantForOuts()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            out1.Add(2);

            //check whether GetOuts(1).Nodes.Add(2) a second time returns false
            Assert.False(out1.Add(2));

            //check whether GetIns(2).Nodes.Add(1) also does not work
            Assert.False(in2.Add(1));
        }

        [Fact]
        public void TestDirectedOutsInsAddRedundantForIns()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            in2.Add(1);

            //check whether GetIns(2).Nodes.Add(1) a second time does not work
            Assert.False(in2.Add(1));

            //check whether GetOuts(1).Nodes.Add(2) also does not work
            Assert.False(out1.Add(2));
        }

        [Fact]
        public void TestDirectedOutsInsRemoveForOuts()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            //check various kinds of sequences that may appear
            out1.Add(2);
            Assert.True(out1.Remove(2));

            Assert.True(out1.Add(2));
            Assert.True(out1.Remove(2));

            Assert.True(out1.Add(2));
            Assert.True(graph.Edges.Remove((1, 2)));
        }

        [Fact]
        public void TestDirectedOutsInsRemoveForIns()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            //check various kinds of sequences that may appear
            in2.Add(1);
            Assert.True(in2.Remove(1));

            Assert.True(in2.Add(1));
            Assert.True(in2.Remove(1));

            Assert.True(in2.Add(1));
            Assert.True(graph.Edges.Remove((1, 2)));
        }

        [Fact]
        public void TestDirectedOutsInsRemoveUnavailableForOuts()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            out1.Add(2);
            Assert.True(out1.Remove(2));

            Assert.False(out1.Remove(2));
        }

        [Fact]
        public void TestDirectedOutsInsRemoveUnavailableForIns()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            in2.Add(1);
            Assert.True(in2.Remove(1));

            Assert.False(in2.Remove(1));
        }

        [Fact]
        public void TestDirectedOutsInsClearForOuts()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            out1.Add(1);
            out1.Add(2);

            out1.Clear();

            AssertEmpty(out1);
            AssertEmpty(graph.Edges);
            AssertEmpty(in1);
            AssertEmpty(in2);
        }

        [Fact]
        public void TestDirectedOutsInsClearForIns()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            out1.Add(1);
            out1.Add(2);

            in1.Clear();

            //we only cleared in[1] --> we still have 1 -> 2
            Assert.True(out1.Any());
            AssertEmpty(in1);
        }

        [Fact]
        public void TestDirectedOutsInsClearOutsRetainsOtherEdges()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            out1.Add(2);
            out2.Add(1);

            out1.Clear();

            Assert.True(graph.Edges.Any());
        }

        [Fact]
        public void TestDirectedOutsInsClearInsRetainsOtherEdges()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            out1.Add(2);
            out2.Add(1);

            in1.Clear();

            Assert.True(graph.Edges.Any());
        }

        [Fact]
        public void TestDirectedOutsInsRemoveGraphNode()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);
            graph.Nodes.Add(3);
            graph.Nodes.Add(4);

            graph.Edges.Add(1, 1);
            graph.Edges.Add(1, 2);
            graph.Edges.Add(1, 3);
            graph.Edges.Add(1, 4);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            graph.Nodes.Remove(1);

            Assert.True(out1.IsDetached);
            Assert.True(in1.IsDetached);

            Assert.False(out2.IsDetached);
            Assert.False(in2.IsDetached);
        }

        [Fact]
        public void TestDirectedOutsInsRemoveGraphNodeRemovedFromConnectedNodes()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            graph.Edges.Add(1, 1);
            graph.Edges.Add(1, 2);
            graph.Edges.Add(2, 1);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            graph.Nodes.Remove(1);

            Assert.DoesNotContain(1, out2);
            Assert.DoesNotContain(1, in2);
        }

        [Fact]
        public void TestDirectedOutsInsRemoveGraphNodeDetached()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            graph.Edges.Add(1, 1);
            graph.Edges.Add(1, 2);
            graph.Edges.Add(2, 1);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            graph.Nodes.Remove(1);

            Assert.True(in1.IsDetached);
            Assert.True(out1.IsDetached);
            Assert.False(in2.IsDetached);
            Assert.False(out2.IsDetached);

            Assert.Throws<InvalidOperationException>(() => in1.Count);
            Assert.Throws<InvalidOperationException>(() => out1.Count);
            Assert.Throws<InvalidOperationException>(() => in1.Contains(1));
            Assert.Throws<InvalidOperationException>(() => out1.Contains(1));
            Assert.Throws<InvalidOperationException>(() => in1.CopyTo(new int[100], 0));
            Assert.Throws<InvalidOperationException>(() => out1.CopyTo(new int[100], 0));
            Assert.Throws<InvalidOperationException>(() => in1.Add(2));
            Assert.Throws<InvalidOperationException>(() => out1.Add(2));
            Assert.Throws<InvalidOperationException>(() => in1.Remove(2));
            Assert.Throws<InvalidOperationException>(() => out1.Remove(2));
            Assert.Throws<InvalidOperationException>(() => in1.Clear());
            Assert.Throws<InvalidOperationException>(() => out1.Clear());
            Assert.Throws<InvalidOperationException>(() => in1.Add(2));
            Assert.Throws<InvalidOperationException>(() => out1.Add(2));

            Assert.Equal("", in1.ToString());
            Assert.Equal("", out1.ToString());
        }

        [Fact]
        public void TestDirectedOutsInsRemoveGraphNodeWorkOnNodeFails()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            graph.Edges.Add(1, 1);
            graph.Edges.Add(1, 2);
            graph.Edges.Add(2, 1);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            graph.Nodes.Remove(1);

            Assert.Throws<ArgumentException>(() => in2.Add(1));
            Assert.Throws<ArgumentException>(() => out2.Add(1));
        }

        [Fact]
        public void TestDirectedOutsInsOpSequence()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);
            graph.Nodes.Add(3);
            graph.Nodes.Add(4);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            //add 1 -> 2, redundant add on in[2]
            Assert.True(out1.Add(2));
            Assert.Contains(1, in2);
            Assert.Contains(2, out1);
            Assert.False(in2.Add(1));

            //add 2 -> 1, redundant add on in[1]
            Assert.True(out2.Add(1));
            Assert.False(in1.Add(2));
            Assert.Contains(2, in1);
            Assert.Contains(1, out2);

            //remove 2 -> 1, redundant removal on in[1] and out[2]
            Assert.True(in1.Remove(2));
            Assert.DoesNotContain(2, in1);
            Assert.DoesNotContain(1, out2);
            Assert.False(in1.Remove(2));
            Assert.False(out2.Remove(1));

            Assert.True(in1.Add(3));
            Assert.True(in1.Add(4));

            Assert.True(out2.Add(3));

            //our graph now contains the nodes {1, 2, 3, 4} and edges {1 -> 2, 3 -> 1, 4 -> 1, 2 -> 3}
            Assert.Contains((1, 2), graph.Edges);
            Assert.Contains((3, 1), graph.Edges);
            Assert.Contains((4, 1), graph.Edges);

            //we remove the node 1; G = ({2, 3, 4}, {2 -> 3})
            graph.Nodes.Remove(1);

            Assert.DoesNotContain((1, 2), graph.Edges);
            Assert.DoesNotContain((3, 1), graph.Edges);
            Assert.DoesNotContain((4, 1), graph.Edges);
            Assert.Contains((2, 3), graph.Edges);

            Assert.True(in1.IsDetached);
            Assert.True(out1.IsDetached);
            Assert.False(in2.IsDetached);
            Assert.False(out2.IsDetached);

            //add a new instance 1 that is not bound to the previous collection
            graph.Nodes.Add(1);
            graph.Edges.Add((1, 2));

            Assert.True(in1.IsDetached);
            Assert.True(out1.IsDetached);
            Assert.False(ReferenceEquals(in1, graph.GetIns(1)));
            Assert.False(ReferenceEquals(out1, graph.GetOuts(1)));

            in1 = graph.GetIns(1);
            out1 = graph.GetOuts(1);

            Assert.Contains((1, 2), graph.Edges);
            Assert.Contains(2, out1);
            Assert.Contains(1, in2);
        }

        [Fact]
        public void TestDirectedOutsInsOpSequenceCount()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);
            graph.Nodes.Add(3);
            graph.Nodes.Add(4);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            //add 1 -> 2, redundant add on in[2]
            out1.Add(2);
            Assert.Single(out1);
            Assert.Single(in2);
            AssertEmpty(out2);
            AssertEmpty(in1);
            in2.Add(1); //redundant
            Assert.Single(out1);
            Assert.Single(in2);
            AssertEmpty(out2);
            AssertEmpty(in1);
            Assert.Single(graph.Edges);

            //add 2 -> 1, redundant add on in[1]
            out2.Add(1);
            Assert.Single(out2);
            Assert.Single(in1);
            Assert.Single(out1);
            Assert.Single(in2);
            Assert.Equal(2, graph.Edges.Count);
            in1.Add(2); //redundant
            Assert.Single(out2);
            Assert.Single(in1);
            Assert.Single(out1);
            Assert.Single(in2);
            Assert.Equal(2, graph.Edges.Count);

            //remove 2 -> 1, redundant removal on in[1] and out[2]
            in1.Remove(2);
            Assert.Single(out1);
            Assert.Single(in2);
            AssertEmpty(out2);
            AssertEmpty(in1);
            Assert.Single(graph.Edges);
            in1.Remove(2);
            Assert.Single(out1);
            Assert.Single(in2);
            AssertEmpty(out2);
            AssertEmpty(in1);
            Assert.Single(graph.Edges);
            out2.Remove(1);
            Assert.Single(out1);
            Assert.Single(in2);
            AssertEmpty(out2);
            AssertEmpty(in1);
            Assert.Single(graph.Edges);

            in1.Add(3); // add 3 -> 1
            in1.Add(4); // add 4 -> 1
            out2.Add(3); // add 2 -> 3

            Assert.Equal(2, in1.Count);
            Assert.Equal(4, graph.Edges.Count);

            //we remove the node 1; G = ({2, 3, 4}, {2 -> 3})
            graph.Nodes.Remove(1);
            Assert.Single(graph.Edges);

            //add a new instance 1 that is not bound to the previous collection
            graph.Nodes.Add(1);
            graph.Edges.Add((1, 2));

            Assert.Equal(2, graph.Edges.Count);
            Assert.Single(graph.GetOuts(1));
            AssertEmpty(graph.GetIns(1));
        }

        [Fact]
        public void TestDirectedOutsInsClearCount()
        {
            var graph = new Graph<int>(true);

            var edges = new HashSet<Edge<int>>();
            var nodes = new List<int>();

            IEnumerable<int> GetIns(int node) => edges.Where((n) => n.Node2 == node).Select((n) => n.Node1);
            IEnumerable<int> GetOuts(int node) => edges.Where((n) => n.Node1 == node).Select((n) => n.Node2);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);
            graph.Nodes.Add(3);
            graph.Nodes.Add(4);

            nodes.AddRange(new int[] { 1, 2, 3, 4 });

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            void AssertGraphEqual()
            {
                AssertSetEquals(nodes, graph.Nodes);
                Assert.Equal(nodes.Count, graph.Nodes.Count);
                AssertSetEquals(edges, graph.Edges);
                Assert.Equal(edges.Count, graph.Edges.Count);

                foreach (var node in nodes)
                {
                    var ins = GetIns(node).ToArray();
                    var outs = GetOuts(node).ToArray();
                    var gins = graph.GetIns(node);
                    var gouts = graph.GetOuts(node);

                    Assert.Equal(ins.Length, gins.Count);
                    Assert.Equal(outs.Length, gouts.Count);

                    AssertSetEquals(ins, gins);
                    AssertSetEquals(outs, gouts);
                }
            }

            void RemoveNode(int node)
            {
                ClearNode(node, true, true);
                nodes.Remove(node);
            }

            void ClearNode(int node, bool from, bool to)
            {
                foreach (var edge in edges.ToArray())
                {
                    if ((from && edge.Node1 == node) || (to && edge.Node2 == node))
                        edges.Remove(edge);
                }
            }

            //add 1 -> 2, redundant add on in[2]
            edges.Add((1, 2));
            out1.Add(2);
            AssertGraphEqual();
            edges.Add((1, 2));
            AssertGraphEqual();
            in2.Add(1); //redundant
            AssertGraphEqual();

            //add 2 -> 1, redundant add on in[1]
            edges.Add((2, 1));
            out2.Add(1);
            AssertGraphEqual();
            edges.Add((2, 1));
            AssertGraphEqual();
            in1.Add(2); //redundant
            AssertGraphEqual();

            //remove 2 -> 1, redundant removal on in[1] and out[2]
            edges.Remove((2, 1));
            in1.Remove(2);
            AssertGraphEqual();
            in1.Remove(2);
            AssertGraphEqual();
            out2.Remove(1);
            AssertGraphEqual();

            edges.Add((3, 1));
            in1.Add(3); // add 3 -> 1
            AssertGraphEqual();

            edges.Add((4, 1));
            in1.Add(4); // add 4 -> 1
            AssertGraphEqual();

            edges.Add((2, 3));
            out2.Add(3); // add 2 -> 3
            AssertGraphEqual();

            //we remove the node 1; G = ({2, 3, 4}, {2 -> 3})
            RemoveNode(1);
            graph.Nodes.Remove(1);
            AssertGraphEqual();

            //add a new instance 1 that is not bound to the previous collection
            nodes.Add(1);
            graph.Nodes.Add(1);
            AssertGraphEqual();

            edges.Add((1, 2));
            graph.Edges.Add((1, 2));
            AssertGraphEqual();

            //we add a few edges and start clearing
            out1 = graph.GetOuts(1);
            in1 = graph.GetIns(1);

            edges.Add((2, 4));
            out2.Add(4);
            AssertGraphEqual();

            edges.Add((1, 2));
            in2.Add(1);
            AssertGraphEqual();

            edges.Add((2, 2));
            out2.Add(2);
            AssertGraphEqual();

            out2.Clear();
            ClearNode(2, true, false);
            AssertGraphEqual();

            edges.Add((1, 2));
            out1.Add(2);
            AssertGraphEqual();

            edges.Add((2, 2));
            in2.Add(2);
            AssertGraphEqual();

            edges.Add((3, 2));
            graph.GetOuts(3).Add(2);
            AssertGraphEqual();

            in2.Clear();
            ClearNode(2, false, true);
            AssertGraphEqual();

            edges.Add((1, 2));
            out1.Add(2);
            AssertGraphEqual();

            edges.Add((1, 2));
            in2.Add(1);
            AssertGraphEqual();

            edges.Add((3, 2));
            graph.GetOuts(3).Add(2);
            AssertGraphEqual();

            edges.Clear();
            graph.Edges.Clear();
            AssertGraphEqual();

            edges.Add((1, 2));
            out1.Add(2);
            AssertGraphEqual();

            edges.Add((1, 2));
            in2.Add(1);
            AssertGraphEqual();

            edges.Add((3, 2));
            graph.GetOuts(3).Add(2);
            AssertGraphEqual();
        }

        [Fact]
        public void TestDirectedOutsInsOpSequenceEnumerator()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);
            graph.Nodes.Add(3);
            graph.Nodes.Add(4);

            graph.Edges.AddSet((1, 2), (1, 3));
            graph.Edges.AddSet((2, 3), (2, 4));
            graph.Edges.AddSet((4, 1), (4, 2));

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            in1.Clear();

            Assert.DoesNotContain((4, 1), graph.Edges);
            Assert.Equal(5, graph.Edges.Count); //we only removed (4, 1)
            AssertEmpty(in1);
            Assert.Single(graph.GetOuts(4)); // still contains (4, 2)
            AssertNotEmpty(out1);

            out1.Clear(); //we remove 1 -> 2 and 1 -> 3

            AssertEmpty(out1);
            Assert.DoesNotContain((1, 2), graph.Edges);
            Assert.DoesNotContain((1, 3), graph.Edges);

            Assert.Equal(3, graph.Edges.Count);
        }

        [Fact]
        public void TestDirectedNodesClearAndSwipeEdges()
        {
            var graph = new Graph<int>(true);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);
            graph.Nodes.Add(3);
            graph.Nodes.Add(4);

            graph.Edges.AddSet((1, 2), (1, 3));
            graph.Edges.AddSet((2, 3), (2, 4));
            graph.Edges.AddSet((4, 1), (4, 2));

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            graph.Nodes.Clear();

            AssertEmpty(graph.Edges);

            Assert.True(in1.IsDetached);
            Assert.True(in2.IsDetached);
            Assert.True(out1.IsDetached);
            Assert.True(out2.IsDetached);
        }

        //Undirected graph
        //Nodes
        [Fact]
        public void TestUndirectedEmptyGraph()
        {
            var graph = new Graph<int>(false);

            AssertEmpty(graph.Edges);
            AssertEmpty(graph.Nodes);
        }

        [Fact]
        public void TestUndirectedNodeAdd()
        {
            var graph = new Graph<int>(false);

            Assert.True(graph.Nodes.Add(1));
            Assert.Contains(1, graph.Nodes);

            Assert.True(graph.Nodes.Add(2));
            Assert.Contains(1, graph.Nodes);
            Assert.Contains(2, graph.Nodes);
        }

        [Fact]
        public void TestUndirectedNodeAddRedundant()
        {
            var graph = new Graph<int>(false);

            Assert.True(graph.Nodes.Add(1));
            Assert.Contains(1, graph.Nodes);

            Assert.False(graph.Nodes.Add(1));
            Assert.Contains(1, graph.Nodes);
        }

        [Fact]
        public void TestUndirectedNodeRemove()
        {
            var graph = new Graph<int>(false);

            Assert.True(graph.Nodes.Add(1));
            Assert.Contains(1, graph.Nodes);

            Assert.True(graph.Nodes.Remove(1));
            Assert.DoesNotContain(1, graph.Nodes);
        }

        [Fact]
        public void TestUndirectedNodeRemoveUnavailable()
        {
            var graph = new Graph<int>(false);

            Assert.False(graph.Nodes.Remove(1));
            Assert.DoesNotContain(1, graph.Nodes);

            Assert.True(graph.Nodes.Add(1));
            Assert.Contains(1, graph.Nodes);

            Assert.False(graph.Nodes.Remove(2));
            Assert.DoesNotContain(2, graph.Nodes);
            Assert.Contains(1, graph.Nodes);
        }

        [Fact]
        public void TestUndirectedNodesClear()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Clear();

            AssertEmpty(graph.Nodes);
        }

        [Fact]
        public void TestUndirectedNodeOpSequence()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);
            graph.Nodes.Add(4);
            graph.Nodes.Add(5);

            AssertSetEquals(new int[] { 1, 2, 4, 5 }, graph.Nodes);

            graph.Nodes.Remove(2);

            AssertSetEquals(new int[] { 1, 4, 5 }, graph.Nodes);

            graph.Nodes.Clear();

            AssertEmpty(graph.Nodes);

            graph.Nodes.Add(1);

            AssertSetEquals(new int[] { 1 }, graph.Nodes);
        }

        [Fact]
        public void TestUndirectedNodeOpSequenceCount()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            Assert.Single(graph.Nodes);

            graph.Nodes.Add(2);
            Assert.Equal(2, graph.Nodes.Count);

            graph.Nodes.Add(4);
            Assert.Equal(3, graph.Nodes.Count);

            graph.Nodes.Add(5);
            Assert.Equal(4, graph.Nodes.Count);

            graph.Nodes.Remove(2);
            Assert.Equal(3, graph.Nodes.Count);

            graph.Nodes.Clear();
            AssertEmpty(graph.Nodes);
            graph.Nodes.Add(1);
            Assert.Single(graph.Nodes);
        }

        //Edges
        [Fact]
        public void TestUndirectedEdgeAddOnUnavailableNodeFailure()
        {
            var graph = new Graph<int>(false);

            Assert.Throws<ArgumentException>(() => graph.Edges.Add(1, 2));
            Assert.DoesNotContain((1, 2), graph.Edges);

            graph.Nodes.Add(1);

            Assert.Throws<ArgumentException>(() => graph.Edges.Add(1, 2));
            Assert.DoesNotContain(2, graph.Nodes);
            Assert.DoesNotContain((1, 2), graph.Edges);
        }

        [Fact]
        public void TestUndirectedEdgeAdd()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            graph.Edges.Add(1, 2);
            Assert.Contains((1, 2), graph.Edges);
        }

        [Fact]
        public void TestUndirectedEdgeAddRedundant()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            graph.Edges.Add(1, 2);

            Assert.False(graph.Edges.Add(1, 2));
            Assert.False(graph.Edges.Add(2, 1));
        }

        [Fact]
        public void TestUndirectedEdgeContains()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            graph.Edges.Add(1, 2);

            Assert.Contains((1, 2), graph.Edges);
            Assert.Contains((2, 1), graph.Edges);
        }

        [Fact]
        public void TestUndirectedEdgeRemove()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            graph.Edges.Add(1, 2);

            graph.Edges.Remove(1, 2);
            Assert.DoesNotContain((1, 2), graph.Edges);
        }

        [Fact]
        public void TestUndirectedEdgeRemoveReversed()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            graph.Edges.Add(1, 2);

            graph.Edges.Remove(2, 1);
            Assert.DoesNotContain((1, 2), graph.Edges);
            Assert.DoesNotContain((2, 1), graph.Edges);
        }

        [Fact]
        public void TestUndirectedEdgeRemoveUnavailable()
        {
            var graph = new Graph<int>(false);

            Assert.False(graph.Edges.Remove(1, 2));

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            Assert.False(graph.Edges.Remove(1, 2));
            Assert.False(graph.Edges.Remove(2, 1));

            graph.Edges.Add(1, 2);

            graph.Edges.Remove(1, 2);

            Assert.False(graph.Edges.Remove(1, 2));
            Assert.False(graph.Edges.Remove(2, 1));
        }

        [Fact]
        public void TestUndirectedEdgesClear()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            graph.Edges.Add(1, 2);
            graph.Edges.Add(2, 1);

            graph.Edges.Clear();

            AssertEmpty(graph.Edges);

            //nodes unaffected by clearing the edges
            AssertSetEquals(new int[] { 1, 2 }, graph.Nodes);
        }

        [Fact]
        public void TestUndirectedEdgeOpSequence()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);
            graph.Nodes.Add(3);
            graph.Nodes.Add(4);

            for (int i = 1; i <= 4; i++)
                for (int j = 1; j <= 4; j++)
                    Assert.Equal(j >= i, graph.Edges.Add(i, j));

            for (int i = 1; i <= 4; i++)
                for (int j = 1; j <= 4; j++)
                    Assert.Contains((i, j), graph.Edges);


            graph.Edges.Remove(2, 3);

            Assert.DoesNotContain((2, 3), graph.Edges);
            Assert.DoesNotContain((3, 2), graph.Edges);

            Assert.False(graph.Edges.Remove(2, 3));
            Assert.False(graph.Edges.Remove(3, 2));

            for (int i = 1; i <= 4; i++)
                for (int j = 1; j <= 4; j++)
                    if ((i, j) != (2, 3) && (i, j) != (3, 2))
                        Assert.Contains((i, j), graph.Edges);

            Assert.False(graph.Edges.Remove(3, 2));

            graph.Edges.Clear();

            AssertEmpty(graph.Edges);

            Assert.True(graph.Edges.Add(1, 2));
            Assert.False(graph.Edges.Add(2, 1));
        }

        [Fact]
        public void TestUndirectedEdgeOpSequenceCount()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);
            graph.Nodes.Add(3);
            graph.Nodes.Add(4);

            AssertEmpty(graph.Edges);

            for (int i = 1; i <= 4; i++)
                for (int j = i; j <= 4; j++)
                    graph.Edges.Add(i, j);

            Assert.Equal(10, graph.Edges.Count);

            graph.Edges.Remove(2, 3);

            Assert.Equal(9, graph.Edges.Count);

            graph.Edges.Remove(2, 3); //false
            graph.Edges.Remove(3, 2); //false

            Assert.Equal(9, graph.Edges.Count);

            graph.Edges.Clear();
            AssertEmpty(graph.Edges);

            graph.Edges.Add(1, 2);
            Assert.Single(graph.Edges);

            graph.Edges.Add(2, 1); //false
            Assert.Single(graph.Edges);
        }

        //Edges by changes on Node-Outs
        [Fact]
        public void TestUndirectedOutsInsAddForOuts()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            //test adding an item for the first time
            Assert.True(out1.Add(2));

            Assert.Contains(2, out1); //GetOuts(1).Contains(2)
            Assert.Contains(1, in2); //GetIns(2).Contains(1)
            Assert.Contains(1, out2); //GetOuts(2).Contains(1)
            Assert.Contains(2, in1); //GetIns(1).Contains(2)

            //check for any other possible edge that may mistakingly have been added
            Assert.DoesNotContain(2, out2);
            Assert.DoesNotContain(1, out1);

            //check the same for graph.Edges
            Assert.Contains((1, 2), graph.Edges);
            Assert.Contains((2, 1), graph.Edges);
            Assert.DoesNotContain((1, 1), graph.Edges);
            Assert.DoesNotContain((2, 2), graph.Edges);
        }

        [Fact]
        public void TestUndirectedOutsInsAddForIns()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            //test adding an item for the first time
            Assert.True(in2.Add(1));

            Assert.Contains(2, out1); //GetOuts(1).Contains(2)
            Assert.Contains(1, in2); //GetIns(2).Contains(1)
            Assert.Contains(1, out2); //GetOuts(2).Contains(1)
            Assert.Contains(2, in1); //GetIns(1).Contains(2)

            //check for any other possible edge that may mistakingly have been added
            Assert.DoesNotContain(2, out2);
            Assert.DoesNotContain(1, out1);

            //check the same for graph.Edges
            Assert.Contains((1, 2), graph.Edges);
            Assert.Contains((2, 1), graph.Edges);
            Assert.DoesNotContain((1, 1), graph.Edges);
            Assert.DoesNotContain((2, 2), graph.Edges);
        }

        [Fact]
        public void TestUndirectedOutsInsAddRedundantForOuts()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            out1.Add(2);

            //check whether GetOuts(1).Nodes.Add(2) a second time returns false
            Assert.False(out1.Add(2));

            //check whether GetIns(2).Nodes.Add(1) also does not work
            Assert.False(in2.Add(1));
        }

        [Fact]
        public void TestUndirectedOutsInsAddRedundantForIns()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            in2.Add(1);

            //check whether GetIns(2).Nodes.Add(1) a second time does not work
            Assert.False(in2.Add(1));

            //check whether GetOuts(1).Nodes.Add(2) also does not work
            Assert.False(out1.Add(2));
        }

        [Fact]
        public void TestUndirectedOutsInsRemoveForOuts()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            //check various kinds of sequences that may appear
            out1.Add(2);
            Assert.True(out1.Remove(2));

            Assert.True(out1.Add(2));
            Assert.True(out1.Remove(2));

            Assert.True(out1.Add(2));
            Assert.True(graph.Edges.Remove((1, 2)));

            Assert.True(in2.Add(1));
            Assert.True(out1.Remove(2));
        }

        [Fact]
        public void TestUndirectedOutsInsRemoveForIns()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            //check various kinds of sequences that may appear
            in2.Add(1);
            Assert.True(in2.Remove(1));

            Assert.True(in2.Add(1));
            Assert.True(in2.Remove(1));

            Assert.True(in2.Add(1));
            Assert.True(graph.Edges.Remove((1, 2)));

            Assert.True(out2.Add(1));
            Assert.True(in1.Remove(2));
        }

        [Fact]
        public void TestUndirectedOutsInsRemoveUnavailableForOuts()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            out1.Add(2);
            Assert.True(out1.Remove(2));

            Assert.False(out1.Remove(2));
            Assert.False(in2.Remove(1));
        }

        [Fact]
        public void TestUndirectedOutsInsRemoveUnavailableForIns()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            in2.Add(1);
            Assert.True(in2.Remove(1));

            Assert.False(in2.Remove(1));
            Assert.False(out1.Remove(2));
        }

        [Fact]
        public void TestUndirectedOutsInsClearForOuts()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            out1.Add(1);
            out1.Add(2);

            out1.Clear();

            AssertEmpty(out1);
            AssertEmpty(graph.Edges);
            AssertEmpty(in1);
            AssertEmpty(in2);
        }

        [Fact]
        public void TestUndirectedOutsInsClearForIns()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            out1.Add(1);
            out1.Add(2);

            in1.Clear();

            //we removed 1 - 2 and 1 - 1
            Assert.False(out1.Any());
            AssertEmpty(in1);
        }

        [Fact]
        public void TestUndirectedOutsInsClearOutsRetainsOtherEdges()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            out1.Add(2);
            out2.Add(1);
            out2.Add(2);

            out1.Clear();

            Assert.True(graph.Edges.Any());
        }

        [Fact]
        public void TestUndirectedOutsInsClearInsRetainsOtherEdges()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);
            graph.Nodes.Add(3);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            out1.Add(2);
            out2.Add(1);
            out2.Add(2);

            in1.Clear();

            Assert.True(graph.Edges.Any());
        }

        [Fact]
        public void TestUndirectedOutsInsRemoveGraphNode()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);
            graph.Nodes.Add(3);
            graph.Nodes.Add(4);

            graph.Edges.Add(1, 1);
            graph.Edges.Add(1, 2);
            graph.Edges.Add(1, 3);
            graph.Edges.Add(1, 4);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            graph.Nodes.Remove(1);

            Assert.True(out1.IsDetached);
            Assert.True(in1.IsDetached);

            Assert.False(out2.IsDetached);
            Assert.False(in2.IsDetached);
        }

        [Fact]
        public void TestUndirectedOutsInsRemoveGraphNodeRemovedFromConnectedNodes()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            graph.Edges.Add(1, 1);
            graph.Edges.Add(1, 2);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            graph.Nodes.Remove(1);

            Assert.DoesNotContain(1, out2);
            Assert.DoesNotContain(1, in2);
        }

        [Fact]
        public void TestUndirectedOutsInsRemoveGraphNodeDetached()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            graph.Edges.Add(1, 1);
            graph.Edges.Add(1, 2);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            graph.Nodes.Remove(1);

            Assert.True(in1.IsDetached);
            Assert.True(out1.IsDetached);
            Assert.False(in2.IsDetached);
            Assert.False(out2.IsDetached);

            Assert.Throws<InvalidOperationException>(() => in1.Count);
            Assert.Throws<InvalidOperationException>(() => out1.Count);
            Assert.Throws<InvalidOperationException>(() => in1.Contains(1));
            Assert.Throws<InvalidOperationException>(() => out1.Contains(1));
            Assert.Throws<InvalidOperationException>(() => in1.CopyTo(new int[100], 0));
            Assert.Throws<InvalidOperationException>(() => out1.CopyTo(new int[100], 0));
            Assert.Throws<InvalidOperationException>(() => in1.Add(2));
            Assert.Throws<InvalidOperationException>(() => out1.Add(2));
            Assert.Throws<InvalidOperationException>(() => in1.Remove(2));
            Assert.Throws<InvalidOperationException>(() => out1.Remove(2));
            Assert.Throws<InvalidOperationException>(() => in1.Clear());
            Assert.Throws<InvalidOperationException>(() => out1.Clear());
            Assert.Throws<InvalidOperationException>(() => in1.Add(2));
            Assert.Throws<InvalidOperationException>(() => out1.Add(2));

            Assert.Equal("", in1.ToString());
            Assert.Equal("", out1.ToString());
        }

        [Fact]
        public void TestUndirectedOutsInsRemoveGraphNodeWorkOnNodeFails()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);

            graph.Edges.Add(1, 1);
            graph.Edges.Add(1, 2);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            graph.Nodes.Remove(1);

            Assert.Throws<ArgumentException>(() => in2.Add(1));
            Assert.Throws<ArgumentException>(() => out2.Add(1));
        }

        [Fact]
        public void TestUndirectedOutsInsOpSequence()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);
            graph.Nodes.Add(3);
            graph.Nodes.Add(4);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            //add 1 - 2, redundant add on in[2]
            Assert.True(out1.Add(2));
            Assert.Contains(1, in2);
            Assert.Contains(2, out1);
            Assert.Contains(1, out2);
            Assert.Contains(2, in1);
            Assert.False(in2.Add(1));
            Assert.False(out2.Add(1));
            Assert.False(in1.Add(2));
            Assert.Contains(1, in2);
            Assert.Contains(2, out1);
            Assert.Contains(1, out2);
            Assert.Contains(2, in1);

            //remove 2 - 1, redundant removal on in[1] and out[2]
            Assert.True(in1.Remove(2));
            Assert.DoesNotContain(2, in1);
            Assert.DoesNotContain(1, out2);
            Assert.DoesNotContain(1, in2);
            Assert.DoesNotContain(2, out1);
            Assert.False(in1.Remove(2));
            Assert.False(in2.Remove(1));
            Assert.False(out2.Remove(1));
            Assert.False(out1.Remove(2));

            Assert.True(in1.Add(3));
            Assert.True(in1.Add(4));

            Assert.True(out2.Add(3));

            //our graph now contains the nodes {1, 2, 3, 4} and edges {3 - 1, 4 - 1, 2 - 3}
            Assert.DoesNotContain((1, 2), graph.Edges);
            Assert.Contains((3, 1), graph.Edges);
            Assert.Contains((4, 1), graph.Edges);
            Assert.DoesNotContain((2, 1), graph.Edges);
            Assert.Contains((1, 3), graph.Edges);
            Assert.Contains((1, 4), graph.Edges);

            //we remove the node 1; G = ({2, 3, 4}, {2 - 3})
            graph.Nodes.Remove(1);

            Assert.DoesNotContain((1, 2), graph.Edges);
            Assert.DoesNotContain((2, 1), graph.Edges);
            Assert.DoesNotContain((3, 1), graph.Edges);
            Assert.DoesNotContain((1, 3), graph.Edges);
            Assert.DoesNotContain((4, 1), graph.Edges);
            Assert.DoesNotContain((1, 4), graph.Edges);
            Assert.Contains((2, 3), graph.Edges);
            Assert.Contains((3, 2), graph.Edges);

            Assert.True(in1.IsDetached);
            Assert.True(out1.IsDetached);
            Assert.False(in2.IsDetached);
            Assert.False(out2.IsDetached);

            //add a new instance 1 that is not bound to the previous collection
            graph.Nodes.Add(1);
            graph.Edges.Add((1, 2));

            Assert.True(in1.IsDetached);
            Assert.True(out1.IsDetached);
            Assert.False(ReferenceEquals(in1, graph.GetIns(1)));
            Assert.False(ReferenceEquals(out1, graph.GetOuts(1)));

            in1 = graph.GetIns(1);
            out1 = graph.GetOuts(1);

            Assert.Contains((1, 2), graph.Edges);
            Assert.Contains((2, 1), graph.Edges);
            Assert.Contains(2, out1);
            Assert.Contains(1, in2);
            Assert.Contains(1, out2);
            Assert.Contains(2, in1);
        }

        [Fact]
        public void TestUndirectedOutsInsOpSequenceCount()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);
            graph.Nodes.Add(3);
            graph.Nodes.Add(4);

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);
            var out3 = graph.GetOuts(3);
            var out4 = graph.GetOuts(4);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);
            var in3 = graph.GetIns(3);
            var in4 = graph.GetIns(4);

            //note that we insert 3 nodes *before* working on 1 - 2 (compared to the code above)
            in1.Add(3); // add 3 - 1
            in1.Add(4); // add 4 - 1
            out2.Add(3); // add 2 - 3

            //add 1 - 2, redundant add on in[2]
            //on node 1, we have neighbors 2, 3, 4
            out1.Add(2);
            Assert.Equal(3, out1.Count);
            Assert.Equal(3, in1.Count);
            Assert.Equal(2, out2.Count);
            Assert.Equal(2, in2.Count);
            Assert.Equal(2, out3.Count);
            Assert.Equal(2, in3.Count);
            Assert.Single(out4);
            Assert.Single(in4);
            in2.Add(1); //redundant
            Assert.Equal(3, out1.Count);
            Assert.Equal(3, in1.Count);
            Assert.Equal(2, out2.Count);
            Assert.Equal(2, in2.Count);
            Assert.Equal(4, graph.Edges.Count);
            out2.Add(1); //redundant
            Assert.Equal(3, out1.Count);
            Assert.Equal(3, in1.Count);
            Assert.Equal(2, out2.Count);
            Assert.Equal(2, in2.Count);
            Assert.Equal(4, graph.Edges.Count);
            in1.Add(2); //redundant
            Assert.Equal(3, out1.Count);
            Assert.Equal(3, in1.Count);
            Assert.Equal(2, out2.Count);
            Assert.Equal(2, in2.Count);
            Assert.Equal(4, graph.Edges.Count);

            //remove 2 - 1, redundant removal on in[1] and out[2]
            in1.Remove(2);
            Assert.Equal(2, out1.Count);
            Assert.Equal(2, in1.Count);
            Assert.Single(out2);
            Assert.Single(in2);
            Assert.Equal(3, graph.Edges.Count);
            in1.Remove(2);
            Assert.Equal(2, out1.Count);
            Assert.Equal(2, in1.Count);
            Assert.Single(out2);
            Assert.Single(in2);
            Assert.Equal(3, graph.Edges.Count);
            out2.Remove(1);
            Assert.Equal(2, out1.Count);
            Assert.Equal(2, in1.Count);
            Assert.Single(out2);
            Assert.Single(in2);
            Assert.Equal(3, graph.Edges.Count);
            in2.Remove(1);
            Assert.Equal(2, out1.Count);
            Assert.Equal(2, in1.Count);
            Assert.Single(out2);
            Assert.Single(in2);
            Assert.Equal(3, graph.Edges.Count);
            out1.Remove(2);
            Assert.Equal(2, out1.Count);
            Assert.Equal(2, in1.Count);
            Assert.Single(out2);
            Assert.Single(in2);
            Assert.Equal(3, graph.Edges.Count);

            //we remove the node 1; G = ({2, 3, 4}, {2 - 3})
            graph.Nodes.Remove(1);
            Assert.Single(graph.Edges);
            AssertEmpty(in4);
            AssertEmpty(out4);
            Assert.Single(out2);
            Assert.Single(in2);
            Assert.Single(out3);
            Assert.Single(in3);

            //add a new instance 1 that is not bound to the previous collection
            graph.Nodes.Add(1);
            graph.Edges.Add((1, 2));

            Assert.Equal(2, graph.Edges.Count);
            Assert.Single(graph.GetOuts(1));
            Assert.Single(graph.GetIns(1));
        }

        [Fact]
        public void TestUndirectedOutsInsClearCount()
        {
            var graph = new Graph<int>(false);

            var edges = new HashSet<Edge<int>>();
            var nodes = new List<int>();

            IEnumerable<int> GetIns(int node) => edges.Where((n) => n.Node2 == node || n.Node1 == node).Select((n) => n.Node1 == node ? n.Node2 : n.Node1);
            IEnumerable<int> GetOuts(int node) => GetIns(node);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);
            graph.Nodes.Add(3);
            graph.Nodes.Add(4);

            nodes.AddRange(new int[] { 1, 2, 3, 4 });

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            void AssertUndirectedSetEquals(ICollection<int> edges, int node, IEnumerable<int> other)
            {
                var l = new HashSet<Edge<int>>();
                var r = new HashSet<Edge<int>>();

                foreach (var lv in edges)
                    l.Add((lv, node));
                foreach (var rv in other)
                    r.Add((rv, node));

                foreach (var le in l)
                    Assert.True(r.Contains(le) || r.Contains(le.Reversed()));

                foreach (var re in r)
                    Assert.True(l.Contains(re) || l.Contains(re.Reversed()));
            }

            void AssertGraphEqual()
            {
                AssertSetEquals(nodes, graph.Nodes);
                Assert.Equal(nodes.Count, graph.Nodes.Count);
                Assert.Equal(edges.Count, graph.Edges.Count);

                foreach(var edge in edges)
                    Assert.True(graph.Edges.Contains(edge) || graph.Edges.Contains(edge.Reversed()));
                foreach (var edge in graph.Edges)
                    Assert.True(edges.Contains(edge) || edges.Contains(edge.Reversed()));

                foreach (var node in nodes)
                {
                    var ins = GetIns(node).ToArray();
                    var outs = GetOuts(node).ToArray();
                    var gins = graph.GetIns(node);
                    var gouts = graph.GetOuts(node);

                    Assert.Equal(ins.Length, gins.Count);
                    Assert.Equal(outs.Length, gouts.Count);

                    AssertUndirectedSetEquals(ins, node, gins);
                    AssertUndirectedSetEquals(outs, node, gouts);
                }
            }

            bool RemoveEdge(Edge<int> edge) => edges.Remove(edge) || edges.Remove(edge.Reversed());

            void RemoveNode(int node)
            {
                ClearNode(node);
                nodes.Remove(node);
            }

            void ClearNode(int node)
            {
                foreach (var edge in edges.ToArray())
                {
                    if ((edge.Node1 == node) || (edge.Node2 == node))
                        edges.Remove(edge);
                }
            }

            //add 1 - 2, redundant add on in[2]
            edges.Add((1, 2));
            out1.Add(2);
            AssertGraphEqual();
            in2.Add(1); //redundant
            AssertGraphEqual();
            out2.Add(1); //redundant
            AssertGraphEqual();
            in1.Add(2); //redundant
            AssertGraphEqual();

            //remove 2 -> 1, redundant removal on in[1] and out[2]
            RemoveEdge((2, 1));
            in1.Remove(2);
            AssertGraphEqual();
            in1.Remove(2);
            AssertGraphEqual();
            out2.Remove(1);
            AssertGraphEqual();

            edges.Add((3, 1));
            in1.Add(3); // add 3 -> 1
            AssertGraphEqual();

            edges.Add((4, 1));
            in1.Add(4); // add 4 -> 1
            AssertGraphEqual();

            edges.Add((2, 3));
            out2.Add(3); // add 2 -> 3
            AssertGraphEqual();

            //we remove the node 1; G = ({2, 3, 4}, {2 -> 3})
            RemoveNode(1);
            graph.Nodes.Remove(1);
            AssertGraphEqual();

            //add a new instance 1 that is not bound to the previous collection
            nodes.Add(1);
            graph.Nodes.Add(1);
            AssertGraphEqual();

            edges.Add((1, 2));
            graph.Edges.Add((1, 2));
            AssertGraphEqual();

            //we add a few edges and start clearing
            out1 = graph.GetOuts(1);
            in1 = graph.GetIns(1);

            edges.Add((2, 4));
            out2.Add(4);
            AssertGraphEqual();

            edges.Add((1, 2));
            in2.Add(1);
            AssertGraphEqual();

            edges.Add((2, 2));
            out2.Add(2);
            AssertGraphEqual();

            out2.Clear();
            ClearNode(2);
            AssertGraphEqual();

            edges.Add((1, 2));
            out1.Add(2);
            AssertGraphEqual();

            edges.Add((2, 2));
            in2.Add(2);
            AssertGraphEqual();

            edges.Add((3, 2));
            graph.GetOuts(3).Add(2);
            AssertGraphEqual();

            in2.Clear();
            ClearNode(2);
            AssertGraphEqual();

            edges.Add((1, 2));
            out1.Add(2);
            AssertGraphEqual();

            edges.Add((1, 2));
            in2.Add(1);
            AssertGraphEqual();

            edges.Add((3, 2));
            graph.GetOuts(3).Add(2);
            AssertGraphEqual();

            edges.Clear();
            graph.Edges.Clear();
            AssertGraphEqual();

            edges.Add((1, 2));
            out1.Add(2);
            AssertGraphEqual();

            edges.Add((1, 2));
            in2.Add(1);
            AssertGraphEqual();

            edges.Add((3, 2));
            graph.GetOuts(3).Add(2);
            AssertGraphEqual();
        }

        [Fact]
        public void TestUndirectedOutsInsOpSequenceEnumerator()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);
            graph.Nodes.Add(3);
            graph.Nodes.Add(4);
            graph.Nodes.Add(5);

            graph.Edges.AddSet((1, 2), (1, 3));
            graph.Edges.AddSet((2, 3), (2, 5));
            graph.Edges.AddSet((3, 3), (3, 5));
            graph.Edges.AddSet((4, 1), (4, 2));

            /*
             * 1 - 2
             * 1 - 3
             * 2 - 3
             * 2 - 5
             * 3 - 3
             * 3 - 5
             * 4 - 1
             * 4 - 2
             */

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            in1.Clear();

            /*
             * 2 - 3
             * 2 - 5
             * 3 - 3
             * 3 - 5
             * 4 - 2
             */
            
            Assert.DoesNotContain((4, 1), graph.Edges);
            Assert.DoesNotContain((1, 4), graph.Edges);
            Assert.DoesNotContain((2, 1), graph.Edges);
            Assert.DoesNotContain((1, 2), graph.Edges);
            Assert.DoesNotContain((3, 1), graph.Edges);
            Assert.DoesNotContain((1, 3), graph.Edges);
            Assert.Equal(5, graph.Edges.Count);
            AssertEmpty(in1);
            AssertEmpty(out1);
            Assert.Single(graph.GetOuts(4)); // still contains 4 - 2

            out2.Clear();

            /*
             * 3 - 3
             * 3 - 5
             */

            AssertEmpty(out2);
            Assert.DoesNotContain((4, 2), graph.Edges);
            Assert.DoesNotContain((2, 4), graph.Edges);
            Assert.DoesNotContain((5, 2), graph.Edges);
            Assert.DoesNotContain((2, 5), graph.Edges);

            Assert.Equal(2, graph.Edges.Count);
        }

        [Fact]
        public void TestUndirectedNodesClearAndSwipeEdges()
        {
            var graph = new Graph<int>(false);

            graph.Nodes.Add(1);
            graph.Nodes.Add(2);
            graph.Nodes.Add(3);
            graph.Nodes.Add(4);
            graph.Nodes.Add(5);

            graph.Edges.AddSet((1, 2), (1, 3));
            graph.Edges.AddSet((2, 3), (2, 5));
            graph.Edges.AddSet((3, 3), (3, 5));
            graph.Edges.AddSet((4, 1), (4, 2));

            var out1 = graph.GetOuts(1);
            var out2 = graph.GetOuts(2);

            var in1 = graph.GetIns(1);
            var in2 = graph.GetIns(2);

            graph.Nodes.Clear();

            AssertEmpty(graph.Edges);

            Assert.True(in1.IsDetached);
            Assert.True(in2.IsDetached);
            Assert.True(out1.IsDetached);
            Assert.True(out2.IsDetached);
        }
    }
}
