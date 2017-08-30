using System;
using System.Collections.Generic;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Defines UI-related operations and event hooks.
    /// </summary>
    public interface IUiService
    {
        void ZoomToContent();
        void FollowDiagramNodes(IReadOnlyList<IDiagramNode> diagramNodes);

        event ShowModelItemsEventHandler ShowModelItemsRequested;
        event Action<IDiagramNode, Size2D> DiagramNodeSizeChanged;
        event Action<IDiagramNode> DiagramNodeInvoked;
        event Action<IDiagramNode> RemoveDiagramNodeRequested;
    }
}
