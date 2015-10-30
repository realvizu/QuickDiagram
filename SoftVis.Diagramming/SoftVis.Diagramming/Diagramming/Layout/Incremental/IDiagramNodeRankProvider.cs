namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Provides diagram node ranking information.
    /// </summary>
    internal interface IDiagramNodeRankProvider
    {
        int GetRank(DiagramNode diagramNode);
        int GetRankSpan(DiagramConnector diagramConnector);
    }
}
