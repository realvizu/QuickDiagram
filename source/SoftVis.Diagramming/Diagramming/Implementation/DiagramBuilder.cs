using Codartis.SoftVis.Modeling2;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// Implements diagram building and modification logic.
    /// </summary>
    public class DiagramBuilder
    {
        public virtual ConnectorType GetConnectorType(IModelRelationship modelRelationship) => ConnectorTypes.Generalization;
    }
}
