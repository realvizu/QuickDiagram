using Codartis.SoftVis.Modeling2;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// Implements diagram building and modification logic.
    /// </summary>
    public abstract class DiagramBuilder
    {
        public abstract ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype);
    }
}
