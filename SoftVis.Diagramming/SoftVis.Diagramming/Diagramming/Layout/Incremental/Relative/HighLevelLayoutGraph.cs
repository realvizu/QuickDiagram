namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative
{
    /// <summary>
    /// A high-level graph for layout calculation. 
    /// Contains vertices for diagram nodes and layout paths (layout edge sequences) for diagram connectors.
    /// </summary>
    /// <remarks>
    /// Responsibilities:
    /// <para>TODO</para>
    /// Invariants:
    /// <para>TODO</para>
    /// </remarks>
    internal sealed class HighLevelLayoutGraph : LayoutGraphBase<DiagramNodeLayoutVertex, LayoutPath>,
        IReadOnlyHighLevelLayoutGraph
    {
    }
}
