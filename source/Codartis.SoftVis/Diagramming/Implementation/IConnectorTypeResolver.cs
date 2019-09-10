using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    public interface IConnectorTypeResolver
    {
        ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype);
    }
}