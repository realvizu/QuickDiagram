using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UnitTests.Modeling;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation
{
    public class ImmutableDiagramTests
    {
        [NotNull] private readonly ModelBuilder _modelBuilder;

        public ImmutableDiagramTests()
        {
            _modelBuilder = new ModelBuilder();
        }

        [Fact]
        public void PathExists_WorksInRootLayoutGroup()
        {
            var model = _modelBuilder.AddNodes("A", "B").AddRelationships("A->B").Model;
            var node1 = _modelBuilder.GetNode("A");
            var node2 = _modelBuilder.GetNode("B");

            var diagram = new DiagramBuilder(model).AddAllModelItems().GetDiagram();

            diagram.PathExists(node1.Id, node2.Id, ModelRelationshipStereotype.Default).Should().BeTrue();
            diagram.PathExists(node2.Id, node1.Id, ModelRelationshipStereotype.Default).Should().BeFalse();
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

            var diagram = new DiagramBuilder(model).AddAllModelItems().GetDiagram();

            diagram.PathExists(childNode1.Id, childNode2.Id, ModelRelationshipStereotype.Default).Should().BeTrue();
            diagram.PathExists(childNode2.Id, childNode1.Id, ModelRelationshipStereotype.Default).Should().BeFalse();
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

            var diagram = new DiagramBuilder(model).AddAllModelItems().GetDiagram();

            diagram.PathExists(childNode1.Id, childNode2.Id, ModelRelationshipStereotype.Default).Should().BeTrue();
            diagram.PathExists(childNode2.Id, childNode1.Id, ModelRelationshipStereotype.Default).Should().BeFalse();
            diagram.PathExists(parentNode1.Id, parentNode2.Id, ModelRelationshipStereotype.Default).Should().BeFalse();
        }
    }
}