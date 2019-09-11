using Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama;
using Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama.Relative;
using Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama.Relative.Logic;
using Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout.Sugiyama.Builders;
using FluentAssertions;
using Xunit;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout.Sugiyama.Relative
{
    public class RelativeLocationCalculatorTests
    {
        private readonly QuasiProperLayoutGraphBuilder _graphBuilder;
        private readonly LayoutVertexLayersBuilder _layersBuilder;
        private readonly RelativeLocationCalculator _calculator;

        public RelativeLocationCalculatorTests()
        {
            _graphBuilder = new QuasiProperLayoutGraphBuilder();
            _layersBuilder = new LayoutVertexLayersBuilder(_graphBuilder.Graph);
            _calculator = new RelativeLocationCalculator(_graphBuilder.Graph, _layersBuilder.Layers);
        }

        [Fact]
        public void GetTargetLocation_FirstVertex()
        {
            _graphBuilder.SetUp("A");
            _calculator.GetTargetLocation(GetVertex("A")).Should().Be(new RelativeLocation(0, 0));
        }

        [Fact]
        public void GetTargetLocation_SecondVertex()
        {
            _graphBuilder.SetUp("A", "B");
            _layersBuilder.SetUp("A");
            _calculator.GetTargetLocation(GetVertex("B")).Should().Be(new RelativeLocation(0, 1));
        }

        [Fact]
        public void GetTargetLocation_SimpleConnector()
        {
            _graphBuilder.SetUp("A<-B");
            _layersBuilder.SetUp("A");
            _calculator.GetTargetLocation(GetVertex("B")).Should().Be(new RelativeLocation(1, 0));
        }

        [Fact]
        public void GetTargetLocation_ConnectorWithDummyAsSource()
        {
            _graphBuilder.SetUp("A<-*1");
            _layersBuilder.SetUp("A");
            _calculator.GetTargetLocation(GetVertex("*1")).Should().Be(new RelativeLocation(1, 0));
        }

        [Fact]
        public void GetTargetLocation_ConnectorWithDummyAsTarget()
        {
            _graphBuilder.SetUp("A<-*1<-C");
            _layersBuilder.SetUp("A", "*1");
            _calculator.GetTargetLocation(GetVertex("C")).Should().Be(new RelativeLocation(2, 0));
        }

        [Fact]
        public void GetTargetLocation_LocationBasedOnParents()
        {
            _graphBuilder.SetUp("P1<-C1", "P2<-C2", "P3<-C3");
            _layersBuilder.SetUp("P1,P2,P3", "C1,C3");
            _calculator.GetTargetLocation(GetVertex("C2")).Should().Be(new RelativeLocation(1, 1));
        }

        [Fact]
        public void GetTargetLocation_LocationBasedOnSiblings()
        {
            _graphBuilder.SetUp("P1<-C1", "P1<-C2", "P1<-C3");
            _layersBuilder.SetUp("P1", "C1,C3");
            _calculator.GetTargetLocation(GetVertex("C2")).Should().Be(new RelativeLocation(1, 1));
        }

        [Fact]
        public void GetTargetLocation_LocationBasedOnDummySiblings()
        {
            _graphBuilder.SetUp("P1<-*1<-C1", "P1<-C2", "P1<-*3<-C3");
            _layersBuilder.SetUp("P1", "*1,*3");
            _calculator.GetTargetLocation(GetVertex("C2")).Should().Be(new RelativeLocation(1, 1));
        }

        private LayoutVertexBase GetVertex(string vertexName)
        {
            return _graphBuilder.GetVertex(vertexName);
        }
    }
}
