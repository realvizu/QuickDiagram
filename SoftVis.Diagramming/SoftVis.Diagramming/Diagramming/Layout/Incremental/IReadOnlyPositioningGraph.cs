using System;
using System.Collections.Generic;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Provides a read-only view of a positioning graph.
    /// </summary>
    internal interface IReadOnlyPositioningGraph : IBidirectionalGraph<PositioningVertexBase, PositioningEdge>
    {
        PositioningEdge GetInEdge(DummyPositioningVertex dummyVertex);
        PositioningEdge GetOutEdge(DummyPositioningVertex dummyVertex);

        IEnumerable<PositioningVertexBase> GetParents(PositioningVertexBase vertex);
        PositioningVertexBase GetPrimaryParent(PositioningVertexBase vertex);

        IEnumerable<PositioningVertexBase> GetPrimaryChildren(PositioningVertexBase vertex);
        IEnumerable<PositioningVertexBase> GetPrimaryPositionedChildren(PositioningVertexBase vertex);

        IEnumerable<PositioningVertexBase> GetPrimarySiblings(PositioningVertexBase vertex);
        bool IsPlacedPrimarySiblingOf(PositioningVertexBase vertex1, PositioningVertexBase vertex2);

        void ExecuteOnPrimaryDescendantVertices(PositioningVertexBase vertex, Action<PositioningVertexBase> actionOnVertex);
    }
}