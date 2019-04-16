using Codartis.SoftVis.Graphs;
using FluentAssertions;
using QuickGraph;
using Xunit;

namespace Codartis.SoftVis.Diagramming.UnitTests.Graphs
{
    public class GraphExtensionsTests
    {
        [Fact]
        public void GetAdjacentVertices_NonRecursive_Works()
        {
            var graph = CreateGraph();

            graph.GetAdjacentVertices(1, EdgeDirection.Out).Should().BeEquivalentTo(new[] { 2, 3, 4 });
            graph.GetAdjacentVertices(1, EdgeDirection.In).Should().BeEquivalentTo(new[] { 5, 6, 7 });

            graph.GetAdjacentVertices(1, EdgeDirection.Out, e => e.Label == "a").Should().BeEquivalentTo(new[] { 2, 3, });
            graph.GetAdjacentVertices(1, EdgeDirection.In, e => e.Label == "a").Should().BeEquivalentTo(new[] { 5, 6 });

            graph.GetAdjacentVertices(1, EdgeDirection.Out, e => e.Label == "b").Should().BeEquivalentTo(new[] { 2, 4, });
            graph.GetAdjacentVertices(1, EdgeDirection.In, e => e.Label == "b").Should().BeEquivalentTo(new[] { 5, 7 });
        }

        [Fact]
        public void GetAdjacentVertices_Recursive_Works()
        {
            var graph = CreateGraph();

            graph.GetAdjacentVertices(1, EdgeDirection.Out, recursive: true).Should().BeEquivalentTo(new[] { 2, 3, 4, 8, 9, 10, 11 });
            graph.GetAdjacentVertices(1, EdgeDirection.In, recursive: true).Should().BeEquivalentTo(new[] { 5, 6, 7, 12, 13 });

            graph.GetAdjacentVertices(1, EdgeDirection.Out, e => e.Label == "a", true).Should().BeEquivalentTo(new[] { 2, 3, 8, 10 });
            graph.GetAdjacentVertices(1, EdgeDirection.In, e => e.Label == "a", true).Should().BeEquivalentTo(new[] { 5, 6, 12 });

            graph.GetAdjacentVertices(1, EdgeDirection.Out, e => e.Label == "b", true).Should().BeEquivalentTo(new[] { 2, 4, 9 });
            graph.GetAdjacentVertices(1, EdgeDirection.In, e => e.Label == "b", true).Should().BeEquivalentTo(new[] { 5, 7, 13 });
        }

        [Fact]
        public void GetAdjacentVertices_Recursive_WithCycle_Works()
        {
            var graph = CreateGraphWithCycle();

            graph.GetAdjacentVertices(1, EdgeDirection.Out, recursive: true).Should().BeEquivalentTo(new[] { 2, 3, 4, 8, 9, 10, 11, 12, 6 });
            graph.GetAdjacentVertices(1, EdgeDirection.In, recursive: true).Should().BeEquivalentTo(new[] { 5, 6, 7, 12, 13, 10, 3 });

            graph.GetAdjacentVertices(1, EdgeDirection.Out, e => e.Label == "a", true).Should().BeEquivalentTo(new[] { 2, 3, 8, 10, 12, 6 });
            graph.GetAdjacentVertices(1, EdgeDirection.In, e => e.Label == "a", true).Should().BeEquivalentTo(new[] { 5, 6, 12, 10, 3 });

            graph.GetAdjacentVertices(1, EdgeDirection.Out, e => e.Label == "b", true).Should().BeEquivalentTo(new[] { 2, 4, 9 });
            graph.GetAdjacentVertices(1, EdgeDirection.In, e => e.Label == "b", true).Should().BeEquivalentTo(new[] { 5, 7, 13 });
        }

        private static IntGraph CreateGraph()
        {
            var graph = new IntGraph();

            graph.AddVertexRange(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 });

            graph.AddEdge(new IntEdge(1, 2, "a"));
            graph.AddEdge(new IntEdge(1, 2, "b"));
            graph.AddEdge(new IntEdge(1, 3, "a"));
            graph.AddEdge(new IntEdge(1, 4, "b"));
            graph.AddEdge(new IntEdge(5, 1, "a"));
            graph.AddEdge(new IntEdge(5, 1, "b"));
            graph.AddEdge(new IntEdge(6, 1, "a"));
            graph.AddEdge(new IntEdge(7, 1, "b"));

            graph.AddEdge(new IntEdge(2, 8, "a"));
            graph.AddEdge(new IntEdge(2, 9, "b"));
            graph.AddEdge(new IntEdge(3, 10, "a"));
            graph.AddEdge(new IntEdge(3, 11, "b"));
            graph.AddEdge(new IntEdge(12, 6, "a"));
            graph.AddEdge(new IntEdge(13, 7, "b"));

            return graph;
        }

        private static IntGraph CreateGraphWithCycle()
        {
            var graph = CreateGraph();
            graph.AddEdge(new IntEdge(10, 12, "a"));
            return graph;
        }


        private class IntEdge : Edge<int>
        {
            public string Label { get; }

            public IntEdge(int source, int target, string label) : base(source, target)
            {
                Label = label;
            }
        }

        private class IntGraph : BidirectionalGraph<int, IntEdge>
        {
            public IntGraph() : base(allowParallelEdges: true)
            {
            }
        }
    }
}
