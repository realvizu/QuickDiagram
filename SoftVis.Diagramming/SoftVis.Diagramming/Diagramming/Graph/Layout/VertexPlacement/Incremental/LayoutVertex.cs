using System;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.VertexPlacement.Incremental
{
    internal class LayoutVertex
    {
        private DiagramPoint _center;
        public IPositionedExtent OriginalVertex { get; }
        public int? Rank { get; set; }

        public event EventHandler<DiagramPoint> CenterChanged;

        private LayoutVertex(IPositionedExtent originalVertex)
        {
            OriginalVertex = originalVertex;
        }

        public static LayoutVertex Create(IPositionedExtent originalVertex)
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
        public DiagramRect Rect => new DiagramRect(Center.X - Width / 2, Center.Y - Height / 2, Center.X + Width / 2, Center.Y + Height / 2);
        public double Left => Rect.Left;
        public double Right => Rect.Right;
        public double Top => Rect.Top;
        public double Bottom => Rect.Bottom;

        public DiagramPoint Center
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
            return Rect.WithMarginX(marginX).Intersect(otherVertex.Rect) != DiagramRect.Empty;
        }

        public override string ToString()
        {
            return OriginalVertex?.ToString() ?? "(dummy)";
        }
    }
}
