using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// The default implementation of a connector type resolver.
    /// </summary>
    public class DefaultConnectorTypeResolver : IConnectorTypeResolver
    {
        public virtual ConnectorType GetConnectorType(IModelRelationship modelRelationship)
        {
            return ConnectorTypes.Generalization;
        }
    }
}
