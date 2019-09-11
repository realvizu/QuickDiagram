using System.Linq;
using Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama;
using Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama.Relative.Logic;
using Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout.Sugiyama.Builders;
using FluentAssertions;
using Xunit;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout.Sugiyama.Relative
{
    public class LayeredLayoutGraphTests
    {
        private readonly LayeredLayoutGraphBuilder _testGraphBuilder;
        private LayeredLayoutGraph TestGraph => _testGraphBuilder.Graph;

        public LayeredLayoutGraphTests()
        {
            _testGraphBuilder = new LayeredLayoutGraphBuilder();
        }

        [Fact]
        public void AddVertex_LayerIndexSet()
        {
            _testGraphBuilder.SetUp("A");
            TestGraph.GetLayerIndex(GetVertex("A")).Should().Be(0);
        }

        [Fact]
        public void AddEdge_SourceLayerIndexUpdated()
        {
            _testGraphBuilder.SetUp("A<-B");
            TestGraph.GetLayerIndex(GetVertex("A")).Should().Be(0);
            TestGraph.GetLayerIndex(GetVertex("B")).Should().Be(1);
        }

        [Fact]
        public void AddEdge_SourceTreeLayerIndexUpdated()
        {
            _testGraphBuilder.SetUp("B<-C", "B<-D");
            TestGraph.GetLayerIndex(GetVertex("B")).Should().Be(0);
            TestGraph.GetLayerIndex(GetVertex("C")).Should().Be(1);
            TestGraph.GetLayerIndex(GetVertex("D")).Should().Be(1);

            _testGraphBuilder.SetUp("A<-B");
            TestGraph.GetLayerIndex(GetVertex("A")).Should().Be(0);
            TestGraph.GetLayerIndex(GetVertex("B")).Should().Be(1);
            TestGraph.GetLayerIndex(GetVertex("C")).Should().Be(2);
            TestGraph.GetLayerIndex(GetVertex("D")).Should().Be(2);
        }

        [Fact]
        public void RemoveEdge_LayerIndexUpdated()
        {
            _testGraphBuilder.SetUp("A<-B");
            TestGraph.GetLayerIndex(GetVertex("B")).Should().Be(1);

            TestGraph.RemoveEdge(GetEdge("A<-B"));
            TestGraph.GetLayerIndex(GetVertex("B")).Should().Be(0);
        }

        [Fact]
        public void RemoveEdge_ThenAddEdge_PushingTheSourceToHigherLayer()
        {
            _testGraphBuilder.SetUp("A<-B");
            TestGraph.RemoveEdge(GetEdge("A<-B"));
            _testGraphBuilder.SetUp("A<-C<-B");
            TestGraph.GetLayerIndex(GetVertex("B")).Should().Be(2);
        }

        [Fact]
        public void RemoveEdge_ThenAddEdge_SourceComesBackToLowerLayer()
        {
            _testGraphBuilder.SetUp("A<-B<-C");
            TestGraph.RemoveEdge(GetEdge("B<-C"));
            _testGraphBuilder.SetUp("A<-C");
            TestGraph.GetLayerIndex(GetVertex("C")).Should().Be(1);
        }

        [Fact]
        public void GetParents_Works()
        {
            _testGraphBuilder.SetUp("P1<-C1", "P1<-C2", "P1<-C3", "P2<-C2", "P2<-C3", "C2<-CC2", "P4<-C4");
            TestGraph.GetParents(GetVertex("C2")).Select(i => i.Name)
                .Should().BeEquivalentTo("P1", "P2");
        }

        [Fact]
        public void GetChildren_Works()
        {
            _testGraphBuilder.SetUp("P1<-C1", "P1<-C2", "P1<-C3", "P2<-C2", "P2<-C3", "C2<-CC2", "P4<-C4");
            TestGraph.GetChildren(GetVertex("P2")).Select(i => i.Name)
                .Should().BeEquivalentTo("C2", "C3");
        }

        [Fact]
        public void GetSiblings_Works()
        {
            _testGraphBuilder.SetUp("P1<-C1", "P1<-C2", "P1<-C3", "P2<-C2", "P2<-C3", "C2<-CC2", "P4<-C4");
            TestGraph.GetSiblings(GetVertex("C2")).Select(i=>i.Name)
                .Should().BeEquivalentTo("C1", "C3");
        }

        [Fact]
        public void GetDescendants_Works()
        {
            _testGraphBuilder.SetUp("P1<-C1<-C2", "C1<-C3", "P1<-C4", "P2");
            TestGraph.GetDescendants(GetVertex("P1")).Select(i => i.Name)
                .Should().BeEquivalentTo("C1", "C2", "C3", "C4");
        }

        [Fact]
        public void GetGetVertexAndDescendants_Works()
        {
            _testGraphBuilder.SetUp("P1<-C1<-C2", "C1<-C3", "P1<-C4", "P2");
            TestGraph.GetVertexAndDescendants(GetVertex("P1")).Select(i => i.Name)
                .Should().BeEquivalentTo("P1", "C1", "C2", "C3", "C4");
        }

        [Fact]
        public void SplitEdge_Works()
        {
            _testGraphBuilder.SetUp(
                "P<-C",
                "P<-I1<-I2<-C"
                );
            GetEdge("P<-C").Length.Should().Be(3);
        }

        [Fact]
        public void MergeEdge_WithAddEdge_Works()
        {
            _testGraphBuilder.SetUp(
                "P1<-C",
                "P2<-I1<-I2<-C"
                );
            GetEdge("P1<-C").Length.Should().Be(3);

            _testGraphBuilder.AddEdge("I2<-P1");
            GetEdge("P1<-C").Length.Should().Be(1);
            GetEdge("I2<-C").Length.Should().Be(2);
        }

        [Fact]
        public void MergeEdge_WithRemoveEdge_Works()
        {
            _testGraphBuilder.SetUp(
                "A<-B<-C",
                "A<-C"
                );
            GetEdge("A<-C").Length.Should().Be(2);

            _testGraphBuilder.RemoveEdge("B<-C");
            GetEdge("A<-C").Length.Should().Be(1);
        }

        [Fact]
        public void AddEdge_DescendantsAlsoGetNewLayerIndex()
        {
            _testGraphBuilder.SetUp(
                "A<-B<-C",
                "A<-C",
                "D"
                );
            _testGraphBuilder.AddEdge("D<-A");

            TestGraph.GetLayerIndex(GetVertex("A")).Should().Be(1);
            TestGraph.GetLayerIndex(GetVertex("B")).Should().Be(2);
            TestGraph.GetLayerIndex(GetVertex("C")).Should().Be(3);
        }

        private LayoutPath GetEdge(string edgeString)
        {
            return _testGraphBuilder.GetEdge(edgeString);
        }

        private DiagramNodeLayoutVertex GetVertex(string vertexName)
        {
            return _testGraphBuilder.GetVertex(vertexName);
        }
    }
}
