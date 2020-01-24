namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative
{
    /// <summary>
    /// Provides a read-only view about all the information of the relative layout.
    /// </summary>
    public interface IReadOnlyRelativeLayout
    {
        IReadOnlyLayeredLayoutGraph LayeredLayoutGraph { get; }
        IReadOnlyQuasiProperLayoutGraph ProperLayeredLayoutGraph { get; }
        IReadOnlyLayoutVertexLayers LayoutVertexLayers { get; }
    }
}
