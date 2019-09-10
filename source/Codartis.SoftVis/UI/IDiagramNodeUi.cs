using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Abstraction for the UI of a diagram node.
    /// </summary>
    public interface IDiagramNodeUi : IDiagramShapeUi
    {
        IDiagramNode DiagramNode { get; }

        event Action<IDiagramNode, Size2D> SizeChanged;
        event Action<IDiagramNode> RemoveRequested;
        //event RelatedNodeMiniButtonEventHandler ShowRelatedNodesRequested;
        //event RelatedNodeMiniButtonEventHandler RelatedNodeSelectorRequested;

        void AddChildNode(IDiagramNodeUi childNode);
    }
}