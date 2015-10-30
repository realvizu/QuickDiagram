using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// A graph that assigns a rank to all vertices so for all edges Source.Rank > Target.Rank.
    /// That is, all edges point "upwards".
    /// TODO: Also ensures that the graph is acyclic by reversing edges that cause a cycle.
    /// </summary>
    internal class LayeringGraph : BidirectionalGraph<LayeringVertex, LayeringEdge>
    {
        public LayeringGraph()
            : base(allowParallelEdges: false)
        {
        }
    }
}
