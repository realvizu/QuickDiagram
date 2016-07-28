using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    internal class RoslynBasedConnectorTypeResolver : IConnectorTypeResolver
    {
        public ConnectorType GetConnectorType(IModelRelationship modelRelationship)
        {
            return modelRelationship.Stereotype == ModelRelationshipStereotypes.Implementation
                ? RoslynBasedConnectorTypes.Implementation
                : ConnectorTypes.Generalization;
        }
    }
}
