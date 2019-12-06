using System;
using System.Collections.Generic;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Defines diagram UI operations and event hooks.
    /// </summary>
    public interface IDiagramUiService
    {
        IDiagramUi DiagramUi { get; }

        void ZoomToDiagram();
        void FollowDiagramNode(ModelNodeId nodeId);
        void FollowDiagramNodes(IReadOnlyCollection<ModelNodeId> nodeIds);
        void KeepDiagramCentered();

        event ShowModelItemsEventHandler ShowModelItemsRequested;
        event Action<IDiagramNode, Size2D> DiagramNodeHeaderSizeChanged;
        event Action<IDiagramNode> DiagramNodeInvoked;
        event Action<IDiagramNode> RemoveDiagramNodeRequested;
    }
}
