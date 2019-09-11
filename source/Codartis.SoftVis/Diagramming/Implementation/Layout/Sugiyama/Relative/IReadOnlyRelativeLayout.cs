namespace Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama.Relative
{
    /// <summary>
    /// Provides a read-only view about all the information of the relative layout.
    /// </summary>
    internal interface IReadOnlyRelativeLayout
    {
        IReadOnlyLayeredLayoutGraph LayeredLayoutGraph { get; }
        IReadOnlyQuasiProperLayoutGraph ProperLayeredLayoutGraph { get; }
        IReadOnlyLayoutVertexLayers LayoutVertexLayers { get; }
    }
}
