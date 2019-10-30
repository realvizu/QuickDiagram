using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.Diagramming.Definition
{
    public interface IConnectorTypeResolver
    {
        ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype);
    }
}