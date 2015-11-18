using System.Linq;
using Codartis.SoftVis.Diagramming.Layout.Incremental;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic;
using Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Builders;
using FluentAssertions;
using Xunit;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Relative
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

            _testGraphBuilder.Graph.RemoveEdge(GetEdge("A<-B"));
            TestGraph.GetLayerIndex(GetVertex("B")).Should().Be(0);
        }

        [Fact]
        public void RemoveEdge_ThenAddEdge_PushingTheSourceToHigherLayer()
        {
            _testGraphBuilder.SetUp("A<-B");
            _testGraphBuilder.Graph.RemoveEdge(GetEdge("A<-B"));
            _testGraphBuilder.SetUp("A<-C<-B");
            TestGraph.GetLayerIndex(GetVertex("B")).Should().Be(2);
        }

        [Fact]
        public void RemoveEdge_ThenAddEdge_SourceComesBackToLowerLayer()
        {
            _testGraphBuilder.SetUp("A<-B<-C");
            _testGraphBuilder.Graph.RemoveEdge(GetEdge("B<-C"));
            _testGraphBuilder.SetUp("A<-C");
            TestGraph.GetLayerIndex(GetVertex("C")).Should().Be(1);
        }

        [Fact]
        public void GetParents_Works()
        {
            _testGraphBuilder.SetUp("P1<-C1", "P1<-C2", "P1<-C3", "P2<-C2", "P2<-C3", "C2<-CC2", "P4<-C4");
            TestGraph.GetParents(GetVertex("C2")).Select(i => i.Name)
                .ShouldBeEquivalentTo(new[] { "P1", "P2" });
        }

        [Fact]
        public void GetChildren_Works()
        {
            _testGraphBuilder.SetUp("P1<-C1", "P1<-C2", "P1<-C3", "P2<-C2", "P2<-C3", "C2<-CC2", "P4<-C4");
            TestGraph.GetChildren(GetVertex("P2")).Select(i => i.Name)
                .ShouldBeEquivalentTo(new[] { "C2", "C3" });
        }

        [Fact]
        public void GetSiblings_Works()
        {
            _testGraphBuilder.SetUp("P1<-C1", "P1<-C2", "P1<-C3", "P2<-C2", "P2<-C3", "C2<-CC2", "P4<-C4");
            TestGraph.GetSiblings(GetVertex("C2")).Select(i=>i.Name)
                .ShouldBeEquivalentTo(new[] {"C1", "C3"});
        }

        [Fact]
        public void SplitEdge_Works()
        {
            _testGraphBuilder.SetUp(
                "P<-C",
                "P<-I1<-I2<-C"
                );
            _testGraphBuilder.GetEdge("P<-C").Length.Should().Be(3);
        }

        [Fact]
        public void MergeEdge_Works()
        {
            _testGraphBuilder.SetUp(
                "P1<-C",
                "P2<-I1<-I2<-C"
                );
            _testGraphBuilder.GetEdge("P1<-C").Length.Should().Be(3);

            _testGraphBuilder.SetUp("I1<-P1");
            _testGraphBuilder.GetEdge("P1<-C").Length.Should().Be(1);
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
