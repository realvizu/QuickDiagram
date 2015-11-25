using System.Linq;
using Codartis.SoftVis.Diagramming.Layout.Incremental;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic;
using Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Builders;
using Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Helpers;
using Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.TestSubjects;
using FluentAssertions;
using Xunit;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Relative
{
    public class RelativeLayoutCalculatorTests
    {
        private readonly RelativeLayoutCalculatorBuilder _calculatorBuilder;
        private RelativeLayoutCalculator Calculator => _calculatorBuilder.RelativeLayoutCalculator;
        private IReadOnlyLayoutVertexLayers Layers => Calculator.RelativeLayout.LayoutVertexLayers;

        public RelativeLayoutCalculatorTests()
        {
            _calculatorBuilder = new RelativeLayoutCalculatorBuilder();
        }

        [Fact]
        public void OnDiagramNodeAdded_FirstNodeAdded()
        {
            Calculator.OnDiagramNodeAdded(CreateVertex("A"), null);
            AssertLocation("A", new RelativeLocation(0, 0));
        }

        [Fact]
        public void OnDiagramNodeAdded_SecondNodeAdded()
        {
            Calculator.OnDiagramNodeAdded(CreateVertex("A"), null);
            AssertLocation("A", new RelativeLocation(0, 0));

            Calculator.OnDiagramNodeAdded(CreateVertex("B"), null);
            AssertLocation("B", new RelativeLocation(0, 1));
        }

        [Fact]
        public void OnDiagramNodeRemove_Works()
        {
            _calculatorBuilder.SetUp("A");

            Calculator.OnDiagramNodeRemoved(GetVertex("A"), null);
            GetVertex("A").Should().BeNull();
        }

        [Fact]
        public void OnDiagramConnectorAdded_SourceVertexMoved()
        {
            _calculatorBuilder.SetUp("A", "B");

            Calculator.OnDiagramConnectorAdded(CreateEdge("A<-B"), null);
            AssertLocation("B", new RelativeLocation(1, 0));
        }

        [Fact]
        public void OnDiagramConnectorAdded_SourceVertexMoved2Layers()
        {
            _calculatorBuilder.SetUp("A<-B", "C");

            Calculator.OnDiagramConnectorAdded(CreateEdge("B<-C"), null);
            AssertLocation("C", new RelativeLocation(2, 0));
        }

        [Fact]
        public void OnDiagramConnectorAdded_SplitEdge_DummyVertexAdded()
        {
            DummyLayoutVertex.ResetUniqueIdCounter();
            _calculatorBuilder.SetUp("A<-B<-C");

            Calculator.OnDiagramConnectorAdded(CreateEdge("A<-C"), null);
            AssertLocation(GetDummyVertex("#1"), new RelativeLocation(1, 1));
        }

        [Fact]
        public void OnDiagramConnectorAdded_MergeEdge_DummyVertexRemoved()
        {
            DummyLayoutVertex.ResetUniqueIdCounter();
            _calculatorBuilder.SetUp("A<-B<-C", "D<-C", "E");

            var dummy = GetDummyVertex("#1");
            Layers.GetLocation(dummy).Should().Be(new RelativeLocation(1, 1));

            Calculator.OnDiagramConnectorAdded(CreateEdge("E<-D"), null);
            Layers.HasLocation(dummy).Should().BeFalse();
        }

        [Fact]
        public void OnDiagramConnectorRemoved_DummyVertexRemoved()
        {
            DummyLayoutVertex.ResetUniqueIdCounter();
            _calculatorBuilder.SetUp("A<-B<-C", "A<-C");

            Calculator.OnDiagramConnectorRemoved(GetEdge("A<-C"), null);
            GetDummyVertex("#1").Should().BeNull();
        }

        [Fact]
        public void OnDiagramConnectorAdded_SourceVertexTreeMoved()
        {
            _calculatorBuilder.SetUp("A<-B", "A<-C", "P");

            Calculator.OnDiagramConnectorAdded(CreateEdge("P<-A"), null);
            AssertLocation("A", new RelativeLocation(1, 0));
            AssertLocation("B", new RelativeLocation(2, 0));
            AssertLocation("C", new RelativeLocation(2, 1));
        }

        [Fact]
        public void OnDiagramConnectorAdded_SourceAddedToSiblingsInOrder()
        {
            _calculatorBuilder.SetUp("A<-C", "B");

            Calculator.OnDiagramConnectorAdded(CreateEdge("A<-B"), null);
            AssertLocation("B", new RelativeLocation(1, 0));
        }

        [Fact]
        public void OnDiagramConnectorAdded_ChildIsAlsoSibling()
        {
            _calculatorBuilder.SetUp("A<-C", "A<-B<-C", "P");

            Calculator.OnDiagramConnectorAdded(CreateEdge("P<-A"), null);
            AssertLocation("A", new RelativeLocation(1, 0));
            AssertLocation("B", new RelativeLocation(2, 0));
            AssertLocation("C", new RelativeLocation(3, 0));
        }

        private DiagramNodeLayoutVertex CreateVertex(string name)
        {
            return new TestLayoutVertex(name);
        }

        private LayoutPath CreateEdge(string edgeString)
        {
            var edge = EdgeSpecification.Parse(edgeString);
            var sourceVertex = GetVertex(edge.SourceVertexName);
            var targetVertex = GetVertex(edge.TargetVertexName);
            return new LayoutPath(sourceVertex, targetVertex, null);
        }

        private void AssertLocation(LayoutVertexBase vertex, RelativeLocation to)
        {
            Layers.GetLocation(vertex).Should().Be(to);
        }

        private void AssertLocation(string vertexName, RelativeLocation to)
        {
            AssertLocation(GetVertex(vertexName), to);
        }

        private LayoutPath GetEdge(string edgeString)
        {
            var edgeSpecification = EdgeSpecification.Parse(edgeString);
            return GetEdge(edgeSpecification);
        }

        private LayoutPath GetEdge(EdgeSpecification edgeSpecification)
        {
            return Calculator.RelativeLayout.LayeredLayoutGraph.Edges
                .FirstOrDefault(i => i.Source.Name == edgeSpecification.SourceVertexName
                && i.Target.Name == edgeSpecification.TargetVertexName);
        }

        private DiagramNodeLayoutVertex GetVertex(string vertexName)
        {
            return Calculator.RelativeLayout.LayeredLayoutGraph.Vertices.FirstOrDefault(i => i.Name == vertexName);
        }

        private DummyLayoutVertex GetDummyVertex(string dummyPostfix)
        {
            return Calculator.ProperLayoutGraph.Vertices.OfType<DummyLayoutVertex>()
                .FirstOrDefault(i => i.Name.EndsWith(dummyPostfix));
        }
    }
}
