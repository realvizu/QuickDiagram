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
        public void AddNodeToParent_Works()
        {
            var model = _modelBuilder.AddNodes("A").AddChildNodes("A", "B").Model;
            var parentNode = _modelBuilder.GetNode("A");
            var childNode = _modelBuilder.GetNode("B");

            var diagram = new DiagramBuilder(model).AddNodes("A").GetDiagram();

            var diagramMutator = CreateDiagramMutator(diagram);
            diagramMutator.AddNode(childNode.Id, parentNode.Id);
            var diagramEvent = diagramMutator.GetDiagramEvent();

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
        public void UpdateNodeHeaderSize_Works()
        {
            var model = _modelBuilder.AddNodes("A").Model;
            var node = _modelBuilder.GetNode("A");

            var diagram = new DiagramBuilder(model).AddAllModelNodes().GetDiagram();

            var diagramMutator = CreateDiagramMutator(diagram);
            diagramMutator.UpdateNodeHeaderSize(node.Id, new Size2D(1, 1));
            var diagramEvent = diagramMutator.GetDiagramEvent();

            diagramEvent.NewDiagram.GetNode(node.Id).HeaderSize.Should().Be(new Size2D(1, 1));
            diagramEvent.ShapeEvents.Should().SatisfyRespectively(
                i => i.Should().BeOfType<DiagramNodeChangedEvent>().Which.NewNode.HeaderSize.Should().Be(new Size2D(1, 1))
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
                .AddChildNodes("A", ("B", (1, 1)))
                .UpdateNodeTopLeft("B", (0, 0))
                .GetDiagram();

            var diagramMutator = CreateDiagramMutator(diagram);
            diagramMutator.UpdateNodeHeaderSize(childNode.Id, (2, 2));
            var diagramEvent = diagramMutator.GetDiagramEvent();

            diagramEvent.ShapeEvents.Should().SatisfyRespectively(
                i =>
                {
                    var changedEvent = i.Should().BeOfType<DiagramNodeChangedEvent>().Subject;
                    changedEvent.NewNode.Id.Should().Be(childNode.Id);
                    changedEvent.ChangedMember.Should().Be(DiagramNodeMember.HeaderSize);
                    changedEvent.NewNode.HeaderSize.Should().Be(new Size2D(2, 2));
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

        //[Fact]
        //public void ApplyLayout_RootNodesOnly_Works()
        //{
        //    var model = _modelBuilder
        //        .AddNodes("A", "B")
        //        .AddRelationships("A->B")
        //        .Model;

        //    var diagramBuilder = new DiagramBuilder(model).AddAllModelItems();
        //    var diagram = diagramBuilder.GetDiagram();

        //    var diagramNodeA = diagramBuilder.GetDiagramNode("A");
        //    var diagramNodeB = diagramBuilder.GetDiagramNode("B");
        //    var diagramConnectorAtoB = diagramBuilder.GetDiagramConnector("A->B");

        //    var layout = new GroupLayoutInfo(
        //        new[]
        //        {
        //            new BoxLayoutInfo(diagramNodeA.ShapeId, topLeft: (1, 1), headerSize: Size2D.Zero, childrenAreaSize: Size2D.Zero),
        //            new BoxLayoutInfo(diagramNodeB.ShapeId, topLeft: (2, 2), headerSize: Size2D.Zero, childrenAreaSize: Size2D.Zero)
        //        },
        //        new[]
        //        {
        //            new LineLayoutInfo(diagramConnectorAtoB.ShapeId, new Route((1, 1), (2, 2)))
        //        }
        //    );

        //    var expectedDiagram = diagramBuilder
        //        .UpdateNodeTopLeft("A", (1, 1))
        //        .UpdateNodeTopLeft("B", (2, 2))
        //        .UpdateConnectorRoute("A->B", new Route((1, 1), (2, 2)))
        //        .GetDiagram();

        //    var diagramEvent = diagram.ApplyLayout(layout);
        //    diagramEvent.ShapeEvents.Should().SatisfyRespectively(
        //        i => i.Should().BeOfType<DiagramNodeChangedEvent>().Which.NewNode.Rect.Should().Be(new Rect2D(1, 1, 1, 1)),
        //        i => i.Should().BeOfType<DiagramNodeChangedEvent>().Which.NewNode.Rect.Should().Be(new Rect2D(2, 2, 2, 2)),
        //        i => i.Should().BeOfType<DiagramConnectorRouteChangedEvent>().Which.NewConnector.Route.Should().BeEquivalentTo(new Route((1, 1), (2, 2)))
        //    );

        //    AllRectsShouldMatch(diagramEvent.NewDiagram, expectedDiagram);
        //}

        //[Fact]
        //public void ApplyLayout_WithChildNodes_Works()
        //{
        //    var model = _modelBuilder
        //        .AddNodes("A", "B")
        //        .AddChildNodes("A", "A1", "A2")
        //        .AddRelationships("A->B", "A1->A2", "A2->B")
        //        .Model;

        //    var diagramBuilder = new DiagramBuilder(model).AddAllModelItems();
        //    var diagram = diagramBuilder.GetDiagram;

        //    var diagramNodeA = diagramBuilder.GetDiagramNode("A");
        //    var diagramNodeA1 = diagramBuilder.GetDiagramNode("A1");
        //    var diagramNodeA2 = diagramBuilder.GetDiagramNode("A2");
        //    var diagramNodeB = diagramBuilder.GetDiagramNode("B");

        //    var layout = new GroupLayoutInfo(
        //        new[]
        //        {
        //            new BoxLayoutInfo(
        //                diagramNodeA.ShapeId,
        //                topLeft: (1, 1),
        //                headerSize: Size2D.Zero,
        //                childrenAreaSize: (2, 2),
        //                new GroupLayoutInfo(
        //                    new[]
        //                    {
        //                        new BoxLayoutInfo(diagramNodeA1.ShapeId, topLeft: (2, 2), headerSize: Size2D.Zero, childrenAreaSize: Size2D.Zero),
        //                        new BoxLayoutInfo(diagramNodeA2.ShapeId, topLeft: (4, 4), headerSize: Size2D.Zero, childrenAreaSize: Size2D.Zero)
        //                    })),
        //            new BoxLayoutInfo(diagramNodeB.ShapeId, topLeft: (9, 9), headerSize: Size2D.Zero, childrenAreaSize: Size2D.Zero)
        //        },
        //        new List<LineLayoutInfo>());

        //    var expectedDiagram = diagramBuilder
        //        .UpdateNodeTopLeft("A", (1, 1))
        //        .UpdateNodeTopLeft("A1", (2, 2))
        //        .UpdateNodeTopLeft("A2", (4, 4))
        //        .UpdateNodeTopLeft("B", (9, 9))
        //        .GetDiagram;

        //    var diagramEvent = diagram.ApplyLayout(layout);

        //    AllRectsShouldMatch(diagramEvent.NewDiagram, expectedDiagram);

        //    diagramEvent.ShapeEvents.Should().SatisfyRespectively(
        //        i =>
        //        {
        //            i.Should().BeOfType<DiagramNodeChangedEvent>().Which.ChangedMember.Should().Be(DiagramNodeMember.Position);
        //            i.Should().BeOfType<DiagramNodeChangedEvent>().Which.NewNode.Rect.Should().Be(new Rect2D(2, 2, 2, 2));
        //        },
        //        i =>
        //        {
        //            i.Should().BeOfType<DiagramNodeChangedEvent>().Which.ChangedMember.Should().Be(DiagramNodeMember.Position);
        //            i.Should().BeOfType<DiagramNodeChangedEvent>().Which.NewNode.Rect.Should().Be(new Rect2D(4, 4, 4, 4));
        //        },
        //        i =>
        //        {
        //            i.Should().BeOfType<DiagramNodeChangedEvent>().Which.ChangedMember.Should().Be(DiagramNodeMember.Position);
        //            i.Should().BeOfType<DiagramNodeChangedEvent>().Which.NewNode.Rect.Should().Be(new Rect2D(1, 1, 3, 3));
        //        },
        //        i =>
        //        {
        //            i.Should().BeOfType<DiagramNodeChangedEvent>().Which.ChangedMember.Should().Be(DiagramNodeMember.ChildrenAreaSize);
        //            i.Should().BeOfType<DiagramNodeChangedEvent>().Which.NewNode.Rect.Should().Be(new Rect2D(1, 1, 3, 3));
        //        },
        //        i =>
        //        {
        //            i.Should().BeOfType<DiagramNodeChangedEvent>().Which.ChangedMember.Should().Be(DiagramNodeMember.Position);
        //            i.Should().BeOfType<DiagramNodeChangedEvent>().Which.NewNode.Rect.Should().Be(new Rect2D(9, 9, 9, 9));
        //        }
        //    );

        //    // TODO: check connectors updated too
        //}

        private static void AllRectsShouldMatch([NotNull] IDiagram actualDiagram, [NotNull] IDiagram expectedDiagram)
        {
            actualDiagram.Nodes.OrderBy(i => i.Id).Select(i => i.Rect).Should().Equal(expectedDiagram.Nodes.OrderBy(i => i.Id).Select(i => i.Rect));
            actualDiagram.Connectors.OrderBy(i => i.Id).Select(i => i.Rect).Should().Equal(expectedDiagram.Connectors.OrderBy(i => i.Id).Select(i => i.Rect));
        }

        [NotNull]
        private static DiagramMutator CreateDiagramMutator([NotNull] IModel model)
        {
            var diagram = ImmutableDiagram.Create(model);
            return CreateDiagramMutator(diagram);
        }

        [NotNull]
        private static DiagramMutator CreateDiagramMutator(
            [NotNull] IDiagram diagram,
            IConnectorTypeResolver connectorTypeResolver = null,
            double childrenAreaPadding = 1)
        {
            return new DiagramMutator(diagram, connectorTypeResolver ?? new DummyConnectorTypeResolver(), childrenAreaPadding);
        }

        /// <summary>
        /// A dummy resolver that always returns Dependency connector type.
        /// </summary>
        private sealed class DummyConnectorTypeResolver : IConnectorTypeResolver
        {
            public ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype) => ConnectorTypes.Dependency;
        }
    }
}