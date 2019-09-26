using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Diagramming.Implementation.Layout;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Modeling.Implementation;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout
{
    public class DiagramLayoutStructureTests
    {
        [Fact]
        public void Create_OnlyNodes_Works()
        {
            var modelService = CreateModelService();
            var parentNode = modelService.AddNode("parent");
            var childNode = modelService.AddNode("child", parentNodeId: parentNode.Id);

            var diagramService = CreateDiagramService(modelService.LatestModel);
            diagramService.AddNode(parentNode.Id);
            diagramService.AddNode(childNode.Id, parentNode.Id);

            var layoutStructure = new DiagramLayoutStructure(diagramService.LatestDiagram);
            layoutStructure.RootLayoutGroup.Nodes.ShouldBeEquivalentById(parentNode.Id);
            layoutStructure.TryGetLayoutGroupByNodeId(parentNode.Id).Value.Nodes.ShouldBeEquivalentById(childNode.Id);
        }

        [Fact]
        public void Create_ConnectorInRootLayoutGroup_Works()
        {
            var modelService = CreateModelService();
            var node1 = modelService.AddNode("node1");
            var node2 = modelService.AddNode("node2");
            var relationship = modelService.AddRelationship(node1.Id, node2.Id);

            var diagramService = CreateDiagramService(modelService.LatestModel);
            diagramService.AddNodes(new[] { node1.Id, node2.Id });
            diagramService.AddConnector(relationship.Id);

            var layoutStructure = new DiagramLayoutStructure(diagramService.LatestDiagram);
            layoutStructure.RootLayoutGroup.Connectors.ShouldBeEquivalentById(relationship.Id);
            layoutStructure.CrossLayoutGroupConnectors.Should().BeEmpty();
        }

        [Fact]
        public void Create_ConnectorInNestedLayoutGroup_Works()
        {
            var modelService = CreateModelService();
            var parentNode = modelService.AddNode("parent");
            var childNode1 = modelService.AddNode("child1", parentNodeId: parentNode.Id);
            var childNode2 = modelService.AddNode("child2", parentNodeId: parentNode.Id);
            var relationship = modelService.AddRelationship(childNode1.Id, childNode2.Id);

            var diagramService = CreateDiagramService(modelService.LatestModel);
            diagramService.AddNodes(new[] { parentNode.Id, childNode1.Id, childNode2.Id });
            diagramService.AddConnector(relationship.Id);

            var layoutStructure = new DiagramLayoutStructure(diagramService.LatestDiagram);
            layoutStructure.RootLayoutGroup.Connectors.Should().BeEmpty();
            layoutStructure.CrossLayoutGroupConnectors.Should().BeEmpty();
            layoutStructure.TryGetLayoutGroupByNodeId(parentNode.Id).Value.Connectors.ShouldBeEquivalentById(relationship.Id);
        }

        [Fact]
        public void Create_ConnectorInCrossLayoutGroup_Works()
        {
            var modelService = CreateModelService();
            var parentNode = modelService.AddNode("parent");
            var childNode = modelService.AddNode("child", parentNodeId: parentNode.Id);
            var relationship = modelService.AddRelationship(parentNode.Id, childNode.Id);

            var diagramService = CreateDiagramService(modelService.LatestModel);
            diagramService.AddNode(parentNode.Id);
            diagramService.AddNode(childNode.Id, parentNode.Id);
            diagramService.AddConnector(relationship.Id);

            var layoutStructure = new DiagramLayoutStructure(diagramService.LatestDiagram);
            layoutStructure.RootLayoutGroup.Connectors.Should().BeEmpty();
            layoutStructure.CrossLayoutGroupConnectors.ShouldBeEquivalentById(relationship.Id);
            layoutStructure.TryGetLayoutGroupByNodeId(parentNode.Id).Value.Connectors.Should().BeEmpty();
        }

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