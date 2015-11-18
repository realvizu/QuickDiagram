using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative
{
    /// <summary>
    /// Provides a read-only view about all the information of the relative layout.
    /// </summary>
    internal interface IReadOnlyRelativeLayout
    {
        IReadOnlyLayeredLayoutGraph LayeredLayoutGraph { get; }
        IReadOnlyQuasiProperLayoutGraph ProperLayeredLayoutGraph { get; }
        IReadOnlyLayoutVertexLayers LayoutVertexLayers { get; }

        IEnumerable<LayoutVertexBase> GetPrimarySiblingsInLayer(LayoutVertexBase vertex, int layerIndex);
        IEnumerable<LayoutVertexBase> GetPrimarySiblingsInSameLayer(LayoutVertexBase vertex);
        IEnumerable<LayoutVertexBase> GetPlacedPrimarySiblingsInLayer(LayoutVertexBase vertex, int layerIndex);
        IEnumerable<LayoutVertexBase> GetPlacedPrimarySiblingsInSameLayer(LayoutVertexBase vertex);
        bool HasPlacedPrimarySiblingsInSameLayer(LayoutVertexBase vertex);
        LayoutVertexBase GetPreviousPlacedPrimarySiblingInSameLayer(LayoutVertexBase vertex);
        LayoutVertexBase GetNextPlacedPrimarySiblingInSameLayer(LayoutVertexBase vertex);
    }
}
