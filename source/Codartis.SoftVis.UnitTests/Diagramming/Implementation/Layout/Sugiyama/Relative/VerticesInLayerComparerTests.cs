using Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama;
using Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama.Relative;
using Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama.Relative.Logic;
using Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout.Sugiyama.Builders;
using FluentAssertions;
using Xunit;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout.Sugiyama.Relative
{
    public class VerticesInLayerComparerTests
    {
        private readonly QuasiProperLayoutGraphBuilder _testGraphBuilder;
        private IReadOnlyQuasiProperLayoutGraph TestGraph => _testGraphBuilder.Graph;

        public VerticesInLayerComparerTests()
        {
            _testGraphBuilder = new QuasiProperLayoutGraphBuilder();
        }

        [Fact]
        public void Compare_SimpleCases()
        {
            _testGraphBuilder.SetUp("A", "B");
            var comparer = new SiblingsComparer(TestGraph);
            comparer.Compare(GetVertex("A"), GetVertex("B")).Should().BeLessThan(0);
            comparer.Compare(GetVertex("B"), GetVertex("A")).Should().BeGreaterThan(0);
            comparer.Compare(GetVertex("A"), GetVertex("A")).Should().Be(0);
        }

        [Fact]
        public void Compare_DummyVerticesInvolved()
        {
            _testGraphBuilder.SetUp("P<-A<-B", "P<-*1<-B", "P<-C");
            var comparer = new SiblingsComparer(TestGraph);
            var dummyVertex = GetVertex("*1");
            comparer.Compare(GetVertex("A"), dummyVertex).Should().BeLessThan(0);
            comparer.Compare(dummyVertex, GetVertex("C")).Should().BeLessThan(0);
        }

        private LayoutVertexBase GetVertex(string name) => _testGraphBuilder.GetVertex(name);
    }
}
