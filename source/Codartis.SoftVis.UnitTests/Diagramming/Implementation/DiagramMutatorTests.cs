using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Events;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UnitTests.Modeling;
using FluentAssertions;
using JetBrains.Annotations;
using System.Linq;
using Xunit;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation
{
    public class DiagramMutatorTests
    {
        private static readonly Route TestRoute = new Route((1, 1), (2, 2));

        [NotNull] private readonly ModelBuilder _modelBuilder;

        public DiagramMutatorTests()
        {
            _modelBuilder = new ModelBuilder();
        }

        [Fact]
        public void AddNode_Works()
        {
            var model = _modelBuilder.AddNodes("A").Model;
            var node = _modelBuilder.GetNode("A");

            var diagramMutator = CreateDiagramMutator(model);
            diagramMutator.AddNode(node.Id);
            var diagramEvent = diagramMutator.GetDiagramEvent();

            diagramEvent.NewDiagram.Nodes.ShouldBeEquivalentById(node.Id);
            diagramEvent.ShapeEvents.Should().SatisfyRespectively(
                i => i.Should().BeOfType<DiagramNodeAddedEvent>().Which.NewNode.Id.Should().Be(node.Id)
            );
        }

        [Fact]
        public void AddChildToExistingNode_Works()
        {
            var model = _modelBuilder.AddNodes("A").AddChildNodes("A", "B").Model;
            var parentNode = _modelBuilder.GetNode("A");
            var childNode = _modelBuilder.GetNode("B");

            var diagram = new DiagramBuilder(model).AddNodes("A").GetDiagram();

            var diagramMutator = CreateDiagramMutator(diagram);
            diagramMutator.AddNode(childNode.Id);
            var diagramEvent = diagramMutator.GetDiagramEvent();

            diagramEvent.NewDiagram.Nodes.ShouldBeEquivalentById(parentNode.Id, childNode.Id);
            diagramEvent.ShapeEvents.Should().SatisfyRespectively(
                i =>
                {
                    var newNode = i.Should().BeOfType<DiagramNodeAddedEvent>().Which.NewNode;
                    newNode.Id.Should().Be(childNode.Id);
                },
                i =>
                {
                    var @event = i.Should().BeOfType<DiagramNodeChangedEvent>().Which;
                    @event.ChangedMember.Should().Be(DiagramNodeMember.ParentNode);
                    @event.NewNode.Id.Should().Be(childNode.Id);
                    @event.NewNode.ParentNodeId.Value.Should().Be(parentNode.Id);
                }
            );
        }

        [Fact]
        public void UpdateNodeSize_Works()
        {
            var model = _modelBuilder.AddNodes("A").Model;
            var node = _modelBuilder.GetNode("A");

            var diagram = new DiagramBuilder(model).AddAllModelNodes().GetDiagram();

            var diagramMutator = CreateDiagramMutator(diagram);
            diagramMutator.UpdateSize(node.Id, new Size2D(1, 1));
            var diagramEvent = diagramMutator.GetDiagramEvent();

            diagramEvent.NewDiagram.GetNode(node.Id).Size.Should().Be(new Size2D(1, 1));
            diagramEvent.ShapeEvents.Should().SatisfyRespectively(
                i => i.Should().BeOfType<DiagramNodeChangedEvent>().Which.NewNode.Size.Should().Be(new Size2D(1, 1))
            );
        }

        /// <remarks>
        /// Parent children area size is updated only if the child have defined position and size.
        /// </remarks>
        [Fact]
        public void UpdateNodeHeaderSize_UpdatesParentChildrenAreaSize()
        {
            var model = _modelBuilder.AddNodes("A").AddChildNodes("A", "B").Model;
            var parentNode = _modelBuilder.GetNode("A");
            var childNode = _modelBuilder.GetNode("B");

            var diagram = new DiagramBuilder(model)
                .AddNodes("A")
                .AddNodes(("B", (1, 1)))
                .UpdateNodeTopLeft("B", (0, 0))
                .GetDiagram();

            var diagramMutator = CreateDiagramMutator(diagram);
            diagramMutator.UpdateSize(childNode.Id, (2, 2));
            var diagramEvent = diagramMutator.GetDiagramEvent();

            diagramEvent.ShapeEvents.Should().SatisfyRespectively(
                i =>
                {
                    var changedEvent = i.Should().BeOfType<DiagramNodeChangedEvent>().Subject;
                    changedEvent.NewNode.Id.Should().Be(childNode.Id);
                    changedEvent.ChangedMember.Should().Be(DiagramNodeMember.Size);
                    changedEvent.NewNode.Size.Should().Be(new Size2D(2, 2));
                },
                i =>
                {
                    var changedEvent = i.Should().BeOfType<DiagramNodeChangedEvent>().Subject;
                    changedEvent.NewNode.Id.Should().Be(parentNode.Id);
                    changedEvent.ChangedMember.Should().Be(DiagramNodeMember.ChildrenAreaSize);
                    changedEvent.NewNode.ChildrenAreaSize.Should().Be(new Size2D(4, 4));
                });
        }

        [Fact]
        public void RemoveNode_Works()
        {
            var model = _modelBuilder.AddNodes("A").Model;
            var node = _modelBuilder.GetNode("A");

            var diagram = new DiagramBuilder(model).AddAllModelNodes().GetDiagram();

            var diagramMutator = CreateDiagramMutator(diagram);
            diagramMutator.RemoveNode(node.Id);
            var diagramEvent = diagramMutator.GetDiagramEvent();

            diagramEvent.NewDiagram.Nodes.Should().BeEmpty();
            diagramEvent.ShapeEvents.Should().SatisfyRespectively(
                i => i.Should().BeOfType<DiagramNodeRemovedEvent>().Which.OldNode.Id.Should().Be(node.Id)
            );
        }

        [Fact]
        public void AddConnector_Works()
        {
            var model = _modelBuilder.AddNodes("A", "B").AddRelationships("A->B").Model;
            var relationship = _modelBuilder.GetRelationship("A->B");

            var diagram = new DiagramBuilder(model).AddAllModelNodes().GetDiagram();

            var diagramMutator = CreateDiagramMutator(diagram);
            diagramMutator.AddConnector(relationship.Id);
            var diagramEvent = diagramMutator.GetDiagramEvent();

            diagramEvent.NewDiagram.Connectors.ShouldBeEquivalentById(relationship.Id);
            diagramEvent.ShapeEvents.Should().SatisfyRespectively(
                i => i.Should().BeOfType<DiagramConnectorAddedEvent>().Which.NewConnector.Id.Should().Be(relationship.Id)
            );
        }

        [Fact]
        public void UpdateConnector_Works()
        {
            var model = _modelBuilder.AddNodes("A", "B").AddRelationships("A->B").Model;
            var relationship = _modelBuilder.GetRelationship("A->B");

            var diagram = new DiagramBuilder(model).AddAllModelItems().GetDiagram();

            var diagramMutator = CreateDiagramMutator(diagram);
            diagramMutator.UpdateConnectorRoute(relationship.Id, TestRoute);
            var diagramEvent = diagramMutator.GetDiagramEvent();

            diagramEvent.NewDiagram.Connectors.Single().Route.Should().BeEquivalentTo(TestRoute);
            diagramEvent.ShapeEvents.Should().SatisfyRespectively(
                i => i.Should().BeOfType<DiagramConnectorRouteChangedEvent>().Which.NewConnector.Route.Should().BeEquivalentTo(TestRoute)
            );
        }

        [Fact]
        public void RemoveConnector_Works()
        {
            var model = _modelBuilder.AddNodes("A", "B").AddRelationships("A->B").Model;
            var relationship = _modelBuilder.GetRelationship("A->B");

            var diagram = new DiagramBuilder(model).AddAllModelItems().GetDiagram();

            var diagramMutator = CreateDiagramMutator(diagram);
            diagramMutator.RemoveConnector(relationship.Id);
            var diagramEvent = diagramMutator.GetDiagramEvent();

            diagramEvent.NewDiagram.Connectors.Should().BeEmpty();
            diagramEvent.ShapeEvents.Should().SatisfyRespectively(
                i => i.Should().BeOfType<DiagramConnectorRemovedEvent>().Which.OldConnector.Id.Should().Be(relationship.Id)
            );
        }

        [NotNull]
        private static DiagramMutator CreateDiagramMutator([NotNull] IModel model)
        {
            var diagram = ImmutableDiagram.Create(model, new DummyModelRelationshipFeatureProvider());
            return CreateDiagramMutator(diagram);
        }

        [NotNull]
        private static DiagramMutator CreateDiagramMutator(
            [NotNull] IDiagram diagram,
            IConnectorTypeResolver connectorTypeResolver = null,
            double childrenAreaPadding = 1)
        {
            return new DiagramMutator(
                diagram,
                connectorTypeResolver ?? new DummyConnectorTypeResolver(),
                new DummyModelRelationshipFeatureProvider(),
                childrenAreaPadding);
        }
    }
}