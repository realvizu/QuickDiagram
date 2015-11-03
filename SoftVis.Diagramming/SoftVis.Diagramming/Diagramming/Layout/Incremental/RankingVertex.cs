namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// A vertex in the ranking graph.
    /// Corresponds to a DiagramNode. 
    /// </summary>
    internal class RankingVertex
    {
        public DiagramNode DiagramNode { get; }
        public int Rank { get; set; }

        public RankingVertex(DiagramNode diagramNode)
        {
            DiagramNode = diagramNode;
        }

        public override string ToString()
        {
            return $"{DiagramNode.Name}";
        }
    }
}
