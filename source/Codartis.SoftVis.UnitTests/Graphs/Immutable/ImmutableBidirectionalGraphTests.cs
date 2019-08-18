using System;
using System.Collections.Generic;
using Codartis.SoftVis.Graphs;
using Codartis.SoftVis.Graphs.Immutable;
using Codartis.Util;
using FluentAssertions;
using Xunit;

namespace Codartis.SoftVis.UnitTests.Graphs.Immutable
{
    using TestGraph = ImmutableBidirectionalGraph<TestVertex, int, TestEdge, int>;

    public class ImmutableBidirectionalGraphTests
    {
        [Fact]
        public void AddVertex_Works()
        {
            var graph = TestGraph.Empty().AddVertex(new TestVertex(1, "a"));

            graph.Vertices.Should().BeEquivalentTo(new TestVertex(1, "a"));
        }

        [Fact]
        public void UpdateVertex_Works()
        {
            var testVertex = new TestVertex(1, "a");
            var graph = TestGraph.Empty()
                .AddVertex(testVertex)
                .UpdateVertex(testVertex.WithName("b"));

            graph.Vertices.Should().BeEquivalentTo(new TestVertex(1, "b"));
        }

        [Fact]
        public void RemoveVertex_Works()
        {
            var graph = TestGraph.Empty()
                .AddVertex(new TestVertex(1, "a"))
                .RemoveVertex(1);

            graph.Vertices.Should().BeEmpty();
        }

        [Fact]
        public void AddEdge_Works()
        {
            var v1 = new TestVertex(1, "v1");
            var v2 = new TestVertex(2, "v2");
            var e1 = new TestEdge(3, 1, 2, "e1");

            var graph = TestGraph.Empty()
                .AddVertex(v1)
                .AddVertex(v2)
                .AddEdge(e1);

            graph.Vertices.Should().BeEquivalentTo(v1, v2);
            graph.Edges.Should().BeEquivalentTo(e1);
        }

        [Fact]
        public void UpdateEdge_Name_Works()
        {
            var v1 = new TestVertex(1, "v1");
            var v2 = new TestVertex(2, "v2");
            var e1 = new TestEdge(1, 1, 2, "e1");

            var graph = TestGraph.Empty()
                .AddVertex(v1)
                .AddVertex(v2)
                .AddEdge(e1)
                .UpdateEdge(e1.WithName("e2"));

            graph.Vertices.Should().BeEquivalentTo(v1, v2);
            graph.Edges.Should().BeEquivalentTo(new TestEdge(1, 1, 2, "e2"));
        }

        [Fact]
        public void RemoveEdge_Works()
        {
            var v1 = new TestVertex(1, "v1");
            var v2 = new TestVertex(2, "v2");
            var e1 = new TestEdge(1, 1, 2, "e1");

            var graph = TestGraph.Empty()
                .AddVertex(v1)
                .AddVertex(v2)
                .AddEdge(e1)
                .RemoveEdge(1);

            graph.Vertices.Should().BeEquivalentTo(v1, v2);
            graph.Edges.Should().BeEmpty();
        }

        [Fact]
        public void GetVertex_VertexExists_Works()
        {
            var v1 = new TestVertex(1, "v1");
            TestGraph.Empty().AddVertex(v1).GetVertex(1).Should().Be(v1);
        }

        [Fact]
        public void GetVertex_VertexDoesNotExist_Throws()
        {
            Action action = () => TestGraph.Empty().GetVertex(1);
            action.Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public void TryGetVertex_VertexExists_Works()
        {
            var v1 = new TestVertex(1, "v1");
            TestGraph.Empty().AddVertex(v1).TryGetVertex(1).Should().Be(v1.ToMaybe());
        }

        [Fact]
        public void TryGetVertex_VertexDoesNotExist_Works()
        {
            TestGraph.Empty().TryGetVertex(1).Should().Be(Maybe<TestVertex>.Nothing);
        }

        [Fact]
        public void PathExists_Works()
        {
            var v1 = new TestVertex(1, "v1");
            var v2 = new TestVertex(2, "v2");
            var v3 = new TestVertex(3, "v3");
            var e1 = new TestEdge(1, 1, 2, "e1");
            var e2 = new TestEdge(2, 2, 3, "e2");

            var graph = TestGraph.Empty()
                .AddVertex(v1)
                .AddVertex(v2)
                .AddVertex(v3)
                .AddEdge(e1)
                .AddEdge(e2);

            graph.PathExists(1, 2).Should().BeTrue();
            graph.PathExists(1, 3).Should().BeTrue();
            graph.PathExists(1, 1).Should().BeFalse();
            graph.PathExists(2, 1).Should().BeFalse();
        }

        [Fact]
        public void GetAdjacentVertices_Works()
        {
            var v1 = new TestVertex(1, "v1");
            var v2 = new TestVertex(2, "v2");
            var v3 = new TestVertex(3, "v3");
            var e1 = new TestEdge(1, 1, 2, "e1");
            var e2 = new TestEdge(2, 2, 3, "e2");

            var graph = TestGraph.Empty()
                .AddVertex(v1)
                .AddVertex(v2)
                .AddVertex(v3)
                .AddEdge(e1)
                .AddEdge(e2);

            graph.GetAdjacentVertices(1, EdgeDirection.Out).Should().BeEquivalentTo(v2);
            graph.GetAdjacentVertices(1, EdgeDirection.In).Should().BeEmpty();
            graph.GetAdjacentVertices(3, EdgeDirection.In).Should().BeEquivalentTo(v2);
            graph.GetAdjacentVertices(1, EdgeDirection.Out, recursive: true).Should().BeEquivalentTo(v2, v3);
            graph.GetAdjacentVertices(3, EdgeDirection.In, recursive: true).Should().BeEquivalentTo(v1, v2);
            graph.GetAdjacentVertices(1, EdgeDirection.Out, edgePredicate: i => i.Name == "e1", recursive: true).Should().BeEquivalentTo(v2);
        }
    }

    public class TestVertex : IImmutableVertex<int>
    {
        public int Id { get; }
        public string Name { get; }

        public TestVertex(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public TestVertex WithName(string newName) => new TestVertex(Id, newName);
    }

    public class TestEdge : IImmutableEdge<int, TestEdge, int>
    {
        public int Id { get; }
        public int Source { get; }
        public int Target { get; }
        public string Name { get; }

        public TestEdge(int id, int source, int target, string name)
        {
            Id = id;
            Source = source;
            Target = target;
            Name = name;
        }

        public TestEdge WithName(string newName) => new TestEdge(Id, Source, Target, newName);
    }
}