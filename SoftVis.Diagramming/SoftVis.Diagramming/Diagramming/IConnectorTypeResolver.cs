using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// Extensibility point for resolving model relationship types to connector types. 
    /// </summary>
    public interface IConnectorTypeResolver
    {
        ConnectorType GetConnectorType(IModelRelationship modelRelationship);
    }
}
