using System;

namespace Codartis.SoftVis.Diagramming.Definition
{
    public interface IDiagramEventSource
    {
        event Action<DiagramChangedEvent> DiagramChanged;
    }
}