using System.Collections.Generic;
using Codartis.SoftVis.Modeling;
using IModelRelationship = Codartis.SoftVis.Modeling2.IModelRelationship;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// Implements diagram building and modification logic.
    /// </summary>
    public class DiagramBuilder
    {
        public virtual ConnectorType GetConnectorType(IModelRelationship modelRelationship) => ConnectorTypes.Generalization;

        //public virtual IEnumerable<EntityRelationType>
    }
}
