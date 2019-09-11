using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama.Relative
{
    /// <summary>
    /// Provides a read-only view of a layout vertex layer.
    /// </summary>
    internal interface IReadOnlyLayoutVertexLayer : IEnumerable<LayoutVertexBase>
    {
        int LayerIndex { get; }

        int Count { get; }
        LayoutVertexBase this[int i] { get; }
        int IndexOf(LayoutVertexBase vertex);
    }
}
