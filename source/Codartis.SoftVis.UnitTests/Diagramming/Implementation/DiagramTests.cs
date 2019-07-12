using System.Linq;
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

            diagram.Nodes.Should().BeEquivalentTo(node);
            diagram.RootLayoutGroup.Nodes.Should().BeEquivalentTo(node);
        }

        [Fact]
        public void WithNode_Nested_Works()
        {
            var parentNode = new TestDiagramNode("parent");
            var childNode = new TestDiagramNode("child");

            var diagram = Diagram.Empty
                .AddNode(parentNode)
                .AddNode(childNode, parentNode.Id);

            diagram.Nodes.Select(i => i.Id).Should().BeEquivalentTo(parentNode.Id, childNode.Id);
            diagram.RootLayoutGroup.Nodes.Select(i => i.Id).Should().BeEquivalentTo(parentNode.Id);
            diagram.GetNode(parentNode.Id).As<IContainerDiagramNode>().LayoutGroup.Nodes.Select(i => i.Id).Should().BeEquivalentTo(childNode.Id);
        }

        [Fact]
        public void PathExists_WorksBetweenLayoutGroups()
        {
            var parentNode1 = new TestDiagramNode("parent1");
            var childNode1 = new TestDiagramNode("child1", parentNode1);
            var parentNode2 = new TestDiagramNode("parent2");
            var childNode2 = new TestDiagramNode("child2", parentNode2);
            var connectorChild1Child2 = new DiagramConnector(
                new TestModelRelationship(childNode1.ModelNode, childNode2.ModelNode),
                childNode1,
                childNode2,
                ConnectorTypes.Dependency);

            var diagram = Diagram.Empty
                .AddNode(parentNode1)
                .AddNode(childNode1, parentNode1.Id)
                .AddNode(parentNode2)
                .AddNode(childNode2, parentNode2.Id)
                .AddConnector(connectorChild1Child2);

            diagram.Nodes.Select(i => i.Id).Should().BeEquivalentTo(parentNode1.Id, childNode1.Id, parentNode2.Id, childNode2.Id);
            diagram.RootLayoutGroup.Nodes.Select(i => i.Id).Should().BeEquivalentTo(parentNode1.Id, parentNode2.Id);
            diagram.GetNode(parentNode1.Id).As<IContainerDiagramNode>().LayoutGroup.Nodes.Select(i => i.Id).Should().BeEquivalentTo(childNode1.Id);
            diagram.GetNode(parentNode2.Id).As<IContainerDiagramNode>().LayoutGroup.Nodes.Select(i => i.Id).Should().BeEquivalentTo(childNode2.Id);
            diagram.CrossLayoutGroupConnectors.Should().BeEquivalentTo(connectorChild1Child2);

            diagram.PathExists(childNode1.Id, childNode2.Id).Should().BeTrue();
            diagram.PathExists(childNode2.Id, childNode1.Id).Should().BeFalse();
            diagram.PathExists(parentNode1.Id, parentNode2.Id).Should().BeFalse();
        }
    }
}