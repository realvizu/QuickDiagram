using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.UnitTests.TestSubjects;
using FluentAssertions;
using Xunit;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation
{
    public class DiagramTests
    {
        [Fact]
        public void WithNode_RootLevel_Works()
        {
            var node = new TestDiagramNode();
            var diagram = Diagram.Empty.AddNode(node);

            diagram.Nodes.ShouldBeEquivalentById(node);
            diagram.RootLayoutGroup.Nodes.ShouldBeEquivalentById(node);
        }

        [Fact]
        public void WithNode_Nested_Works()
        {
            var parentNode = new TestDiagramNode("parent");
            var childNode = new TestDiagramNode("child");

            var diagram = Diagram.Empty
                .AddNode(parentNode)
                .AddNode(childNode, parentNode.Id);

            diagram.Nodes.ShouldBeEquivalentById(childNode, parentNode);
            diagram.RootLayoutGroup.Nodes.ShouldBeEquivalentById(parentNode);
            diagram.GetNode(parentNode.Id).As<IContainerDiagramNode>().LayoutGroup.Nodes.ShouldBeEquivalentById(childNode);
        }

        [Fact]
        public void PathExists_WorksInRootLayoutGroup()
        {
            var node1 = new TestDiagramNode("node1");
            var node2 = new TestDiagramNode("node2");
            var testModelRelationship = new TestModelRelationship(node1.ModelNode, node2.ModelNode);
            var connectorNode1Node2 = new DiagramConnectorSpecification(testModelRelationship, ConnectorTypes.Dependency);

            var diagram = Diagram.Empty
                .AddNode(node1)
                .AddNode(node2)
                .AddConnector(connectorNode1Node2);

            diagram.Nodes.ShouldBeEquivalentById(node1, node2);
            diagram.RootLayoutGroup.Nodes.ShouldBeEquivalentById(node1, node2);
            diagram.CrossLayoutGroupConnectors.Should().BeEmpty();

            diagram.PathExists(node1.Id, node2.Id).Should().BeTrue();
            diagram.PathExists(node2.Id, node1.Id).Should().BeFalse();
        }

        [Fact]
        public void PathExists_WorksInSubLayoutGroup()
        {
            var parentNode = new TestDiagramNode("parent");
            var childNode1 = new TestDiagramNode("child1");
            var childNode2 = new TestDiagramNode("child2");
            var testModelRelationship = new TestModelRelationship(childNode1.ModelNode, childNode2.ModelNode);
            var connectorNode1Node2 = new DiagramConnectorSpecification(testModelRelationship, ConnectorTypes.Dependency);

            var diagram = Diagram.Empty
                .AddNode(parentNode)
                .AddNode(childNode1, parentNode.Id)
                .AddNode(childNode2, parentNode.Id)
                .AddConnector(connectorNode1Node2);

            diagram.Nodes.ShouldBeEquivalentById(parentNode, childNode1, childNode2);
            diagram.RootLayoutGroup.Nodes.ShouldBeEquivalentById(parentNode);
            diagram.CrossLayoutGroupConnectors.Should().BeEmpty();
            diagram.GetNode(parentNode.Id).As<IContainerDiagramNode>().LayoutGroup.Nodes.ShouldBeEquivalentById(childNode1, childNode2);

            diagram.PathExists(childNode1.Id, childNode2.Id).Should().BeTrue();
            diagram.PathExists(childNode2.Id, childNode1.Id).Should().BeFalse();
        }

        [Fact]
        public void PathExists_WorksBetweenLayoutGroups()
        {
            var parentNode1 = new TestDiagramNode("parent1");
            var childNode1 = new TestDiagramNode("child1");
            var parentNode2 = new TestDiagramNode("parent2");
            var childNode2 = new TestDiagramNode("child2");
            var testModelRelationship = new TestModelRelationship(childNode1.ModelNode, childNode2.ModelNode);
            var connectorChild1Child2 = new DiagramConnectorSpecification(testModelRelationship, ConnectorTypes.Dependency);

            var diagram = Diagram.Empty
                .AddNode(parentNode1)
                .AddNode(childNode1, parentNode1.Id)
                .AddNode(parentNode2)
                .AddNode(childNode2, parentNode2.Id)
                .AddConnector(connectorChild1Child2);

            diagram.Nodes.ShouldBeEquivalentById(parentNode1, childNode1, parentNode2, childNode2);
            diagram.RootLayoutGroup.Nodes.ShouldBeEquivalentById(parentNode1, parentNode2);
            diagram.GetNode(parentNode1.Id).As<IContainerDiagramNode>().LayoutGroup.Nodes.ShouldBeEquivalentById(childNode1);
            diagram.GetNode(parentNode2.Id).As<IContainerDiagramNode>().LayoutGroup.Nodes.ShouldBeEquivalentById(childNode2);
            diagram.CrossLayoutGroupConnectors.ShouldBeEquivalentById(connectorChild1Child2);

            diagram.PathExists(childNode1.Id, childNode2.Id).Should().BeTrue();
            diagram.PathExists(childNode2.Id, childNode1.Id).Should().BeFalse();
            diagram.PathExists(parentNode1.Id, parentNode2.Id).Should().BeFalse();
        }
    }
}