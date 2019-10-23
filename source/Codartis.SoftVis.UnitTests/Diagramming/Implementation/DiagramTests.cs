using System.Collections.Generic;
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
    public class DiagramTests
    {
        private static readonly Route TestRoute = new Route((1, 1), (2, 2));

        [NotNull] private readonly ModelBuilder _modelBuilder;

        public DiagramTests()
        {
            _modelBuilder = new ModelBuilder();
        }

        [Fact]
        public void AddNode_Works()
        {
            var model = _modelBuilder.AddNodes("A").Model;
            var node = _modelBuilder.GetNode("A");

            var diagram = CreateDiagram(model);
            var diagramEvent = diagram.AddNode(node.Id);

            diagramEvent.NewDiagram.Nodes.ShouldBeEquivalentById(node.Id);
            diagramEvent.ShapeEvents.Should().SatisfyRespectively(
                i => i.Should().BeOfType<DiagramNodeAddedEvent>().Which.NewNode.Id.Should().Be(node.Id)
            );
        }

        [Fact]
        public void AddNodeToParent_Works()
        {
            var model = _modelBuilder.AddNodes("A").AddChildNodes("A", "B").Model;
            var parentNode = _modelBuilder.GetNode("A");
            var childNode = _modelBuilder.GetNode("B");

            var diagram = CreateDiagram(model).AddNode(parentNode.Id).NewDiagram;
            var diagramEvent = diagram.AddNode(childNode.Id, parentNode.Id);

            diagramEvent.NewDiagram.Nodes.ShouldBeEquivalentById(parentNode.Id, childNode.Id);
            diagramEvent.ShapeEvents.Should().SatisfyRespectively(
                i =>
                {
                    var newNode = i.Should().BeOfType<DiagramNodeAddedEvent>().Which.NewNode;
                    newNode.Id.Should().Be(childNode.Id);
                    newNode.ParentNodeId.Value.Should().Be(parentNode.Id);
                });
        }

        [Fact]
        public void UpdateNodePayloadAreaSize_Works()
        {
            var model = _modelBuilder.AddNodes("A").Model;
            var node = _modelBuilder.GetNode("A");

            var diagram = new DiagramBuilder(model).AddAllModelNodes().Diagram;
            var diagramEvent = diagram.UpdateNodePayloadAreaSize(node.Id, new Size2D(1, 1));

            diagramEvent.NewDiagram.GetNode(node.Id).PayloadAreaSize.Should().Be(new Size2D(1, 1));
            diagramEvent.ShapeEvents.Should().SatisfyRespectively(
                i => i.Should().BeOfType<DiagramNodeChangedEvent>().Which.NewNode.PayloadAreaSize.Should().Be(new Size2D(1, 1))
            );
        }

        [Fact]
        public void RemoveNode_Works()
        {
            var model = _modelBuilder.AddNodes("A").Model;
            var node = _modelBuilder.GetNode("A");

            var diagram = new DiagramBuilder(model).AddAllModelNodes().Diagram;
            var diagramEvent = diagram.RemoveNode(node.Id);

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

            var diagram = new DiagramBuilder(model).AddAllModelNodes().Diagram;
            var diagramEvent = diagram.AddConnector(relationship.Id);

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

            var diagram = new DiagramBuilder(model).AddAllModelItems().Diagram;
            var diagramEvent = diagram.UpdateConnectorRoute(relationship.Id, TestRoute);

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

            var diagram = new DiagramBuilder(model).AddAllModelItems().Diagram;
            var diagramEvent = diagram.RemoveConnector(relationship.Id);

            diagramEvent.NewDiagram.Connectors.Should().BeEmpty();
            diagramEvent.ShapeEvents.Should().SatisfyRespectively(
                i => i.Should().BeOfType<DiagramConnectorRemovedEvent>().Which.OldConnector.Id.Should().Be(relationship.Id)
            );
        }

        [Fact]
        public void PathExists_WorksInRootLayoutGroup()
        {
            var model = _modelBuilder.AddNodes("A", "B").AddRelationships("A->B").Model;
            var node1 = _modelBuilder.GetNode("A");
            var node2 = _modelBuilder.GetNode("B");

            var diagram = new DiagramBuilder(model).AddAllModelItems().Diagram;

            diagram.PathExists(node1.Id, node2.Id).Should().BeTrue();
            diagram.PathExists(node2.Id, node1.Id).Should().BeFalse();
        }

        [Fact]
        public void PathExists_WorksInNestedLayoutGroup()
        {
            var model = _modelBuilder
                .AddNodes("parent")
                .AddChildNodes("parent", "child1", "child2")
                .AddRelationships("child1->child2")
                .Model;

            var childNode1 = _modelBuilder.GetNode("child1");
            var childNode2 = _modelBuilder.GetNode("child2");

            var diagram = new DiagramBuilder(model).AddAllModelItems().Diagram;

            diagram.PathExists(childNode1.Id, childNode2.Id).Should().BeTrue();
            diagram.PathExists(childNode2.Id, childNode1.Id).Should().BeFalse();
        }

        [Fact]
        public void PathExists_WorksBetweenLayoutGroups()
        {
            var model = _modelBuilder
                .AddNodes("parent1", "parent2")
                .AddChildNodes("parent1", "child1")
                .AddChildNodes("parent1", "child2")
                .AddRelationships("child1->child2")
                .Model;

            var parentNode1 = _modelBuilder.GetNode("parent1");
            var parentNode2 = _modelBuilder.GetNode("parent2");
            var childNode1 = _modelBuilder.GetNode("child1");
            var childNode2 = _modelBuilder.GetNode("child2");

            var diagram = new DiagramBuilder(model).AddAllModelItems().Diagram;

            diagram.PathExists(childNode1.Id, childNode2.Id).Should().BeTrue();
            diagram.PathExists(childNode2.Id, childNode1.Id).Should().BeFalse();
            diagram.PathExists(parentNode1.Id, parentNode2.Id).Should().BeFalse();
        }

        [Fact]
        public void ApplyLayout_RootNodesOnly_Works()
        {
            var model = _modelBuilder
                .AddNodes("A", "B")
                .AddRelationships("A->B")
                .Model;

            var diagramBuilder = new DiagramBuilder(model).AddAllModelItems();
            var diagram = diagramBuilder.Diagram;

            var diagramNodeA = diagramBuilder.GetDiagramNode("A");
            var diagramNodeB = diagramBuilder.GetDiagramNode("B");
            var diagramConnectorAtoB = diagramBuilder.GetDiagramConnector("A->B");

            var layout = new GroupLayoutInfo(
                new[]
                {
                    new BoxLayoutInfo(diagramNodeA.ShapeId, topLeft: (1, 1), payloadAreaSize: Size2D.Zero, childrenAreaSize: Size2D.Zero),
                    new BoxLayoutInfo(diagramNodeB.ShapeId, topLeft: (2, 2), payloadAreaSize: Size2D.Zero, childrenAreaSize: Size2D.Zero)
                },
                new[]
                {
                    new LineLayoutInfo(diagramConnectorAtoB.ShapeId, new Route((1, 1), (2, 2)))
                }
            );

            var expectedDiagram = diagramBuilder
                .UpdateNodeTopLeft("A", (1, 1))
                .UpdateNodeTopLeft("B", (2, 2))
                .UpdateConnectorRoute("A->B", new Route((1, 1), (2, 2)))
                .Diagram;

            var diagramEvent = diagram.ApplyLayout(layout);
            diagramEvent.ShapeEvents.Should().SatisfyRespectively(
                i => i.Should().BeOfType<DiagramNodeChangedEvent>().Which.NewNode.Rect.Should().Be(new Rect2D(1, 1, 1, 1)),
                i => i.Should().BeOfType<DiagramNodeChangedEvent>().Which.NewNode.Rect.Should().Be(new Rect2D(2, 2, 2, 2)),
                i => i.Should().BeOfType<DiagramConnectorRouteChangedEvent>().Which.NewConnector.Route.Should().BeEquivalentTo(new Route((1, 1), (2, 2)))
            );

            AllRectsShouldMatch(diagramEvent.NewDiagram, expectedDiagram);
        }

        [Fact]
        public void ApplyLayout_WithChildNodes_Works()
        {
            var model = _modelBuilder
                .AddNodes("A", "B")
                .AddChildNodes("A", "A1", "A2")
                .AddRelationships("A->B", "A1->A2", "A2->B")
                .Model;

            var diagramBuilder = new DiagramBuilder(model).AddAllModelItems();
            var diagram = diagramBuilder.Diagram;

            var diagramNodeA = diagramBuilder.GetDiagramNode("A");
            var diagramNodeA1 = diagramBuilder.GetDiagramNode("A1");
            var diagramNodeA2 = diagramBuilder.GetDiagramNode("A2");
            var diagramNodeB = diagramBuilder.GetDiagramNode("B");

            var layout = new GroupLayoutInfo(
                new[]
                {
                    new BoxLayoutInfo(
                        diagramNodeA.ShapeId,
                        topLeft: (1, 1),
                        payloadAreaSize: Size2D.Zero,
                        childrenAreaSize: (2, 2),
                        new GroupLayoutInfo(
                            new[]
                            {
                                new BoxLayoutInfo(diagramNodeA1.ShapeId, topLeft: (2, 2), payloadAreaSize: Size2D.Zero, childrenAreaSize: Size2D.Zero),
                                new BoxLayoutInfo(diagramNodeA2.ShapeId, topLeft: (4, 4), payloadAreaSize: Size2D.Zero, childrenAreaSize: Size2D.Zero)
                            })),
                    new BoxLayoutInfo(diagramNodeB.ShapeId, topLeft: (9, 9), payloadAreaSize: Size2D.Zero, childrenAreaSize: Size2D.Zero)
                },
                new List<LineLayoutInfo>());

            var expectedDiagram = diagramBuilder
                .UpdateNodeTopLeft("A", (1, 1)).UpdateChildrenAreaSize("A", 2, 2)
                .UpdateNodeTopLeft("A1", (2, 2))
                .UpdateNodeTopLeft("A2", (4, 4))
                .UpdateNodeTopLeft("B", (9, 9))
                .Diagram;

            var diagramEvent = diagram.ApplyLayout(layout);

            AllRectsShouldMatch(diagramEvent.NewDiagram, expectedDiagram);

            diagramEvent.ShapeEvents.Should().SatisfyRespectively(
                i =>
                {
                    i.Should().BeOfType<DiagramNodeChangedEvent>().Which.ChangedMember.Should().Be(DiagramNodeMember.Position);
                    i.Should().BeOfType<DiagramNodeChangedEvent>().Which.NewNode.Rect.Should().Be(new Rect2D(2, 2, 2, 2));
                },
                i =>
                {
                    i.Should().BeOfType<DiagramNodeChangedEvent>().Which.ChangedMember.Should().Be(DiagramNodeMember.Position);
                    i.Should().BeOfType<DiagramNodeChangedEvent>().Which.NewNode.Rect.Should().Be(new Rect2D(4, 4, 4, 4));
                },
                i =>
                {
                    i.Should().BeOfType<DiagramNodeChangedEvent>().Which.ChangedMember.Should().Be(DiagramNodeMember.Position);
                    i.Should().BeOfType<DiagramNodeChangedEvent>().Which.NewNode.Rect.Should().Be(new Rect2D(1, 1, 3, 3));
                },
                i =>
                {
                    i.Should().BeOfType<DiagramNodeChangedEvent>().Which.ChangedMember.Should().Be(DiagramNodeMember.ChildrenAreaSize);
                    i.Should().BeOfType<DiagramNodeChangedEvent>().Which.NewNode.Rect.Should().Be(new Rect2D(1, 1, 3, 3));
                },
                i =>
                {
                    i.Should().BeOfType<DiagramNodeChangedEvent>().Which.ChangedMember.Should().Be(DiagramNodeMember.Position);
                    i.Should().BeOfType<DiagramNodeChangedEvent>().Which.NewNode.Rect.Should().Be(new Rect2D(9, 9, 9, 9));
                }
            );

            // TODO: check connectors updated too
        }

        private static void AllRectsShouldMatch([NotNull] IDiagram actualDiagram, [NotNull] IDiagram expectedDiagram)
        {
            actualDiagram.Nodes.OrderBy(i => i.Id).Select(i => i.Rect).Should().Equal(expectedDiagram.Nodes.OrderBy(i => i.Id).Select(i => i.Rect));
            actualDiagram.Connectors.OrderBy(i => i.Id).Select(i => i.Rect).Should().Equal(expectedDiagram.Connectors.OrderBy(i => i.Id).Select(i => i.Rect));
        }

        [NotNull]
        private static IDiagram CreateDiagram([NotNull] IModel model)
        {
            return Diagram.Create(model, new DummyConnectorTypeResolver());
        }

        private sealed class DummyConnectorTypeResolver : IConnectorTypeResolver
        {
            public ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype) => ConnectorTypes.Dependency;
        }
    }
}