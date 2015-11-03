using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// An edge in the ranking graph.
    /// Corresponds to a DiagramConnector but can be reversed to avoid cycles in the ranking graph.
    /// </summary>
    internal class RankingEdge : Edge<RankingVertex>
    {
        public bool IsReversed { get; set; }

        public RankingEdge(RankingVertex source, RankingVertex target) 
            : base(source, target)
        {
        }

        public int RankSpan => Source.Rank - Target.Rank;

        public override string ToString()
        {
            return $"{Source}->{Target}";
        }
    }
}
