using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.Diagramming.Definition
{
    public interface IConnectorTypeResolver
    {
        /// <summary>
        /// Returns the connector type for a relationship type.
        /// </summary>
        ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype);
    }
}