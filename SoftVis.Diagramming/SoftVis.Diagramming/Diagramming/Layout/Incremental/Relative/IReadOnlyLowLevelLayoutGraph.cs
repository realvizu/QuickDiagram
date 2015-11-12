using System;
using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative
{
    /// <summary>
    /// Provides a read-only view of a low level layout graph.
    /// </summary>
    internal interface IReadOnlyLowLevelLayoutGraph : IReadOnlyLayoutGraph<LayoutVertexBase, LayoutEdge>
    {
        LayoutEdge GetInEdge(DummyLayoutVertex dummyVertex);
        LayoutEdge GetOutEdge(DummyLayoutVertex dummyVertex);

        LayoutVertexBase GetPrimaryParent(LayoutVertexBase vertex);
        bool HasPrimaryChildren(LayoutVertexBase vertex);
        IEnumerable<LayoutVertexBase> GetPrimaryChildren(LayoutVertexBase vertex);
        IEnumerable<LayoutVertexBase> GetPlacedPrimaryChildren(LayoutVertexBase vertex);
        IEnumerable<LayoutVertexBase> GetPrimarySiblings(LayoutVertexBase vertex);
        bool IsPlacedPrimarySiblingOf(LayoutVertexBase vertex1, LayoutVertexBase vertex2);

        void ExecuteOnPrimaryDescendantVertices(LayoutVertexBase vertex, Action<LayoutVertexBase> actionOnVertex);
    }
}