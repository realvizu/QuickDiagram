using System;
using System.Collections.Generic;
using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative
{
    /// <summary>
    /// Provides a read-only view of a proper layered graph used for layout calculation.
    /// </summary>
    internal interface IReadOnlyQuasiProperLayoutGraph : IReadOnlyLayeredGraph<LayoutVertexBase, GeneralLayoutEdge>
    {
        GeneralLayoutEdge GetInEdge(DummyLayoutVertex dummyVertex);
        GeneralLayoutEdge GetOutEdge(DummyLayoutVertex dummyVertex);

        LayoutVertexBase GetPrimaryParent(LayoutVertexBase vertex);
        bool HasPrimaryChildren(LayoutVertexBase vertex);
        IEnumerable<LayoutVertexBase> GetPrimaryChildren(LayoutVertexBase vertex);
        IEnumerable<LayoutVertexBase> GetPlacedPrimaryChildren(LayoutVertexBase vertex);
        IEnumerable<LayoutVertexBase> GetPrimarySiblings(LayoutVertexBase vertex);
        IEnumerable<LayoutVertexBase> GetPrimaryDescendants(LayoutVertexBase vertex);
        IEnumerable<LayoutVertexBase> GetVertexAndPrimaryDescendants(LayoutVertexBase vertex);
        bool IsPlacedPrimarySiblingOf(LayoutVertexBase vertex1, LayoutVertexBase vertex2);

        void ExecuteOnPrimaryDescendantVertices(LayoutVertexBase vertex, Action<LayoutVertexBase> actionOnVertex);
    }
}