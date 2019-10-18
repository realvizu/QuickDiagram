using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Events;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UnitTests.Modeling;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation
{
    public class DiagramServiceTests
    {
        [NotNull] private readonly ModelBuilder _modelBuilder;

        public DiagramServiceTests()
        {
            _modelBuilder = new ModelBuilder();
        }

        [Fact]
        public void AddNode_Works()
        {
            var model = _modelBuilder.AddNodes("A").Model;
            var node = _modelBuilder.GetNode("A");

            var diagramService = CreateDiagramService(model);

            using (var monitoredSubject = ((IDiagramEventSource)diagramService).Monitor())
            {
                diagramService.AddNode(node.Id);

                monitoredSubject.Should().Raise(nameof(IDiagramEventSource.DiagramChanged))
                    .WithArgs<DiagramEvent>(
                        i => i.ShapeEvents.Count() == 1 && ((DiagramNodeAddedEvent)i.ShapeEvents.First()).NewNode.Id == node.Id);
            }

            diagramService.LatestDiagram.Nodes.ShouldBeEquivalentById(node.Id);
        }

        [NotNull]
        private static IDiagramService CreateDiagramService([NotNull] IModel model)
        {
            return new DiagramService(model, new DummyConnectorTypeResolver());
        }

        private sealed class DummyConnectorTypeResolver : IConnectorTypeResolver
        {
            public ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype) => ConnectorTypes.Dependency;
        }
    }
}