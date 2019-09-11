using System.Collections.Generic;
using Codartis.SoftVis.Graphs.Layered;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama.Relative
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
        IEnumerable<LayoutVertexBase> GetPrimarySiblings(LayoutVertexBase vertex);
    }
}