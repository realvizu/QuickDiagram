using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic;
using Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Builders;
using FluentAssertions;
using Xunit;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Relative
{
    public class VerticesInLayerComparerTests
    {
        private readonly LowLevelLayoutGraphBuilder _testGraph;
        private LowLevelLayoutGraph LowLevelLayoutGraph => _testGraph.Graph;

        public VerticesInLayerComparerTests()
        {
            _testGraph = new LowLevelLayoutGraphBuilder();
        }

        [Fact]
        public void Compare_SimpleCases()
        {
            _testGraph.SetUp("A", "B");
            var comparer = new VerticesInLayerComparer(LowLevelLayoutGraph);
            comparer.Compare(_testGraph.GetVertex("A"), _testGraph.GetVertex("B")).Should().BeLessThan(0);
            comparer.Compare(_testGraph.GetVertex("B"), _testGraph.GetVertex("A")).Should().BeGreaterThan(0);
            comparer.Compare(_testGraph.GetVertex("A"), _testGraph.GetVertex("A")).Should().Be(0);
        }

        [Fact]
        public void Compare_DummyVerticesInvolved()
        {
            _testGraph.SetUp("*1<-A", "B", "*2<-C");
            var comparer = new VerticesInLayerComparer(LowLevelLayoutGraph);
            comparer.Compare(_testGraph.GetVertex("*1"), _testGraph.GetVertex("B")).Should().BeLessThan(0);
            comparer.Compare(_testGraph.GetVertex("*2"), _testGraph.GetVertex("B")).Should().BeGreaterThan(0);
        }
    }
}
