using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative
{
    /// <summary>
    /// A read-only view of the layout vertex layers.
    /// </summary>
    internal interface IReadOnlyLayoutVertexLayers : IEnumerable<IReadOnlyLayoutVertexLayer>
    {
        int? GetLayerIndex(LayoutVertexBase vertex);
        int GetLayerIndexOrThrow(LayoutVertexBase vertex);
        RelativeLocation? GetLocation(LayoutVertexBase vertex);
        RelativeLocation GetLocationOrThrow(LayoutVertexBase vertex);

        IReadOnlyLayoutVertexLayer GetLayer(LayoutVertexBase vertex);
        IReadOnlyLayoutVertexLayer GetLayer(int layerIndex);
        int GetIndexInLayer(LayoutVertexBase vertex);
        LayoutVertexBase GetPreviousInLayer(LayoutVertexBase vertex);
        LayoutVertexBase GetNextInLayer(LayoutVertexBase vertex);
        IEnumerable<LayoutVertexBase> GetOtherPlacedVerticesInLayer(LayoutVertexBase vertex);

        // TODO: ???
        void UpdateLayerVerticalPositions(double verticalGap);
    }
}
