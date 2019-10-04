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
    public class ModelServiceTests
    {
        [Fact]
        public void Create_Works()
        {
            var modelService = CreateModelService();
            modelService.LatestModel.Should().BeEquivalentTo(Model.Create());
        }

        [Fact]
        public void AddNode_ReplacesTheModel()
        {
            var modelService = CreateModelService();
            var modelBeforeMutation = modelService.LatestModel;

            modelService.AddNode("Node1", ModelNodeStereotype.Default);

            modelService.LatestModel.Should().NotBeSameAs(modelBeforeMutation);
        }

        [Fact]
        public void AddNode_Works()
        {
            var modelService = CreateModelService();

            var node1 = modelService.AddNode("Node1", ModelNodeStereotype.Default);

            modelService.LatestModel.Nodes.Should().BeEquivalentTo(node1);
        }

        [Fact]
        public void AddNode_PublishesEvent()
        {
            var modelService = CreateModelService();

            using (var monitoredSubject = modelService.Monitor())
            {
                var node1 = modelService.AddNode("Node1", ModelNodeStereotype.Default);

                var modelEvents = monitoredSubject.OccurredEvents.SelectMany(i => i.Parameters).OfType<ModelEvent>().ToList();
                modelEvents.Should().HaveCount(1);

                var modelEvent = modelEvents.First();
                modelEvent.NewModel.Should().BeEquivalentTo(modelService.LatestModel);
                modelEvent.ItemEvents.Should().HaveCount(1);
                modelEvent.ItemEvents.First().Should().BeOfType<ModelNodeAddedEvent>().Which.AddedNode.Id.Should().Be(node1.Id);
            }
        }

        [Fact]
        public void AddNodeWithParent_Works()
        {
            var modelService = CreateModelService();

            var parent = modelService.AddNode("Parent", ModelNodeStereotype.Default);
            var child = modelService.AddNode("Child", ModelNodeStereotype.Default, parentNodeId: parent.Id);

            modelService.LatestModel.Nodes.Should().BeEquivalentTo(parent, child);
            modelService.LatestModel.GetRelatedNodes(child.Id, CommonDirectedModelRelationshipTypes.Container).Should().BeEquivalentTo(parent);
            modelService.LatestModel.GetRelatedNodes(parent.Id, CommonDirectedModelRelationshipTypes.Contained).Should().BeEquivalentTo(child);
        }

        [Fact]
        public void AddRelationship_Works()
        {
            var modelService = CreateModelService();

            var node1 = modelService.AddNode("Node1", ModelNodeStereotype.Default);
            var node2 = modelService.AddNode("Node2", ModelNodeStereotype.Default);
            var relationship = modelService.AddRelationship(node1.Id, node2.Id, ModelRelationshipStereotype.Default);

            modelService.LatestModel.Relationships.Should().BeEquivalentTo(relationship);
        }

        [Fact]
        public void AddRelationship_InvalidByModelProvider_Throws()
        {
            var modelService = CreateModelService(new AllInvalidModelRuleProvider());

            var node1 = modelService.AddNode("Node1", ModelNodeStereotype.Default);
            var node2 = modelService.AddNode("Node2", ModelNodeStereotype.Default);
            Action a = () => modelService.AddRelationship(node1.Id, node2.Id, ModelRelationshipStereotype.Default);

            a.Should().Throw<ArgumentException>().Where(i => i.Message.Contains("invalid"));
        }

        [NotNull]
        private static IModelService CreateModelService(params IModelRuleProvider[] modelRuleProviders) => new ModelService(modelRuleProviders);

        private sealed class AllInvalidModelRuleProvider : IModelRuleProvider
        {
            public bool IsRelationshipStereotypeValid(ModelRelationshipStereotype modelRelationshipStereotype, IModelNode source, IModelNode target)
            {
                return false;
            }
        }
    }
}