using System;
using Codartis.SoftVis.Diagramming.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Abstraction for the UI of a diagram node.
    /// </summary>
    public interface IDiagramNodeUi : IDiagramShapeUi
    {
        [NotNull] IDiagramNode DiagramNode { get; }
        [NotNull] IDiagramNodeHeaderUi Header { get; }

        event Action<IDiagramNode> RemoveRequested;

        //event RelatedNodeMiniButtonEventHandler ShowRelatedNodesRequested;
        //event RelatedNodeMiniButtonEventHandler RelatedNodeSelectorRequested;
    }
}