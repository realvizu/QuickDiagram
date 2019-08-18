using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Modeling.Definition.Events;
using Codartis.SoftVis.Modeling.Implementation;
using Codartis.SoftVis.UnitTests.TestSubjects;
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
        public void AddNode_Works()
        {
            var modelService = CreateModelService();

            var node1 = new TestModelNode("Node1");
            modelService.AddNode(node1);

            modelService.Model.Nodes.Should().BeEquivalentTo(node1);
        }

        [Fact]
        public void AddNode_ChangesTheModel()
        {
            var modelService = CreateModelService();
            var modelBeforeMutation = modelService.Model;

            modelService.AddNode(new TestModelNode("Node1"));

            modelService.Model.Should().NotBeSameAs(modelBeforeMutation);
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

        [NotNull]
        private static IModelService CreateModelService()
        {
            return new ModelService(new ModelRelationshipFactory());
        }
    }
}