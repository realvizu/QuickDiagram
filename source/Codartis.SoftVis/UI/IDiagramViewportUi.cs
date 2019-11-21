using System.Collections.Generic;
using Codartis.Util.UI;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Displays the diagram through a pannable and zoomable viewport.
    /// </summary>
    public interface IDiagramViewportUi
    {
        IDiagramShapeUiFactory DiagramShapeUiFactory { get; }
        IDecorationManager<IMiniButton, IDiagramShapeUi> MiniButtonManager { get; }

        IEnumerable<IDiagramNodeUi> DiagramNodeUis { get; }
        IEnumerable<IDiagramConnectorUi> DiagramConnectorUis { get; }
    }
}
