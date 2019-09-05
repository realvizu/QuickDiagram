using System;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Modeling.Definition.Events;
using Codartis.SoftVis.Modeling.Implementation;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Codartis.SoftVis.UnitTests.Modeling
{
    public class ModelServiceTests
    {
        [Fact]
        public void Create_Works()
        {
            var modelService = CreateModelService();
            modelService.LatestModel.Should().Be(Model.Empty);
        }

        [Fact]
        public void AddNode_ReplacesTheModel()
        {
            var modelService = CreateModelService();
            var modelBeforeMutation = modelService.LatestModel;

            modelService.AddNode(CreateModelNode("Node1"));

            modelService.LatestModel.Should().NotBeSameAs(modelBeforeMutation);
        }

        [Fact]
        public void AddNode_Works()
        {
            var modelService = CreateModelService();

            var node1 = CreateModelNode("Node1");
            modelService.AddNode(node1);

            modelService.LatestModel.Nodes.Should().BeEquivalentTo(node1);
        }

        [Fact]
        public void AddNode_PublishesEvent()
        {
            var modelService = CreateModelService();

            using (var monitoredSubject = modelService.Monitor())
            {
                var node1 = CreateModelNode("Node1");
                modelService.AddNode(node1);

                monitoredSubject.Should().Raise(nameof(IModelService.ModelChanged))
                    .WithArgs<ModelNodeAddedEvent>(args => args.NewModel == modelService.LatestModel && args.AddedNode == node1);
            }
        }

        [Fact]
        public void AddNodeWithParent_Works()
        {
            var modelService = CreateModelService();

            var parent = CreateModelNode("Parent");
            modelService.AddNode(parent);
            var child = CreateModelNode("Child");
            modelService.AddNode(child, parent.Id);

            modelService.LatestModel.Nodes.Should().BeEquivalentTo(parent, child);
            modelService.LatestModel.GetRelatedNodes(child.Id, CommonDirectedModelRelationshipTypes.Container).Should().BeEquivalentTo(parent);
            modelService.LatestModel.GetRelatedNodes(parent.Id, CommonDirectedModelRelationshipTypes.Contained).Should().BeEquivalentTo(child);
        }

        [Fact]
        public void UpdateNode_Works()
        {
            var modelService = CreateModelService();

            var node1 = CreateModelNode("Node1");
            modelService.AddNode(node1);
            var node1A = node1.WithName("Node1A");
            modelService.UpdateNode(node1A);

            modelService.LatestModel.Nodes.Should().BeEquivalentTo(node1A);
        }

        [Fact]
        public void UpdateNode_NonExistingId_Throws()
        {
            var modelService = CreateModelService();

            var node1 = CreateModelNode("Node1");
            Action a = () => modelService.UpdateNode(node1);

            a.Should().Throw<InvalidOperationException>().Where(i => i.Message.Contains("not found"));
        }

        [Fact]
        public void AddRelationship_Works()
        {
            var modelService = CreateModelService();

            var node1 = CreateModelNode("Node1");
            var node2 = CreateModelNode("Node2");
            var relationship = new ModelRelationship(ModelRelationshipId.Create(), node1.Id, node2.Id, ModelRelationshipStereotype.Default);
            modelService.AddNode(node1);
            modelService.AddNode(node2);
            modelService.AddRelationship(relationship);

            modelService.LatestModel.Relationships.Should().BeEquivalentTo(relationship);
        }

        [Fact]
        public void AddRelationship_InvalidByModelProvider_Throws()
        {
            var modelService = CreateModelService(new AllInvalidModelRuleProvider());

            var node1 = CreateModelNode("Node1");
            var node2 = CreateModelNode("Node2");
            var relationship = new ModelRelationship(ModelRelationshipId.Create(), node1.Id, node2.Id, ModelRelationshipStereotype.Default);
            modelService.AddNode(node1);
            modelService.AddNode(node2);
            Action a = () => modelService.AddRelationship(relationship);

            a.Should().Throw<ArgumentException>().Where(i => i.Message.Contains("invalid"));
        }

        [NotNull]
        private static IModelService CreateModelService(params IModelRuleProvider[] modelRuleProviders) => new ModelService(modelRuleProviders);

        [NotNull]
        private static IModelNode CreateModelNode([NotNull] string name)
            => new ModelNode(ModelNodeId.Create(), name, ModelNodeStereotype.Default);

        private sealed class AllInvalidModelRuleProvider : IModelRuleProvider
        {
            public bool IsRelationshipStereotypeValid(ModelRelationshipStereotype modelRelationshipStereotype, IModelNode source, IModelNode target)
            {
                return false;
            }
        }
    }
}