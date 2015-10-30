using System.Collections.Generic;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Provides a read-only view of a positioning graph.
    /// </summary>
    internal interface IReadOnlyPositioningGraph : IBidirectionalGraph<PositioningVertexBase, PositioningEdge>
    {
        IEnumerable<IReadOnlyPositioningVertexLayer> Layers { get; } 
    }
}