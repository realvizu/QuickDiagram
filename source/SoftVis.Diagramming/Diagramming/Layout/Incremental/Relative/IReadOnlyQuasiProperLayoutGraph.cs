using System.Collections.Generic;
using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative
{
    /// <summary>
    /// Provides a read-only view of a proper layered graph used for layout calculation.
    /// </summary>
    public interface IReadOnlyQuasiProperLayoutGraph : IReadOnlyLayeredGraph<LayoutVertexBase, GeneralLayoutEdge>
    {
        GeneralLayoutEdge GetInEdge(DummyLayoutVertex dummyVertex);
        GeneralLayoutEdge GetOutEdge(DummyLayoutVertex dummyVertex);

        LayoutVertexBase GetPrimaryParent(LayoutVertexBase vertex);
        bool HasPrimaryChildren(LayoutVertexBase vertex);
        IEnumerable<LayoutVertexBase> GetPrimaryChildren(LayoutVertexBase vertex);
        IEnumerable<LayoutVertexBase> GetPrimarySiblings(LayoutVertexBase vertex);
    }
}