using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.UnitTests.Modeling;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UnitTests.Diagramming
{
    public static class ModelBuilderExtensions
    {
        [NotNull]
        public static IDiagram ToDiagram([NotNull] this ModelBuilder modelBuilder)
        {
            var model = modelBuilder.Model;
            var diagramBuilder = new DiagramBuilder(model);

            foreach (var modelNode in model.Nodes)
                diagramBuilder.AddNode(modelNode.Id);

            foreach (var modelRelationship in model.Relationships)
                diagramBuilder.AddRelationship(modelRelationship.Id);

            return diagramBuilder.Diagram;
        }
    }
}
