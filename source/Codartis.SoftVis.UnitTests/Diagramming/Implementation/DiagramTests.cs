//using System.Linq;
//using Codartis.SoftVis.Diagramming;
//using Codartis.SoftVis.Diagramming.Implementation;
//using Codartis.SoftVis.Geometry;
//using Codartis.SoftVis.UnitTests.TestSubjects;
//using FluentAssertions;
//using Xunit;

//namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation
//{
//    public class DiagramTests
//    {
//        private static readonly Route TestRoute = new Route(new Point2D(1,1),new Point2D(2,2));

//        [Fact]
//        public void WithNode_RootLevel_Works()
//        {
//            var node = new TestDiagramNode();
//            var diagram = Diagram.Empty.AddNode(node);

//            diagram.Nodes.ShouldBeEquivalentById(node);
//            diagram.RootLayoutGroup.Nodes.ShouldBeEquivalentById(node);
//        }

//        [Fact]
//        public void AddNode_Nested_Works()
//        {
//            var parentNode = new TestDiagramNode("parent");
//            var childNode = new TestDiagramNode("child");

//            var diagram = Diagram.Empty
//                .AddNode(parentNode)
//                .AddNode(childNode, parentNode.Id);

//            diagram.Nodes.ShouldBeEquivalentById(childNode, parentNode);
//            diagram.RootLayoutGroup.Nodes.ShouldBeEquivalentById(parentNode);
//            diagram.GetNode(parentNode.Id).As<IContainerDiagramNode>().LayoutGroup.Nodes.ShouldBeEquivalentById(childNode);
//        }

//        [Fact]
//        public void UpdateNode_Nested_Works()
//        {
//            var parentNode = new TestDiagramNode("parent");
//            var childNode = new TestDiagramNode("child");

//            var diagram = Diagram.Empty
//                .AddNode(parentNode)
//                .AddNode(childNode, parentNode.Id)
//                .UpdateNode(childNode.WithSize(new Size2D(1, 1)));

//            diagram.Nodes.ShouldBeEquivalentById(childNode, parentNode);
//            diagram.GetNode(childNode.Id).Size.Should().Be(new Size2D(1, 1));
//        }

//        [Fact]
//        public void RemoveNode_RootLevel_Works()
//        {
//            var node = new TestDiagramNode();

//            var diagram = Diagram.Empty
//                .AddNode(node)
//                .RemoveNode(node.Id);

//            diagram.Nodes.Should().BeEmpty();
//        }

//        [Fact]
//        public void RemoveNode_Nested_Works()
//        {
//            var parentNode = new TestDiagramNode("parent");
//            var childNode = new TestDiagramNode("child");

//            var diagram = Diagram.Empty
//                .AddNode(parentNode)
//                .AddNode(childNode, parentNode.Id)
//                .RemoveNode(childNode.Id);

//            diagram.Nodes.ShouldBeEquivalentById(parentNode);
//            diagram.GetNode(parentNode.Id).As<IContainerDiagramNode>().LayoutGroup.Nodes.Should().BeEmpty();
//        }

//        [Fact]
//        public void AddConnector_InRootLayoutGroup_Works()
//        {
//            var node1 = new TestDiagramNode("node1");
//            var node2 = new TestDiagramNode("node2");
//            var testModelRelationship = new TestModelRelationship(node1.ModelNode, node2.ModelNode);
//            var connectorNode1Node2 = new DiagramConnector(testModelRelationship, node1, node2, ConnectorTypes.Dependency);

//            var diagram = Diagram.Empty
//                .AddNode(node1)
//                .AddNode(node2)
//                .AddConnector(connectorNode1Node2);

//            diagram.Connectors.ShouldBeEquivalentById(connectorNode1Node2);
//            diagram.RootLayoutGroup.Connectors.ShouldBeEquivalentById(connectorNode1Node2);
//            diagram.CrossLayoutGroupConnectors.Should().BeEmpty();
//        }

//        [Fact]
//        public void AddConnector_InNestedLayoutGroup_Works()
//        {
//            var parentNode = new TestDiagramNode("parent");
//            var childNode1 = new TestDiagramNode("child1");
//            var childNode2 = new TestDiagramNode("child2");
//            var testModelRelationship = new TestModelRelationship(childNode1.ModelNode, childNode2.ModelNode);
//            var connectorNode1Node2 = new DiagramConnector(testModelRelationship, childNode1, childNode2, ConnectorTypes.Dependency);

//            var diagram = Diagram.Empty
//                .AddNode(parentNode)
//                .AddNode(childNode1, parentNode.Id)
//                .AddNode(childNode2, parentNode.Id)
//                .AddConnector(connectorNode1Node2);

//            diagram.Connectors.ShouldBeEquivalentById(connectorNode1Node2);
//            diagram.RootLayoutGroup.Connectors.Should().BeEmpty();
//            diagram.CrossLayoutGroupConnectors.Should().BeEmpty();
//            diagram.GetNode(parentNode.Id).As<IContainerDiagramNode>().LayoutGroup.Connectors.ShouldBeEquivalentById(connectorNode1Node2);
//        }

//        [Fact]
//        public void AddConnector_InCrossLayoutGroup_Works()
//        {
//            var parentNode = new TestDiagramNode("parent");
//            var childNode = new TestDiagramNode("child");
//            var testModelRelationship = new TestModelRelationship(parentNode.ModelNode, childNode.ModelNode);
//            var connectorParentChild = new DiagramConnector(testModelRelationship, parentNode, childNode, ConnectorTypes.Dependency);

//            var diagram = Diagram.Empty
//                .AddNode(parentNode)
//                .AddNode(childNode, parentNode.Id)
//                .AddConnector(connectorParentChild);

//            diagram.Connectors.ShouldBeEquivalentById(connectorParentChild);
//            diagram.RootLayoutGroup.Connectors.Should().BeEmpty();
//            diagram.CrossLayoutGroupConnectors.ShouldBeEquivalentById(connectorParentChild);
//            diagram.GetNode(parentNode.Id).As<IContainerDiagramNode>().LayoutGroup.Connectors.Should().BeEmpty();
//        }

//        [Fact]
//        public void UpdateConnector_InRootLayoutGroup_Works()
//        {
//            var node1 = new TestDiagramNode("node1");
//            var node2 = new TestDiagramNode("node2");
//            var testModelRelationship = new TestModelRelationship(node1.ModelNode, node2.ModelNode);
//            var connectorNode1Node2 = new DiagramConnector(testModelRelationship, node1, node2, ConnectorTypes.Dependency);

//            var diagram = Diagram.Empty
//                .AddNode(node1)
//                .AddNode(node2)
//                .AddConnector(connectorNode1Node2);

//            diagram = diagram.UpdateConnector(connectorNode1Node2.WithRoute(TestRoute));

//            var resultingConnectors = diagram.RootLayoutGroup.Connectors;
//            resultingConnectors.Should().HaveCount(1);
//            resultingConnectors.First().Route.Should().BeEquivalentTo(TestRoute);
//        }

//        [Fact]
//        public void UpdateConnector_InNestedLayoutGroup_Works()
//        {
//            var parentNode = new TestDiagramNode("parent");
//            var childNode1 = new TestDiagramNode("child1");
//            var childNode2 = new TestDiagramNode("child2");
//            var testModelRelationship = new TestModelRelationship(childNode1.ModelNode, childNode2.ModelNode);
//            var connectorNode1Node2 = new DiagramConnector(testModelRelationship, childNode1, childNode2, ConnectorTypes.Dependency);

//            var diagram = Diagram.Empty
//                .AddNode(parentNode)
//                .AddNode(childNode1, parentNode.Id)
//                .AddNode(childNode2, parentNode.Id)
//                .AddConnector(connectorNode1Node2);

//            diagram = diagram.UpdateConnector(connectorNode1Node2.WithRoute(TestRoute));

//            var resultingConnectors = diagram.GetNode(parentNode.Id).As<IContainerDiagramNode>().LayoutGroup.Connectors;
//            resultingConnectors.Should().HaveCount(1);
//            resultingConnectors.First().Route.Should().BeEquivalentTo(TestRoute);
//        }

//        [Fact]
//        public void UpdateConnector_InCrossLayoutGroup_Works()
//        {
//            var parentNode = new TestDiagramNode("parent");
//            var childNode = new TestDiagramNode("child");
//            var testModelRelationship = new TestModelRelationship(parentNode.ModelNode, childNode.ModelNode);
//            var connectorParentChild = new DiagramConnector(testModelRelationship, parentNode, childNode, ConnectorTypes.Dependency);

//            var diagram = Diagram.Empty
//                .AddNode(parentNode)
//                .AddNode(childNode, parentNode.Id)
//                .AddConnector(connectorParentChild);

//            diagram = diagram.UpdateConnector(connectorParentChild.WithRoute(TestRoute));

//            var resultingConnectors = diagram.CrossLayoutGroupConnectors;
//            resultingConnectors.Should().HaveCount(1);
//            resultingConnectors.First().Route.Should().BeEquivalentTo(TestRoute);
//        }

//        [Fact]
//        public void RemoveConnector_FromRootLayoutGroup_Works()
//        {
//            var node1 = new TestDiagramNode("node1");
//            var node2 = new TestDiagramNode("node2");
//            var testModelRelationship = new TestModelRelationship(node1.ModelNode, node2.ModelNode);
//            var connectorNode1Node2 = new DiagramConnector(testModelRelationship, node1, node2, ConnectorTypes.Dependency);

//            var diagram = Diagram.Empty
//                .AddNode(node1)
//                .AddNode(node2)
//                .AddConnector(connectorNode1Node2)
//                .RemoveConnector(testModelRelationship.Id);

//            diagram.Connectors.Should().BeEmpty();
//            diagram.RootLayoutGroup.Connectors.Should().BeEmpty();
//            diagram.CrossLayoutGroupConnectors.Should().BeEmpty();
//        }

//        [Fact]
//        public void RemoveConnector_FromNestedLayoutGroup_Works()
//        {
//            var parentNode = new TestDiagramNode("parent");
//            var childNode1 = new TestDiagramNode("child1");
//            var childNode2 = new TestDiagramNode("child2");
//            var testModelRelationship = new TestModelRelationship(childNode1.ModelNode, childNode2.ModelNode);
//            var connectorNode1Node2 = new DiagramConnector(testModelRelationship, childNode1, childNode2, ConnectorTypes.Dependency);

//            var diagram = Diagram.Empty
//                .AddNode(parentNode)
//                .AddNode(childNode1, parentNode.Id)
//                .AddNode(childNode2, parentNode.Id)
//                .AddConnector(connectorNode1Node2)
//                .RemoveConnector(testModelRelationship.Id);

//            diagram.Connectors.Should().BeEmpty();
//            diagram.RootLayoutGroup.Connectors.Should().BeEmpty();
//            diagram.CrossLayoutGroupConnectors.Should().BeEmpty();
//            diagram.GetNode(parentNode.Id).As<IContainerDiagramNode>().LayoutGroup.Connectors.Should().BeEmpty();
//        }

//        [Fact]
//        public void RemoveConnector_FromCrossLayoutGroup_Works()
//        {
//            var parentNode = new TestDiagramNode("parent");
//            var childNode = new TestDiagramNode("child");
//            var testModelRelationship = new TestModelRelationship(parentNode.ModelNode, childNode.ModelNode);
//            var connectorParentChild = new DiagramConnector(testModelRelationship, parentNode, childNode, ConnectorTypes.Dependency);

//            var diagram = Diagram.Empty
//                .AddNode(parentNode)
//                .AddNode(childNode, parentNode.Id)
//                .AddConnector(connectorParentChild)
//                .RemoveConnector(testModelRelationship.Id);

//            diagram.Connectors.Should().BeEmpty();
//            diagram.RootLayoutGroup.Connectors.Should().BeEmpty();
//            diagram.CrossLayoutGroupConnectors.Should().BeEmpty();
//            diagram.GetNode(parentNode.Id).As<IContainerDiagramNode>().LayoutGroup.Connectors.Should().BeEmpty();
//        }

//        [Fact]
//        public void PathExists_WorksInRootLayoutGroup()
//        {
//            var node1 = new TestDiagramNode("node1");
//            var node2 = new TestDiagramNode("node2");
//            var testModelRelationship = new TestModelRelationship(node1.ModelNode, node2.ModelNode);
//            var connectorNode1Node2 = new DiagramConnector(testModelRelationship, node1, node2, ConnectorTypes.Dependency);

//            var diagram = Diagram.Empty
//                .AddNode(node1)
//                .AddNode(node2)
//                .AddConnector(connectorNode1Node2);

//            diagram.PathExists(node1.Id, node2.Id).Should().BeTrue();
//            diagram.PathExists(node2.Id, node1.Id).Should().BeFalse();
//        }

//        [Fact]
//        public void PathExists_WorksInNestedLayoutGroup()
//        {
//            var parentNode = new TestDiagramNode("parent");
//            var childNode1 = new TestDiagramNode("child1");
//            var childNode2 = new TestDiagramNode("child2");
//            var testModelRelationship = new TestModelRelationship(childNode1.ModelNode, childNode2.ModelNode);
//            var connectorNode1Node2 = new DiagramConnector(testModelRelationship, childNode1, childNode2, ConnectorTypes.Dependency);

//            var diagram = Diagram.Empty
//                .AddNode(parentNode)
//                .AddNode(childNode1, parentNode.Id)
//                .AddNode(childNode2, parentNode.Id)
//                .AddConnector(connectorNode1Node2);

//            diagram.PathExists(childNode1.Id, childNode2.Id).Should().BeTrue();
//            diagram.PathExists(childNode2.Id, childNode1.Id).Should().BeFalse();
//        }

//        [Fact]
//        public void PathExists_WorksBetweenLayoutGroups()
//        {
//            var parentNode1 = new TestDiagramNode("parent1");
//            var childNode1 = new TestDiagramNode("child1");
//            var parentNode2 = new TestDiagramNode("parent2");
//            var childNode2 = new TestDiagramNode("child2");
//            var testModelRelationship = new TestModelRelationship(childNode1.ModelNode, childNode2.ModelNode);
//            var connectorChild1Child2 = new DiagramConnector(testModelRelationship, childNode1, childNode2, ConnectorTypes.Dependency);

//            var diagram = Diagram.Empty
//                .AddNode(parentNode1)
//                .AddNode(childNode1, parentNode1.Id)
//                .AddNode(parentNode2)
//                .AddNode(childNode2, parentNode2.Id)
//                .AddConnector(connectorChild1Child2);

//            diagram.PathExists(childNode1.Id, childNode2.Id).Should().BeTrue();
//            diagram.PathExists(childNode2.Id, childNode1.Id).Should().BeFalse();
//            diagram.PathExists(parentNode1.Id, parentNode2.Id).Should().BeFalse();
//        }
//    }
//}