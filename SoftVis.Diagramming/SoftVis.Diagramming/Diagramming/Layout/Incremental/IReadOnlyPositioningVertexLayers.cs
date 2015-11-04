using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// A read-only view of the positioning vertex layers.
    /// </summary>
    internal interface IReadOnlyPositioningVertexLayers : IEnumerable<IReadOnlyPositioningVertexLayer>
    {
        IReadOnlyPositioningVertexLayer GetLayer(PositioningVertexBase vertex);
        int GetLayerIndex(PositioningVertexBase vertex);
        int GetIndexInLayer(PositioningVertexBase vertex);

        PositioningVertexBase GetPreviousInLayer(PositioningVertexBase vertex);
        PositioningVertexBase GetNextInLayer(PositioningVertexBase vertex);
        IEnumerable<PositioningVertexBase> GetOtherPlacedVerticesInLayer(PositioningVertexBase vertex);

        bool HasPlacedPrimarySiblingsInSameLayer(PositioningVertexBase vertex);
        IEnumerable<PositioningVertexBase> GetPlacedPrimarySiblingsInSameLayer(PositioningVertexBase vertex);
        PositioningVertexBase GetPreviousPlacedPrimarySiblingInSameLayer(PositioningVertexBase vertex);
        PositioningVertexBase GetNextPlacedPrimarySiblingInSameLayer(PositioningVertexBase vertex);
    }
}
