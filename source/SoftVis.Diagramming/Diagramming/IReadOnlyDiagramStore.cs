using System;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// Read-only view of a diagram store. 
    /// Publishes diagrams change events.
    /// </summary>
    public interface IReadOnlyDiagramStore
    {
        IDiagram CurrentDiagram { get; }

        event Action<DiagramEventBase> DiagramChanged;

        ConnectorType GetConnectorType(ModelRelationshipStereotype modelRelationshipStereotype);
    }
}