namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative
{
    internal interface IReadOnlyRelativeLayout
    {
        IReadOnlyHighLevelLayoutGraph HighLevelLayoutGraph { get; }
        IReadOnlyLowLevelLayoutGraph LowLevelLayoutGraph { get; }
        IReadOnlyLayoutVertexLayers LayoutVertexLayers { get; }

        bool HasPlacedPrimarySiblingsInSameLayer(LayoutVertexBase vertex);
        LayoutVertexBase GetPreviousPlacedPrimarySiblingInSameLayer(LayoutVertexBase vertex);
        LayoutVertexBase GetNextPlacedPrimarySiblingInSameLayer(LayoutVertexBase vertex);
    }
}
