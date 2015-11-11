using System.Collections.Generic;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative
{
    /// <summary>
    /// Provides a read-only view of a layout vertex layer.
    /// </summary>
    internal interface IReadOnlyLayoutVertexLayer : IEnumerable<LayoutVertexBase>,
        IFilterableToNonFloating<IReadOnlyLayoutVertexLayer, LayoutVertexBase>
    {
        int LayerIndex { get; }

        double Top { get; set; }
        double Bottom { get; }
        double Height { get; }
        double CenterY { get; }
        Rect2D Rect { get; }

        int Count { get; }
        LayoutVertexBase this[int i] { get; }
        int IndexOf(LayoutVertexBase vertex);
        LayoutVertexBase GetPrevious(LayoutVertexBase vertex);
        LayoutVertexBase GetNext(LayoutVertexBase vertex);
    }
}
