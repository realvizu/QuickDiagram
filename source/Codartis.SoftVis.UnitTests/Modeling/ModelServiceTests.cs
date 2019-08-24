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
            modelService.Model.Should().Be(Model.Empty);
        }

        [Fact]
        public void AddNode_ReplacesTheModel()
        {
            var modelService = CreateModelService();
            var modelBeforeMutation = modelService.Model;

            modelService.AddNode(new TestModelNode("Node1"));
            
            modelService.Model.Should().NotBeSameAs(modelBeforeMutation);
        }

        [Fact]
        public void AddNode_Works()
        {
            var modelService = CreateModelService();

            var node1 = new TestModelNode("Node1");
            modelService.AddNode(node1);

            modelService.Model.Nodes.Should().BeEquivalentTo(node1);
        }

        [Fact]
        public void AddNode_PublishesEvent()
        {
            var modelService = CreateModelService();

            using (var monitoredSubject = modelService.Monitor())
            {
                var node1 = new TestModelNode("Node1");
                modelService.AddNode(node1);

                monitoredSubject.Should().Raise(nameof(IModelService.ModelChanged))
                    .WithArgs<ModelNodeAddedEvent>(args => args.NewModel == modelService.Model && args.AddedNode == node1);
            }
        }

        [Fact]
        public void AddNodeWithParent_Works()
        {
            var modelService = CreateModelService();

            var parent = new TestModelNode("Parent");
            modelService.AddNode(parent);
            var child = new TestModelNode("Child");
            modelService.AddNode(child, parent.Id);

            modelService.Model.Nodes.Should().BeEquivalentTo(parent, child);
            modelService.Model.GetRelatedNodes(child.Id, CommonDirectedModelRelationshipTypes.Container).Should().BeEquivalentTo(parent);
            modelService.Model.GetRelatedNodes(parent.Id, CommonDirectedModelRelationshipTypes.Contained).Should().BeEquivalentTo(child);
        }

        [Fact]
        public void UpdateNode_Works()
        {
            var modelService = CreateModelService();

            var node1 = new TestModelNode("Node1");
            modelService.AddNode(node1);
            var node1A = node1.WithName("Node1A");
            modelService.UpdateNode(node1A);

            modelService.Model.Nodes.Should().BeEquivalentTo(node1A);
        }

        [Fact]
        public void UpdateNode_NonExistingId_Throws()
        {
            var modelService = CreateModelService();

            var node1 = new TestModelNode("Node1");
            Action a = () => modelService.UpdateNode(node1);

            a.Should().Throw<InvalidOperationException>().Where(i => i.Message.Contains("not found"));
        }

        [NotNull]
        private static IModelService CreateModelService() => new ModelService();
    }
}