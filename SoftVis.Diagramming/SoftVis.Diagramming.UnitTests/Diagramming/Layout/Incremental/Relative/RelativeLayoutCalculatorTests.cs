using System.Linq;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic;
using Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Builders;
using Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Helpers;
using FluentAssertions;
using Xunit;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Relative
{
    public class RelativeLayoutCalculatorTests
    {
        private readonly DiagramGraphBuilder _diagramGraphBuilder;
        private readonly RelativeLayoutCalculator _calculator;

        public RelativeLayoutCalculatorTests()
        {
            _diagramGraphBuilder = new DiagramGraphBuilder();
            _calculator = new RelativeLayoutCalculator();
        }

        private DiagramGraph DiagramGraph => _diagramGraphBuilder.Graph;

        [Fact]
        public void OnDiagramNodeAdded_FirstNodeAdded()
        {
            _diagramGraphBuilder.SetUp("A");

            _calculator.MonitorEvents();
            _calculator.OnDiagramNodeAdded(GetVertex("A"), null);
            AssertAssignEvent("A", new RelativeLocation(0, 0));
        }

        [Fact]
        public void OnDiagramNodeAdded_SecondNodeAdded()
        {
            _diagramGraphBuilder.SetUp("A", "B");

            _calculator.MonitorEvents();
            _calculator.OnDiagramNodeAdded(GetVertex("A"), null);
            AssertAssignEvent("A", new RelativeLocation(0, 0));

            _calculator.MonitorEvents();
            _calculator.OnDiagramNodeAdded(GetVertex("B"), null);
            AssertAssignEvent("B", new RelativeLocation(0, 1));
        }

        [Fact]
        public void OnDiagramConnectorAdded_SourceVertexMoved()
        {
            _diagramGraphBuilder.SetUp("A<-B");
            SetUpCalculator("A", "B");

            _calculator.MonitorEvents();
            _calculator.OnDiagramConnectorAdded(GetEdge("A<-B"), null);
            AssertChangeEvent("B", new RelativeLocation(0, 1), new RelativeLocation(1, 0));
        }

        [Fact]
        public void OnDiagramConnectorAdded_SourceVertexMoved2Layers()
        {
            _diagramGraphBuilder.SetUp("A<-B<-C");
            SetUpCalculator("A<-B", "C");

            _calculator.MonitorEvents();
            _calculator.OnDiagramConnectorAdded(GetEdge("B<-C"), null);
            AssertChangeEvent("C", new RelativeLocation(0, 1), new RelativeLocation(2, 0));
        }

        [Fact]
        public void OnDiagramConnectorAdded_SourceVertexTreeMoved()
        {
            _diagramGraphBuilder.SetUp("P<-A<-B", "A<-C");
            SetUpCalculator("A<-B", "A<-C", "P");

            _calculator.MonitorEvents();
            _calculator.OnDiagramConnectorAdded(GetEdge("P<-A"), null);
            AssertChangeEvent("A", new RelativeLocation(0, 0), new RelativeLocation(1, 2));
            AssertChangeEvent("B", new RelativeLocation(1, 0), new RelativeLocation(2, 0));
            AssertChangeEvent("C", new RelativeLocation(1, 0), new RelativeLocation(2, 1));
        }

        [Fact]
        public void OnDiagramConnectorAdded_SourceAddedToSiblingsInOrder()
        {
            _diagramGraphBuilder.SetUp("A<-B", "A<-C");
            SetUpCalculator("A<-C", "B");

            _calculator.MonitorEvents();
            _calculator.OnDiagramConnectorAdded(GetEdge("A<-B"), null);
            AssertChangeEvent("B", new RelativeLocation(0, 1), new RelativeLocation(1, 0));
        }

        [Fact]
        public void OnDiagramConnectorAdded_ChildIsAlsoSibling()
        {
            _diagramGraphBuilder.SetUp("P<-A<-B", "A<-C<-B");
            SetUpCalculator("A<-B", "A<-C<-B", "P");

            _calculator.MonitorEvents();
            _calculator.OnDiagramConnectorAdded(GetEdge("P<-A"), null);
            AssertChangeEvent("A", new RelativeLocation(0, 0), new RelativeLocation(1, 2));
            AssertChangeEvent("C", new RelativeLocation(1, 1), new RelativeLocation(2, 1));
            AssertChangeEvent("B", new RelativeLocation(2, 0), new RelativeLocation(3, 0));
        }

        private void AssertChangeEvent(string vertexName, RelativeLocation @from, RelativeLocation to)
        {
            _calculator.ShouldRaise(nameof(RelativeLayoutCalculator.LayoutActionExecuted))
                .WithArgs<RelativeLocationChangedLayoutAction>(i => i.Vertex.Name == vertexName)
                .WithArgs<RelativeLocationChangedLayoutAction>(i => i.From == @from)
                .WithArgs<RelativeLocationChangedLayoutAction>(i => i.To == to);
        }

        private void AssertAssignEvent(string vertexName, RelativeLocation to)
        {
            _calculator.ShouldRaise(nameof(RelativeLayoutCalculator.LayoutActionExecuted))
                .WithArgs<RelativeLocationAssignedLayoutAction>(i => i.Vertex.Name == vertexName)
                .WithArgs<RelativeLocationAssignedLayoutAction>(i => i.To == to);
        }

        private DiagramConnector GetEdge(string edgeString)
        {
            var edgeSpecification = EdgeSpecification.Parse(edgeString);
            return GetEdge(edgeSpecification);
        }

        private DiagramConnector GetEdge(EdgeSpecification edgeSpecification)
        {
            return _diagramGraphBuilder.GetEdge(edgeSpecification.SourceVertexName, edgeSpecification.TargetVertexName);
        }

        private DiagramNode GetVertex(string vertexName)
        {
            return _diagramGraphBuilder.GetVertex(vertexName);
        }

        private void SetUpCalculator(params string[] pathStrings)
        {
            foreach (var pathString in pathStrings)
            {
                var pathSpecification = PathSpecification.Parse(pathString);

                foreach (var vertexName in pathSpecification)
                {
                    if (!VertexAlreadyAddedToCalculator(vertexName))
                        _calculator.OnDiagramNodeAdded(GetVertex(vertexName), null);
                }
                foreach (var edgeSpecification in pathSpecification.ToEdgeSpecifications())
                {
                    if (!EdgeAlreadyAddedToCalculator(edgeSpecification))
                        _calculator.OnDiagramConnectorAdded(GetEdge(edgeSpecification), null);
                }
            }
        }

        private bool VertexAlreadyAddedToCalculator(string vertexName)
        {
            return _calculator.RelativeLayout.HighLevelLayoutGraph.Vertices
                .Any(i => i.Name == vertexName);
        }

        private bool EdgeAlreadyAddedToCalculator(EdgeSpecification edgeSpecification)
        {
            return _calculator.RelativeLayout.HighLevelLayoutGraph.Edges
                .Any(i => i.Source.Name == edgeSpecification.SourceVertexName && i.Target.Name == edgeSpecification.TargetVertexName);
        }
    }
}
