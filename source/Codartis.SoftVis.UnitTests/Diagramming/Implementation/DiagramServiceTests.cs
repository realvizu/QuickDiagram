using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Modeling.Implementation;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation
{
    public class DiagramServiceTests
    {
        private static readonly Route TestRoute = new Route(new Point2D(1, 1), new Point2D(2, 2));

        [Fact]
        public void AddNode_Works()
        {
            var modelService = CreateModelService();
            var modelNode = modelService.AddNode("node");

            var diagramService = CreateDiagramService(modelService.LatestModel);
            diagramService.AddNode(modelNode.Id);

            var diagram = diagramService.LatestDiagram;
            diagram.Nodes.ShouldBeEquivalentById(modelNode.Id);
        }

        [Fact]
        public void UpdateNode_Works()
        {
            var modelService = CreateModelService();
            var parentNode = modelService.AddNode("parent");
            var childNode = modelService.AddNode("child", parentNodeId: parentNode.Id);

            var diagramService = CreateDiagramService(modelService.LatestModel);
            diagramService.AddNode(parentNode.Id);
            diagramService.AddNode(childNode.Id, parentNode.Id);
            diagramService.UpdatePayloadAreaSize(childNode.Id, new Size2D(1, 1));

            var diagram = diagramService.LatestDiagram;
            diagram.Nodes.ShouldBeEquivalentById(childNode.Id, parentNode.Id);
            diagram.GetNode(childNode.Id).PayloadAreaSize.Should().Be(new Size2D(1, 1));
        }

        [Fact]
        public void RemoveNode_Works()
        {
            var modelService = CreateModelService();
            var modelNode = modelService.AddNode("node");

            var diagramService = CreateDiagramService(modelService.LatestModel);
            diagramService.AddNode(modelNode.Id);
            diagramService.RemoveNode(modelNode.Id);

            var diagram = diagramService.LatestDiagram;
            diagram.Nodes.Should().BeEmpty();
        }

        [Fact]
        public void AddConnector_Works()
        {
            var modelService = CreateModelService();
            var node1 = modelService.AddNode("node1");
            var node2 = modelService.AddNode("node2");
            var relationship = modelService.AddRelationship(node1.Id, node2.Id);

            var diagramService = CreateDiagramService(modelService.LatestModel);
            diagramService.AddNodes(new[] { node1.Id, node2.Id });
            diagramService.AddConnector(relationship.Id);

            var diagram = diagramService.LatestDiagram;
            diagram.Connectors.ShouldBeEquivalentById(relationship.Id);
        }

        [Fact]
        public void UpdateConnector_Works()
        {
            var modelService = CreateModelService();
            var node1 = modelService.AddNode("node1");
            var node2 = modelService.AddNode("node2");
            var relationship = modelService.AddRelationship(node1.Id, node2.Id);

            var diagramService = CreateDiagramService(modelService.LatestModel);
            diagramService.AddNodes(new[] { node1.Id, node2.Id });
            diagramService.AddConnector(relationship.Id);
            diagramService.UpdateRoute(relationship.Id, TestRoute);

            diagramService.LatestDiagram.Connectors.Should().HaveCount(1).And
                .Subject.First().Route.Should().BeEquivalentTo(TestRoute);
        }

        [Fact]
        public void RemoveConnector_Works()
        {
            var modelService = CreateModelService();
            var node1 = modelService.AddNode("node1");
            var node2 = modelService.AddNode("node2");
            var relationship = modelService.AddRelationship(node1.Id, node2.Id);

            var diagramService = CreateDiagramService(modelService.LatestModel);
            diagramService.AddNodes(new[] { node1.Id, node2.Id });
            diagramService.AddConnector(relationship.Id);
            diagramService.RemoveConnector(relationship.Id);

            var diagram = diagramService.LatestDiagram;
            diagram.Connectors.Should().BeEmpty();
        }

        [Fact]
        public void PathExists_WorksInRootLayoutGroup()
        {
            var modelService = CreateModelService();
            var node1 = modelService.AddNode("node1");
            var node2 = modelService.AddNode("node2");
            var relationship = modelService.AddRelationship(node1.Id, node2.Id);

            var diagramService = CreateDiagramService(modelService.LatestModel);
            diagramService.AddNodes(new[] { node1.Id, node2.Id });
            diagramService.AddConnector(relationship.Id);

            var diagram = diagramService.LatestDiagram;
            diagram.PathExists(node1.Id, node2.Id).Should().BeTrue();
            diagram.PathExists(node2.Id, node1.Id).Should().BeFalse();
        }

        [Fact]
        public void PathExists_WorksInNestedLayoutGroup()
        {
            var modelService = CreateModelService();
            var parentNode = modelService.AddNode("parent");
            var childNode1 = modelService.AddNode("child1", parentNodeId: parentNode.Id);
            var childNode2 = modelService.AddNode("child2", parentNodeId: parentNode.Id);
            var relationship = modelService.AddRelationship(childNode1.Id, childNode2.Id);

            var diagramService = CreateDiagramService(modelService.LatestModel);
            diagramService.AddNodes(new[] { parentNode.Id, childNode1.Id, childNode2.Id });
            diagramService.AddConnector(relationship.Id);

            var diagram = diagramService.LatestDiagram;
            diagram.PathExists(childNode1.Id, childNode2.Id).Should().BeTrue();
            diagram.PathExists(childNode2.Id, childNode1.Id).Should().BeFalse();
        }

        [Fact]
        public void PathExists_WorksBetweenLayoutGroups()
        {
            var modelService = CreateModelService();
            var parentNode1 = modelService.AddNode("parent1");
            var childNode1 = modelService.AddNode("child1", parentNodeId: parentNode1.Id);
            var parentNode2 = modelService.AddNode("parent2");
            var childNode2 = modelService.AddNode("child2", parentNodeId: parentNode2.Id);
            var relationship = modelService.AddRelationship(childNode1.Id, childNode2.Id);

            var diagramService = CreateDiagramService(modelService.LatestModel);
            diagramService.AddNodes(new[] { parentNode1.Id, childNode1.Id, parentNode2.Id, childNode2.Id });
            diagramService.AddConnector(relationship.Id);

            var diagram = diagramService.LatestDiagram;
            diagram.PathExists(childNode1.Id, childNode2.Id).Should().BeTrue();
            diagram.PathExists(childNode2.Id, childNode1.Id).Should().BeFalse();
            diagram.PathExists(parentNode1.Id, parentNode2.Id).Should().BeFalse();
        }

        //[Fact]
        //public void ApplyLayout_Works()
        //{
        //    var modelService = CreateModelService();
        //    var node1 = modelService.AddNode("node1");
        //    var node2 = modelService.AddNode("node2");
        //    var relationship1 = modelService.AddRelationship(node1.Id, node2.Id);
        //    var relationship2 = modelService.AddRelationship(node2.Id, node1.Id);

        //    var diagramService = CreateDiagramService(modelService.LatestModel);
        //    diagramService.AddNodes(new[] { node1.Id, node2.Id });
        //    diagramService.AddConnectors(new[] { relationship1.Id, relationship2.Id });

        //    var expectedDiagram = diagramService.LatestDiagram
        //        .UpdateNode(node1.Id, new Point2D(1, 2))
        //        .UpdateNode(node2.Id, new Point2D(1, 2));

        //    var topLeft1 = new Point2D(1, 2);
        //    var topLeft2 = new Point2D(3, 4);
        //    var route1 = new Route((5, 6), (7, 8));
        //    var route2 = new Route((9, 10), (11, 12));
        //    var expectedLayoutGroup = LayoutGroup.Create(expectedDiagram.Nodes, expectedDiagram.Connectors);

        //    var expectedNodePositions = new[] { (node1.Id, topLeft1), (node2.Id, topLeft2) };
        //    var expectedConnectorRoutes = new[] { (relationship1.Id, route1), (relationship2.Id, route2) };

        //    using (var monitoredSubject = diagramService.Monitor())
        //    {
        //        diagramService.ApplyLayout(layout);

        //        monitoredSubject.Should().Raise(nameof(IDiagramService.DiagramChanged))
        //            .WithArgs2<DiagramNodeRectChangedEvent>(args => args.NewNode.Id == node1.Id && args.NewNode.TopLeft == topLeft1)
        //            .WithArgs2<DiagramNodeRectChangedEvent>(args => args.NewNode.Id == node2.Id && args.NewNode.TopLeft == topLeft2)
        //            .WithArgs2<DiagramConnectorRouteChangedEvent>(args => args.NewConnector.Id == relationship1.Id && args.NewConnector.Route == route1)
        //            .WithArgs2<DiagramConnectorRouteChangedEvent>(args => args.NewConnector.Id == relationship2.Id && args.NewConnector.Route == route2);
        //    }

        //    var diagram = diagramService.LatestDiagram;
        //    diagram.Nodes.Select(i => (i.Id, i.TopLeft)).Should().BeEquivalentTo((node1.Id, topLeft1), (node2.Id, topLeft2));
        //    diagram.Connectors.Select(i => (i.Id, i.Route)).Should().BeEquivalentTo((relationship1.Id, route1), (relationship2.Id, route2));
        //}

        [NotNull]
        private static IModelService CreateModelService() => new ModelService();

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