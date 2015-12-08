using System.Collections.Generic;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// Extensibility point for the customization of a diagram's appearance and behavior.
    /// </summary>
    public interface IDiagramExtensionProvider
    {
        IEnumerable<RelatedEntityMiniButtonDescriptor> GetRelatedEntityMiniButtonDescriptors();

        ConnectorStyle GetConnectorStyle(IModelRelationship modelRelationship);
    }
}
