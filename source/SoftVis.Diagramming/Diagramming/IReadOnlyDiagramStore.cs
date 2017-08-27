using System;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// Read-only view of a diagram store. 
    /// Publishes diagrams change events.
    /// </summary>
    public interface IReadOnlyDiagramStore : IDiagramShapeResolver
    {
        IDiagram CurrentDiagram { get; }

        event Action<DiagramEventBase> DiagramChanged;
    }
}