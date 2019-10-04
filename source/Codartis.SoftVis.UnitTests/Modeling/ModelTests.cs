using System;
using System.Linq;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Modeling.Definition.Events;
using Codartis.SoftVis.Modeling.Implementation;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Codartis.SoftVis.UnitTests.Modeling
{
    public class ModelTests
    {
        [Fact]
        public void AddNode_Works()
        {
            var modelEvent = CreateModel().AddNode("Node1", ModelNodeStereotype.Default);

            modelEvent.NewModel.Nodes.Should().HaveCount(1);
            modelEvent.ItemEvents.Should().SatisfyRespectively(
                i => i.Should().BeOfType<ModelNodeAddedEvent>().Which.AddedNode.Name.Should().Be("Node1")
            );
        }

        [Fact]
        public void AddNodeWithParent_Works()
        {
            var modelBuilder = new ModelBuilder().AddNodes("Parent");
            var parent = modelBuilder.GetNode("Parent");

            var modelEvent = modelBuilder.Model.AddNode("Child", ModelNodeStereotype.Default, parentNodeId: parent.Id);

            modelEvent.NewModel.Nodes.Should().HaveCount(2);
            var child = modelEvent.NewModel.Nodes.Single(i => i.Name == "Child");

            modelEvent.ItemEvents.Should().SatisfyRespectively(
                i => i.Should().BeOfType<ModelNodeAddedEvent>().Which.AddedNode.Name.Should().Be("Child"),
                i => i.Should().BeOfType<ModelRelationshipAddedEvent>().Which.AddedRelationship
                    .ShouldMatch(parent.Id, child.Id, ModelRelationshipStereotype.Containment)
            );
        }

        [Fact]
        public void GetRelatedNodes_Works()
        {
            var modelBuilder = new ModelBuilder().AddNodes("Parent").AddChildNodes("Parent", "Child");
            var parent = modelBuilder.GetNode("Parent");
            var child = modelBuilder.GetNode("Child");
            var model = modelBuilder.Model;

            model.GetRelatedNodes(child.Id, CommonDirectedModelRelationshipTypes.Container).Should().BeEquivalentTo(parent);
            model.GetRelatedNodes(parent.Id, CommonDirectedModelRelationshipTypes.Contained).Should().BeEquivalentTo(child);
        }

        [Fact]
        public void AddRelationship_Works()
        {
            var modelBuilder = new ModelBuilder().AddNodes("Node1", "Node2");
            var node1 = modelBuilder.GetNode("Node1");
            var node2 = modelBuilder.GetNode("Node2");

            var modelEvent = modelBuilder.Model.AddRelationship(node1.Id, node2.Id, ModelRelationshipStereotype.Default);

            modelEvent.NewModel.Relationships.Should().HaveCount(1);
            modelEvent.ItemEvents.Should().SatisfyRespectively(
                i => i.Should().BeOfType<ModelRelationshipAddedEvent>().Which.AddedRelationship
                    .ShouldMatch(node1.Id, node2.Id, ModelRelationshipStereotype.Default)
            );
        }

        [Fact]
        public void AddRelationship_InvalidByModelProvider_Throws()
        {
            var modelBuilder = new ModelBuilder(CreateModel(new AllInvalidModelRuleProvider())).AddNodes("Node1", "Node2");
            var node1 = modelBuilder.GetNode("Node1");
            var node2 = modelBuilder.GetNode("Node1");

            Action a = () => modelBuilder.Model.AddRelationship(node1.Id, node2.Id, ModelRelationshipStereotype.Default);

            a.Should().Throw<ArgumentException>().Where(i => i.Message.Contains("invalid"));
        }

        [NotNull]
        private static IModel CreateModel(params IModelRuleProvider[] modelRuleProviders) => Model.Create(modelRuleProviders);

        private sealed class AllInvalidModelRuleProvider : IModelRuleProvider
        {
            public bool IsRelationshipStereotypeValid(ModelRelationshipStereotype modelRelationshipStereotype, IModelNode source, IModelNode target)
            {
                return false;
            }
        }
    }
}