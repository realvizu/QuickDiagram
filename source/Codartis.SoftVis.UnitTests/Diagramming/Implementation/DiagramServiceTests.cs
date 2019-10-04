using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Events;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UnitTests.Modeling;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation
{
    public class DiagramServiceTests
    {
        private static readonly Route TestRoute = new Route(new Point2D(1, 1), new Point2D(2, 2));

        [NotNull] private readonly ModelBuilder _modelBuilder;

        public DiagramServiceTests()
        {
            _modelBuilder = new ModelBuilder();
        }

        [Fact]
        public void AddNode_Works()
        {
            var model = _modelBuilder.AddNodes("A").Model;
            var node = _modelBuilder.GetNode("A");

            var diagramService = CreateDiagramService(model);
            diagramService.AddNode(node.Id);

            var diagram = diagramService.LatestDiagram;
            diagram.Nodes.ShouldBeEquivalentById(node.Id);
        }

        [Fact]
        public void UpdateNode_Works()
        {
            var model = _modelBuilder.AddNodes("Parent").AddChildNodes("Parent", "Child").Model;
            var parentNode = _modelBuilder.GetNode("Parent");
            var childNode = _modelBuilder.GetNode("Child");

            var diagramService = CreateDiagramService(model);
            diagramService.AddNode(parentNode.Id);
            diagramService.AddNode(childNode.Id, parentNode.Id);
            diagramService.UpdateNodePayloadAreaSize(childNode.Id, new Size2D(1, 1));

            var diagram = diagramService.LatestDiagram;
            diagram.Nodes.ShouldBeEquivalentById(childNode.Id, parentNode.Id);
            diagram.GetNode(childNode.Id).PayloadAreaSize.Should().Be(new Size2D(1, 1));
        }

        [Fact]
        public void RemoveNode_Works()
        {
            var model = _modelBuilder.AddNodes("A").Model;
            var node = _modelBuilder.GetNode("A");

            var diagramService = CreateDiagramService(model);
            diagramService.AddNode(node.Id);
            diagramService.RemoveNode(node.Id);

            var diagram = diagramService.LatestDiagram;
            diagram.Nodes.Should().BeEmpty();
        }

        [Fact]
        public void AddConnector_Works()
        {
            var model = _modelBuilder.AddNodes("A", "B").AddRelationships("A->B").Model;
            var node1 = _modelBuilder.GetNode("A");
            var node2 = _modelBuilder.GetNode("B");
            var relationship = _modelBuilder.GetRelationship("A->B");

            var diagramService = CreateDiagramService(model);
            diagramService.AddNodes(new[] { node1.Id, node2.Id });
            diagramService.AddConnector(relationship.Id);

            var diagram = diagramService.LatestDiagram;
            diagram.Connectors.ShouldBeEquivalentById(relationship.Id);
        }

        [Fact]
        public void UpdateConnector_Works()
        {
            var model = _modelBuilder.AddNodes("A", "B").AddRelationships("A->B").Model;
            var node1 = _modelBuilder.GetNode("A");
            var node2 = _modelBuilder.GetNode("B");
            var relationship = _modelBuilder.GetRelationship("A->B");

            var diagramService = CreateDiagramService(model);
            diagramService.AddNodes(new[] { node1.Id, node2.Id });
            diagramService.AddConnector(relationship.Id);
            diagramService.UpdateConnectorRoute(relationship.Id, TestRoute);

            diagramService.LatestDiagram.Connectors.Should().HaveCount(1).And
                .Subject.First().Route.Should().BeEquivalentTo(TestRoute);
        }

        [Fact]
        public void RemoveConnector_Works()
        {
            var model = _modelBuilder.AddNodes("A", "B").AddRelationships("A->B").Model;
            var node1 = _modelBuilder.GetNode("A");
            var node2 = _modelBuilder.GetNode("B");
            var relationship = _modelBuilder.GetRelationship("A->B");

            var diagramService = CreateDiagramService(model);
            diagramService.AddNodes(new[] { node1.Id, node2.Id });
            diagramService.AddConnector(relationship.Id);
            diagramService.RemoveConnector(relationship.Id);

            var diagram = diagramService.LatestDiagram;
            diagram.Connectors.Should().BeEmpty();
        }

        [Fact]
        public void PathExists_WorksInRootLayoutGroup()
        {
            var model = _modelBuilder.AddNodes("A", "B").AddRelationships("A->B").Model;
            var node1 = _modelBuilder.GetNode("A");
            var node2 = _modelBuilder.GetNode("B");
            var relationship = _modelBuilder.GetRelationship("A->B");

            var diagramService = CreateDiagramService(model);
            diagramService.AddNodes(new[] { node1.Id, node2.Id });
            diagramService.AddConnector(relationship.Id);

            var diagram = diagramService.LatestDiagram;
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

            var parentNode = _modelBuilder.GetNode("parent");
            var childNode1 = _modelBuilder.GetNode("child1");
            var childNode2 = _modelBuilder.GetNode("child2");
            var relationship = _modelBuilder.GetRelationship("child1->child2");

            var diagramService = CreateDiagramService(model);
            diagramService.AddNodes(new[] { parentNode.Id, childNode1.Id, childNode2.Id });
            diagramService.AddConnector(relationship.Id);

            var diagram = diagramService.LatestDiagram;
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
            var relationship = _modelBuilder.GetRelationship("child1->child2");

            var diagramService = CreateDiagramService(model);
            diagramService.AddNodes(new[] { parentNode1.Id, childNode1.Id, parentNode2.Id, childNode2.Id });
            diagramService.AddConnector(relationship.Id);

            var diagram = diagramService.LatestDiagram;
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

            var diagramBuilder = new DiagramBuilder(model)
                .AddNodes(("A", 0, 0), ("B", 0, 0))
                .AddAllModelRelationships();

            var diagramService = CreateDiagramService(diagramBuilder.Diagram);

            var layout = new DiagramLayoutInfo(
                new[]
                {
                    new NodeLayoutInfo(diagramBuilder.GetDiagramNode("A"), new Point2D(1, 1)),
                    new NodeLayoutInfo(diagramBuilder.GetDiagramNode("B"), new Point2D(2, 2))
                },
                new List<ConnectorLayoutInfo>());

            var expectedDiagram = diagramBuilder
                .UpdateNodeTopLeft("A", 1, 1)
                .UpdateNodeTopLeft("B", 2, 2)
                .Diagram;

            using (var monitoredSubject = ((IDiagramEventSource)diagramService).Monitor())
            {
                diagramService.ApplyLayout(layout);

                monitoredSubject.OccurredEvents
                    .SelectMany(i => i.Parameters).OfType<DiagramEvent>()
                    .SelectMany(i => i.ShapeEvents)
                    .Should().SatisfyRespectively(
                        i => i.Should().BeOfType<DiagramNodeRectChangedEvent>().Which.NewNode.Rect.Should().Be(new Rect2D(1, 1, 1, 1)),
                        i => i.Should().BeOfType<DiagramNodeRectChangedEvent>().Which.NewNode.Rect.Should().Be(new Rect2D(2, 2, 2, 2))
                    );

                // TODO: check connectors updated too
            }

            AllRectsShouldMatch(diagramService.LatestDiagram, expectedDiagram);
        }

        // TODO
        //[Fact]
        //public void ApplyLayout_WithChildNodes_Works()
        //{
        //}

        private static void AllRectsShouldMatch([NotNull] IDiagram actualDiagram, [NotNull] IDiagram expectedDiagram)
        {
            actualDiagram.Nodes.OrderBy(i => i.Id).Select(i => i.Rect).Should().Equal(expectedDiagram.Nodes.OrderBy(i => i.Id).Select(i => i.Rect));
            actualDiagram.Connectors.OrderBy(i => i.Id).Select(i => i.Rect).Should().Equal(expectedDiagram.Connectors.OrderBy(i => i.Id).Select(i => i.Rect));
        }

        [NotNull]
        private static IDiagramService CreateDiagramService([NotNull] IDiagram diagram)
        {
            return new DiagramService(diagram, new DummyConnectorTypeResolver());
        }

        [NotNull]
        private static IDiagramService CreateDiagramService([NotNull] IModel model)
        {
            return new DiagramService(model, new DummyConnectorTypeResolver());
        }

        private sealed class DummyConnectorTypeResolver : IConnectorTypeResolver
        {
            public ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype) => ConnectorTypes.Dependency;
        }
    }
}