using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    internal class RoslynBasedConnectorTypeResolver : DefaultConnectorTypeResolver
    {
        public override ConnectorType GetConnectorType(IModelRelationship modelRelationship)
        {
            return modelRelationship.Stereotype == RoslynBasedModelRelationshipStereotype.Implementation
                ? RoslynBasedConnectorTypes.Implementation
                : base.GetConnectorType(modelRelationship);
        }
    }
}
