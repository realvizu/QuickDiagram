using QuickGraph;

namespace Codartis.SoftVis.Graphs.Immutable
{
    /// <summary>
    /// An edge with an Id that connects two VertexIds.
    /// </summary>
    public struct VertexIdEdge<TVertexId, TEdgeId> : IEdge<TVertexId>
    {
        public TEdgeId  Id { get; }
        public TVertexId Source { get; }
        public TVertexId Target { get; }

        public VertexIdEdge(TEdgeId id, TVertexId source, TVertexId target)
        {
            Id = id;
            Source = source;
            Target = target;
        }
    }
}
