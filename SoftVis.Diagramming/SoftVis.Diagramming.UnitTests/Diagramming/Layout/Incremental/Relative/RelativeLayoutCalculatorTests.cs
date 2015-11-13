using System.Linq;
using Codartis.SoftVis.Diagramming.Layout.Incremental;
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
            AssertChangeEvent("B", new RelativeLocation(1, 0));
        }

        [Fact]
        public void OnDiagramConnectorAdded_SourceVertexMoved2Layers()
        {
            _diagramGraphBuilder.SetUp("A<-B<-C");
            SetUpCalculator("A<-B", "C");

            _calculator.MonitorEvents();
            _calculator.OnDiagramConnectorAdded(GetEdge("B<-C"), null);
            AssertChangeEvent("C", new RelativeLocation(2, 0));
        }

        [Fact]
        public void OnDiagramConnectorAdded_SourceVertexTreeMoved()
        {
            _diagramGraphBuilder.SetUp("P<-A<-B", "A<-C");
            SetUpCalculator("A<-B", "A<-C", "P");

            _calculator.MonitorEvents();
            _calculator.OnDiagramConnectorAdded(GetEdge("P<-A"), null);
            AssertChangeEvent("A", new RelativeLocation(1, 2));
            AssertChangeEvent("B", new RelativeLocation(2, 0));
            AssertChangeEvent("C", new RelativeLocation(2, 1));
        }

        [Fact]
        public void OnDiagramConnectorAdded_SourceAddedToSiblingsInOrder()
        {
            _diagramGraphBuilder.SetUp("A<-B", "A<-C");
            SetUpCalculator("A<-C", "B");

            _calculator.MonitorEvents();
            _calculator.OnDiagramConnectorAdded(GetEdge("A<-B"), null);
            AssertChangeEvent("B", new RelativeLocation(1, 0));
        }

        [Fact]
        public void OnDiagramConnectorAdded_ChildIsAlsoSibling()
        {
            _diagramGraphBuilder.SetUp("P<-A<-C", "A<-B<-C");
            SetUpCalculator("A<-C", "A<-B<-C", "P");

            _calculator.MonitorEvents();
            _calculator.OnDiagramConnectorAdded(GetEdge("P<-A"), null);
            AssertChangeEvent("A", new RelativeLocation(1, 2));
            AssertChangeEvent("B", new RelativeLocation(2, 0));
            AssertChangeEvent("C", new RelativeLocation(3, 0));
        }

        [Fact]
        public void OnDiagramConnectorAdded_SplitEdge()
        {
            _diagramGraphBuilder.SetUp("A<-B<-C", "A<-C");
            SetUpCalculator("A<-B<-C");

            _calculator.MonitorEvents();
            _calculator.OnDiagramConnectorAdded(GetEdge("A<-C"), null);
            AssertDummyCreatedEvent();
            AssertDummyAssignEvent(new RelativeLocation(1, 1));
        }

        [Fact]
        public void OnDiagramConnectorAdded_MergeEdge()
        {
            _diagramGraphBuilder.SetUp("A<-B<-C", "E<-D<-C");
            SetUpCalculator("A<-B<-C", "D<-C", "E");

            _calculator.MonitorEvents();
            _calculator.OnDiagramConnectorAdded(GetEdge("E<-D"), null);
            AssertDummyRemovedEvent();
        }

        private void AssertDummyCreatedEvent()
        {
            _calculator.ShouldRaise(nameof(RelativeLayoutCalculator.LayoutActionExecuted))
                .WithArgs<LayoutVertexAction>(i => i.Vertex.IsDummy && i.Action == "DummyVertexCreated");
        }

        private void AssertDummyRemovedEvent()
        {
            _calculator.ShouldRaise(nameof(RelativeLayoutCalculator.LayoutActionExecuted))
                .WithArgs<LayoutVertexAction>(i => i.Vertex.IsDummy && i.Action == "DummyVertexRemoved");
        }

        private void AssertChangeEvent(string vertexName, RelativeLocation to)
        {
            _calculator.ShouldRaise(nameof(RelativeLayoutCalculator.LayoutActionExecuted))
                .WithArgs<LayoutVertexAction>(i => i.Vertex.Name == vertexName &&
                i is RelativeLocationChangedLayoutAction && ((RelativeLocationChangedLayoutAction)i).To == to);
        }

        private void AssertAssignEvent(string vertexName, RelativeLocation to)
        {
            _calculator.ShouldRaise(nameof(RelativeLayoutCalculator.LayoutActionExecuted))
                .WithArgs<LayoutVertexAction>(i => i.Vertex.Name == vertexName &&
                i is RelativeLocationAssignedLayoutAction && ((RelativeLocationAssignedLayoutAction)i).To == to);
        }

        private void AssertDummyAssignEvent(RelativeLocation to)
        {
            _calculator.ShouldRaise(nameof(RelativeLayoutCalculator.LayoutActionExecuted))
                .WithArgs<LayoutVertexAction>(i => i.Vertex.IsDummy &&
                i is RelativeLocationAssignedLayoutAction && ((RelativeLocationAssignedLayoutAction)i).To == to);
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
