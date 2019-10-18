using System;

namespace Codartis.SoftVis.Diagramming.Definition
{
    public interface IDiagramEventSource
    {
        /// <summary>
        /// Raised whenever the diagram changes.
        /// </summary>
        event Action<DiagramEvent> DiagramChanged;

        /// <summary>
        /// Raised after all handlers of <see cref="DiagramChanged"/> has finished.
        /// </summary>
        /// <remarks>
        /// This is a hack for prioritizing change event handlers.
        /// </remarks>
        event Action<DiagramEvent> AfterDiagramChanged;
    }
}