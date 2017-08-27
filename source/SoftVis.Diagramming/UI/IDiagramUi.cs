using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Defines interaction points for the app logic and the UI.
    /// </summary>
    public interface IDiagramUi
    {
        void ZoomToContent();

        event Action<IDiagramNode, Size2D> DiagramNodeSizeChanged;
        event Action<IDiagramNode> DiagramNodeInvoked;
    }
}
