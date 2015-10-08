using System;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.Incremental
{
    /// <summary>
    /// A vertex used in a LayoutGraph. 
    /// Either a dummy vertex or encapulates a real vertex.
    /// </summary>
    /// <remarks>
    /// <para>LayoutVertices can float which indicates that the edge 
    /// does not take part in layout calculations.</para>
    /// <para>Dummy vertices are used to break long edges spanning more than two layers.
    /// They ensure that adjacent vertices in the graph are always on adjacent layers.</para>
    /// <para>LayoutVertices can be compared to each other.
    /// Comparing to a dummy yields 0 (equal).
    /// Comparing real vertices yields the comparison of thier originals.</para>
    /// </remarks>
    internal class LayoutVertex : IComparable<LayoutVertex>
    {
        private Point2D _center;
        public IRect OriginalVertex { get; }
        public bool IsFloating { get; set; }

        public event EventHandler<MoveEventArgs> CenterChanged;

        private LayoutVertex(IRect originalVertex, bool isFloating)
        {
            OriginalVertex = originalVertex;
            IsFloating = isFloating;

            if (originalVertex != null)
                _center = originalVertex.Center;
        }

        public static LayoutVertex Create(IRect originalVertex, bool isFloating = false)
        {
            return new LayoutVertex(originalVertex, isFloating);
        }

        public static LayoutVertex CreateDummy(bool isFloating = false)
        {
            return new LayoutVertex(null, isFloating);
        }

        public bool IsDummy => OriginalVertex == null;
        public double Width => OriginalVertex?.Width ?? 0d;
        public double Height => OriginalVertex?.Height ?? 0d;
        public Size2D Size => OriginalVertex?.Size ?? Size2D.Empty;
        public Rect2D Rect => new Rect2D(Center.X - Width / 2, Center.Y - Height / 2, Center.X + Width / 2, Center.Y + Height / 2);
        public double Left => Rect.Left;
        public double Right => Rect.Right;
        public double Top => Rect.Top;
        public double Bottom => Rect.Bottom;

        public Point2D Center
        {
            get { return _center; }
            set
            {
                IsFloating = false;

                if (_center == value)
                    return;

                var oldValue = _center;
                _center = value;
                CenterChanged?.Invoke(this, new MoveEventArgs(oldValue, value));
            }
        }

        public bool OverlapsWith(LayoutVertex otherVertex, double marginX)
        {
            return Rect.WithMarginX(marginX).Intersect(otherVertex.Rect) != Rect2D.Empty;
        }

        public int CompareTo(LayoutVertex other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (IsDummy || other.IsDummy)
                return 0;

            return OriginalVertex.CompareTo(other.OriginalVertex);
        }

        public override string ToString()
        {
            return OriginalVertex?.ToString() ?? "(dummy)";
        }
    }
}
