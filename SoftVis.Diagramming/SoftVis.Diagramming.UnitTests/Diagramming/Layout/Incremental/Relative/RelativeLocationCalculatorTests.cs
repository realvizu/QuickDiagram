using Codartis.SoftVis.Diagramming.Layout.Incremental;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic;
using Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Builders;
using FluentAssertions;
using Xunit;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Relative
{
    public class RelativeLocationCalculatorTests
    {
        private readonly RelativeLayoutBuilder _relativeLayoutBuilder;
        private readonly RelativeLocationCalculator _relativeLocationCalculator;

        public RelativeLocationCalculatorTests()
        {
            _relativeLayoutBuilder = new RelativeLayoutBuilder();
            _relativeLocationCalculator = new RelativeLocationCalculator(_relativeLayoutBuilder.RelativeLayout);
        }

        private LowLevelLayoutGraphBuilder LowLevelLayoutGraphBuilder => _relativeLayoutBuilder.LowLevelLayoutGraphBuilder;
        private HighLevelLayoutGraphBuilder HighLevelLayoutGraphBuilder => _relativeLayoutBuilder.HighLevelLayoutGraphBuilder;

        [Fact]
        public void GetTargetLocation_FirstVertex()
        {
            _relativeLayoutBuilder.SetUpGraphs("A");
            _relativeLocationCalculator.GetTargetLocation(GetVertex("A")).Should().Be(new RelativeLocation(0, 0));
        }

        [Fact]
        public void GetTargetLocation_SecondVertex()
        {
            _relativeLayoutBuilder.SetUpGraphs("A", "B");
            _relativeLayoutBuilder.SetUpLayers("A");
            _relativeLocationCalculator.GetTargetLocation(GetVertex("B")).Should().Be(new RelativeLocation(0, 1));
        }

        [Fact]
        public void GetTargetLocation_SimpleConnector()
        {
            _relativeLayoutBuilder.SetUpGraphs("A<-B");
            _relativeLayoutBuilder.SetUpLayers("A,B");
            _relativeLocationCalculator.GetTargetLocation(GetVertex("B")).Should().Be(new RelativeLocation(1, 0));
        }

        [Fact]
        public void GetTargetLocation_ConnectorWithDummyAsSource()
        {
            _relativeLayoutBuilder.SetUpGraphs("A<-B<-C", "D<-*1<-C");
            _relativeLayoutBuilder.SetUpLayers("A,B,C,D,*1");
            _relativeLocationCalculator.GetTargetLocation(GetVertex("*1")).Should().Be(new RelativeLocation(1, 0));
        }

        [Fact]
        public void GetTargetLocation_ConnectorWithDummyAsTarget()
        {
            _relativeLayoutBuilder.SetUpGraphs("A<-B<-C", "A<-*1<-C");
            _relativeLayoutBuilder.SetUpLayers("A,C", "B,*1");
            _relativeLocationCalculator.GetTargetLocation(GetVertex("C")).Should().Be(new RelativeLocation(2, 0));
        }

        [Fact]
        public void GetTargetLocation_LocationBasedOnParents()
        {
            _relativeLayoutBuilder.SetUpGraphs("P1<-C1", "P2<-C2", "P3<-C3");
            _relativeLayoutBuilder.SetUpLayers("P1,P2,P3", "C1,C3");
            _relativeLocationCalculator.GetTargetLocation(GetVertex("C2")).Should().Be(new RelativeLocation(1, 1));
        }

        [Fact]
        public void GetTargetLocation_LocationBasedOnSiblings()
        {
            _relativeLayoutBuilder.SetUpGraphs("P1<-C1", "P1<-C2", "P1<-C3");
            _relativeLayoutBuilder.SetUpLayers("P1", "C1,C3");
            _relativeLocationCalculator.GetTargetLocation(GetVertex("C2")).Should().Be(new RelativeLocation(1, 1));
        }

        [Fact]
        public void GetTargetLocation_LocationBasedOnDummySiblings()
        {
            _relativeLayoutBuilder.SetUpGraphs("P1<-*1<-C1", "P1<-C2", "P1<-*3<-C3");
            _relativeLayoutBuilder.SetUpLayers("P1", "*1,*3");
            _relativeLocationCalculator.GetTargetLocation(GetVertex("C2")).Should().Be(new RelativeLocation(1, 1));
        }

        [Fact]
        public void GetTargetLocation_SubjectVertexDoesNotInterfereWithCalculation()
        {
            _relativeLayoutBuilder.SetUpGraphs("A<-B");
            _relativeLayoutBuilder.SetUpLayers("A","B");
            _relativeLocationCalculator.GetTargetLocation(GetVertex("B")).Should().Be(new RelativeLocation(1, 0));
        }

        private LayoutVertexBase GetVertex(string vertexName)
        {
            return LowLevelLayoutGraphBuilder.GetVertex(vertexName);
        }
    }
}
