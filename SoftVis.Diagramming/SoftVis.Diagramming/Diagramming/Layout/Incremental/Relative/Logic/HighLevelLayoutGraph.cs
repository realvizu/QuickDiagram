namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic
{
    /// <summary>
    /// A high-level graph for layout calculation. 
    /// Contains vertices for diagram nodes and layout paths (layout edge sequences) for diagram connectors.
    /// </summary>
    internal sealed class HighLevelLayoutGraph : LayoutGraphBase<DiagramNodeLayoutVertex, LayoutPath>,
        IReadOnlyHighLevelLayoutGraph
    {
    }
}
