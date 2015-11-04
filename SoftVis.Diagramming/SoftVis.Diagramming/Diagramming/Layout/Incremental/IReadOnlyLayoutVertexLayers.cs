using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// A read-only view of the layout vertex layers.
    /// </summary>
    internal interface IReadOnlyLayoutVertexLayers : IEnumerable<IReadOnlyLayoutVertexLayer>
    {
        IReadOnlyLayoutVertexLayer GetLayer(LayoutVertexBase vertex);
        int GetLayerIndex(LayoutVertexBase vertex);
        int GetIndexInLayer(LayoutVertexBase vertex);

        LayoutVertexBase GetPreviousInLayer(LayoutVertexBase vertex);
        LayoutVertexBase GetNextInLayer(LayoutVertexBase vertex);
        IEnumerable<LayoutVertexBase> GetOtherPlacedVerticesInLayer(LayoutVertexBase vertex);

        bool HasPlacedPrimarySiblingsInSameLayer(LayoutVertexBase vertex);
        IEnumerable<LayoutVertexBase> GetPlacedPrimarySiblingsInSameLayer(LayoutVertexBase vertex);
        LayoutVertexBase GetPreviousPlacedPrimarySiblingInSameLayer(LayoutVertexBase vertex);
        LayoutVertexBase GetNextPlacedPrimarySiblingInSameLayer(LayoutVertexBase vertex);
    }
}
