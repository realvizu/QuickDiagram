using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.Incremental
{
    /// <summary>
    /// A set of layout vertices that belong to the same horizontal layer.
    /// </summary>
    internal class Layer : HashSet<LayoutVertex>
    {
        public int Index { get; }
        public double Top { get; set; }

        internal Layer(int index)
        {
            Index = index;
        }

        public double Height => Count == 0 ? 0 : this.Max(i => i.Height);
        public double Bottom => Top + Height;
        public double CenterY => Top + Height / 2;
        public Rect2D Rect => this.Where(i => !i.IsFloating).Select(i => i.Rect).Union();
    }
}
