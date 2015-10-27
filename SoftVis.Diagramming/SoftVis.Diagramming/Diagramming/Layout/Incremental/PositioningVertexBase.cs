using System;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Abstract base class for vertices used in a positioning graph. 
    /// </summary>
    /// <remarks>
    /// <para>LayoutVertices can float which indicates that the edge does not take part in layout calculations.</para>
    /// <para>LayoutVertices can be compared to each other.</para>
    /// </remarks>
    internal abstract class PositioningVertexBase : IRect, IComparable<PositioningVertexBase>
    {
        public Point2D Center { get; set; }
        public int LayerIndex { get; set; }
        public bool IsFloating { get; set; }

        protected PositioningVertexBase(int layerIndex, bool isFloating)
        {
            LayerIndex = layerIndex;
            IsFloating = isFloating;
        }

        public abstract int Priority { get; }
        public abstract double Width { get; }
        public abstract double Height { get; }
        public abstract Size2D Size { get; }
        public Rect2D Rect => new Rect2D(Center.X - Width / 2, Center.Y - Height / 2, Center.X + Width / 2, Center.Y + Height / 2);
        public double Left => Rect.Left;
        public double Right => Rect.Right;
        public double Top => Rect.Top;
        public double Bottom => Rect.Bottom;

        public abstract int CompareTo(PositioningVertexBase other);

        public bool Precedes(PositioningVertexBase otherVertex) => otherVertex != null && CompareTo(otherVertex) < 0;
    }
}
