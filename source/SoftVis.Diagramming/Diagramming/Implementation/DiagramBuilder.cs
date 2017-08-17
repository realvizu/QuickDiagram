using Codartis.SoftVis.Modeling;

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
