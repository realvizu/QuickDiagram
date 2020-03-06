using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.UnitTests.Diagramming
{
    public sealed class DummyModelRelationshipFeatureProvider : IModelRelationshipFeatureProvider
    {
        public bool IsTransitive(ModelRelationshipStereotype stereotype) => true;

        public string GetTransitivityPartitionKey(ModelRelationshipStereotype stereotype) => string.Empty;
    }
}