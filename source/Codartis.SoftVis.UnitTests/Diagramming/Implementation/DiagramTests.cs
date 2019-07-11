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
            var newNode = new TestDiagramNode();
            var newDiagram = Diagram.Empty.WithNode(newNode);

            newDiagram.Nodes.Should().BeEquivalentTo(newNode);
            newDiagram.RootLayoutGroup.Nodes.Should().BeEquivalentTo(newNode);
        }

        [Fact]
        public void WithNode_Nested_Works()
        {
            var parentNode = new TestDiagramNode("parent");
            var childNode = new TestDiagramNode("child");

            var newDiagram = Diagram.Empty
                .WithNode(parentNode)
                .WithNode(childNode, parentNode.Id);

            newDiagram.Nodes.Select(i => i.Id).Should().BeEquivalentTo(parentNode.Id, childNode.Id);
            newDiagram.RootLayoutGroup.Nodes.Select(i => i.Id).Should().BeEquivalentTo(parentNode.Id);
            newDiagram.GetNode(parentNode.Id).As<IContainerDiagramNode>().LayoutGroup.Nodes.Select(i => i.Id).Should().BeEquivalentTo(childNode.Id);
        }
    }
}