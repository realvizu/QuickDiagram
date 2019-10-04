using Codartis.SoftVis.Diagramming.Implementation.Layout;
using Codartis.SoftVis.UnitTests.Modeling;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout
{
    public class DiagramLayoutStructureTests
    {
        [NotNull] private readonly ModelBuilder _modelBuilder;

        public DiagramLayoutStructureTests()
        {
            _modelBuilder = new ModelBuilder();
        }

        [Fact]
        public void Create_OnlyNodes_Works()
        {
            var model = _modelBuilder
                .AddNodes("parent")
                .AddChildNodes("parent", "child")
                .Model;
            var parentNode = _modelBuilder.GetNode("parent");
            var childNode = _modelBuilder.GetNode("child");

            var diagramBuilder = new DiagramBuilder(model).AddAllModelNodes();

            var layoutStructure = new DiagramLayoutStructure(diagramBuilder.Diagram);
            layoutStructure.RootLayoutGroup.Nodes.ShouldBeEquivalentById(parentNode.Id);
            layoutStructure.TryGetLayoutGroupByNodeId(parentNode.Id).Value.Nodes.ShouldBeEquivalentById(childNode.Id);
        }

        [Fact]
        public void Create_ConnectorInRootLayoutGroup_Works()
        {
            var model = _modelBuilder
                .AddNodes("A", "B")
                .AddRelationships("A->B")
                .Model;
            var relationship = _modelBuilder.GetRelationship("A->B");

            var diagramBuilder = new DiagramBuilder(model).AddAllModelNodes().AddAllModelRelationships();

            var layoutStructure = new DiagramLayoutStructure(diagramBuilder.Diagram);
            layoutStructure.RootLayoutGroup.Connectors.ShouldBeEquivalentById(relationship.Id);
            layoutStructure.CrossLayoutGroupConnectors.Should().BeEmpty();
        }

        [Fact]
        public void Create_ConnectorInNestedLayoutGroup_Works()
        {
            var model = _modelBuilder
                .AddNodes("parent")
                .AddChildNodes("parent", "child1", "child2")
                .AddRelationships("child1->child2")
                .Model;
            var parentNode = _modelBuilder.GetNode("parent");
            var relationship = _modelBuilder.GetRelationship("child1->child2");

            var diagramBuilder = new DiagramBuilder(model).AddAllModelNodes().AddAllModelRelationships();

            var layoutStructure = new DiagramLayoutStructure(diagramBuilder.Diagram);
            layoutStructure.RootLayoutGroup.Connectors.Should().BeEmpty();
            layoutStructure.CrossLayoutGroupConnectors.Should().BeEmpty();
            layoutStructure.TryGetLayoutGroupByNodeId(parentNode.Id).Value.Connectors.ShouldBeEquivalentById(relationship.Id);
        }

        [Fact]
        public void Create_ConnectorInCrossLayoutGroup_Works()
        {
            var model = _modelBuilder
                .AddNodes("parent1", "parent2")
                .AddChildNodes("parent1", "child1")
                .AddChildNodes("parent2", "child2")
                .AddRelationships("child1->child2")
                .Model;
            var parentNode1 = _modelBuilder.GetNode("parent1");
            var parentNode2 = _modelBuilder.GetNode("parent2");
            var relationship = _modelBuilder.GetRelationship("child1->child2");

            var diagramBuilder = new DiagramBuilder(model).AddAllModelNodes().AddAllModelRelationships();

            var layoutStructure = new DiagramLayoutStructure(diagramBuilder.Diagram);
            layoutStructure.RootLayoutGroup.Connectors.Should().BeEmpty();
            layoutStructure.CrossLayoutGroupConnectors.ShouldBeEquivalentById(relationship.Id);
            layoutStructure.TryGetLayoutGroupByNodeId(parentNode1.Id).Value.Connectors.Should().BeEmpty();
            layoutStructure.TryGetLayoutGroupByNodeId(parentNode2.Id).Value.Connectors.Should().BeEmpty();
        }
    }
}