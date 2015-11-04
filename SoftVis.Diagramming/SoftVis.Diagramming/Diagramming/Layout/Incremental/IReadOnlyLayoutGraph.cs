using System;
using System.Collections.Generic;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Provides a read-only view of a layout graph.
    /// </summary>
    internal interface IReadOnlyLayoutGraph : IBidirectionalGraph<LayoutVertexBase, LayoutEdge>
    {
        LayoutEdge GetInEdge(DummyLayoutVertex dummyVertex);
        LayoutEdge GetOutEdge(DummyLayoutVertex dummyVertex);

        IEnumerable<LayoutVertexBase> GetParents(LayoutVertexBase vertex);
        LayoutVertexBase GetPrimaryParent(LayoutVertexBase vertex);

        IEnumerable<LayoutVertexBase> GetPrimaryChildren(LayoutVertexBase vertex);
        IEnumerable<LayoutVertexBase> GetPrimaryPositionedChildren(LayoutVertexBase vertex);

        IEnumerable<LayoutVertexBase> GetPrimarySiblings(LayoutVertexBase vertex);
        bool IsPlacedPrimarySiblingOf(LayoutVertexBase vertex1, LayoutVertexBase vertex2);

        void ExecuteOnPrimaryDescendantVertices(LayoutVertexBase vertex, Action<LayoutVertexBase> actionOnVertex);
    }
}