using System;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.Incremental
{
    internal class LayoutVertex : IComparable<LayoutVertex>
    {
        private Point2D _center;
        public IRect OriginalVertex { get; }
        public int? Rank { get; set; }

        public event EventHandler<Point2D> CenterChanged;

        private LayoutVertex(IRect originalVertex)
        {
            OriginalVertex = originalVertex;
        }

        public static LayoutVertex Create(IRect originalVertex)
        {
            return new LayoutVertex(originalVertex);
        }

        public static LayoutVertex CreateDummy()
        {
            return new LayoutVertex(null);
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
                if (_center == value)
                    return;

                _center = value;
                CenterChanged?.Invoke(this, value);
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
