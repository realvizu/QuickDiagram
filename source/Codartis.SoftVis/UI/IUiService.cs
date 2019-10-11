using System;
using System.Collections.Generic;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Defines UI-related operations and event hooks.
    /// </summary>
    public interface IUiService
    {
        void ZoomToDiagram();
        void FollowDiagramNode(ModelNodeId nodeId);
        void FollowDiagramNodes(IReadOnlyList<ModelNodeId> nodeIds);
        void KeepDiagramCentered();

        event ShowModelItemsEventHandler ShowModelItemsRequested;
        event Action<IDiagramNode, Size2D> DiagramNodeSizeChanged;
        event Action<IDiagramNode, Size2D> DiagramNodePayloadAreaSizeChanged;
        event Action<IDiagramNode> DiagramNodeInvoked;
        event Action<IDiagramNode> RemoveDiagramNodeRequested;
    }
}
