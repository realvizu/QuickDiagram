using Codartis.SoftVis.Diagramming.Layout.Incremental;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic;
using Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Builders;
using FluentAssertions;
using Xunit;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Relative
{
    public class LayeredGraphTests
    {
        private readonly LayeredGraphBuilder _layeredGraphBuilder;

        public LayeredGraphTests()
        {
            _layeredGraphBuilder = new LayeredGraphBuilder();
        }

        private LayeredGraph LayeredGraph => _layeredGraphBuilder.Graph;

        [Fact]
        public void AddVertex_LayerIndexSet()
        {
            _layeredGraphBuilder.SetUp("A");
            LayeredGraph.GetLayerIndex(GetVertex("A")).Should().Be(0);
        }

        [Fact]
        public void AddEdge_SourceLayerIndexUpdated()
        {
            _layeredGraphBuilder.SetUp("A<-B");
            LayeredGraph.GetLayerIndex(GetVertex("A")).Should().Be(0);
            LayeredGraph.GetLayerIndex(GetVertex("B")).Should().Be(1);
        }

        [Fact]
        public void AddEdge_SourceTreeUpdated()
        {
            _layeredGraphBuilder.SetUp("B<-C", "B<-D");
            LayeredGraph.GetLayerIndex(GetVertex("B")).Should().Be(0);
            LayeredGraph.GetLayerIndex(GetVertex("C")).Should().Be(1);
            LayeredGraph.GetLayerIndex(GetVertex("D")).Should().Be(1);

            _layeredGraphBuilder.SetUp("A<-B");
            LayeredGraph.GetLayerIndex(GetVertex("A")).Should().Be(0);
            LayeredGraph.GetLayerIndex(GetVertex("B")).Should().Be(1);
            LayeredGraph.GetLayerIndex(GetVertex("C")).Should().Be(2);
            LayeredGraph.GetLayerIndex(GetVertex("D")).Should().Be(2);
        }

        [Fact]
        public void RemoveEdge_LayerIndexPreserved()
        {
            _layeredGraphBuilder.SetUp("A<-B");
            LayeredGraph.GetLayerIndex(GetVertex("B")).Should().Be(1);

            _layeredGraphBuilder.Graph.RemoveEdge(GetEdge("A<-B"));
            LayeredGraph.GetLayerIndex(GetVertex("B")).Should().Be(1);
        }

        [Fact]
        public void RemoveEdge_ThenAddEdge_PushingTheSourceToHigherLayer()
        {
            _layeredGraphBuilder.SetUp("A<-B");
            _layeredGraphBuilder.Graph.RemoveEdge(GetEdge("A<-B"));
            _layeredGraphBuilder.SetUp("A<-C<-B");
            LayeredGraph.GetLayerIndex(GetVertex("B")).Should().Be(2);
        }

        [Fact]
        public void RemoveEdge_ThenAddEdge_SourceRemainsAtHigherLayer()
        {
            _layeredGraphBuilder.SetUp("A<-B<-C");
            _layeredGraphBuilder.Graph.RemoveEdge(GetEdge("B<-C"));
            _layeredGraphBuilder.SetUp("A<-C");
            LayeredGraph.GetLayerIndex(GetVertex("C")).Should().Be(2);
        }

        private LayoutPath GetEdge(string edgeString)
        {
            return _layeredGraphBuilder.GetEdge(edgeString);
        }

        private DiagramNodeLayoutVertex GetVertex(string vertexName)
        {
            return _layeredGraphBuilder.GetVertex(vertexName);
        }
    }
}
