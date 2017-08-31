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
        void ZoomToDiagram();
        void FollowDiagramNode(IDiagramNode diagramNode);
        void FollowDiagramNodes(IReadOnlyList<IDiagramNode> diagramNodes);
        void KeepDiagramCentered();

        event ShowModelItemsEventHandler ShowModelItemsRequested;
        event Action<IDiagramNode, Size2D> DiagramNodeSizeChanged;
        event Action<IDiagramNode> DiagramNodeInvoked;
        event Action<IDiagramNode> RemoveDiagramNodeRequested;
    }
}
