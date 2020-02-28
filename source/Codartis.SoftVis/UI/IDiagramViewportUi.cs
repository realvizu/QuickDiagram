using System.Collections.Generic;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Displays the diagram through a pannable and zoomable viewport.
    /// </summary>
    public interface IDiagramViewportUi
    {
        IDiagramShapeUiFactory DiagramShapeUiFactory { get; }
        IMiniButtonManager MiniButtonManager { get; }
        IEnumerable<IDiagramNodeUi> DiagramNodeUis { get; }
        IEnumerable<IDiagramConnectorUi> DiagramConnectorUis { get; }
    }
}
