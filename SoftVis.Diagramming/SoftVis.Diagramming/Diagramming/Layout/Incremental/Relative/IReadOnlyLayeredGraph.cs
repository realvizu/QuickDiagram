namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative
{
    /// <summary>
    /// Provides a read-only view of a layered graph.
    /// </summary>
    internal interface IReadOnlyLayeredGraph : IReadOnlyLayoutGraph<DiagramNodeLayoutVertex, LayoutPath>
    {
        int GetLayerIndex(DiagramNodeLayoutVertex vertex);
    }
}