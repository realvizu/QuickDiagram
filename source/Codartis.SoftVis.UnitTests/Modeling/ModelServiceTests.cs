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
        
        [NotNull]
        private static IModelService CreateModelService(params IModelRuleProvider[] modelRuleProviders) => new ModelService(modelRuleProviders);
    }
}