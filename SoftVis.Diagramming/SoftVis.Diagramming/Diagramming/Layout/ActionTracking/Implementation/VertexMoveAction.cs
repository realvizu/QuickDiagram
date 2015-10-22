using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.ActionTracking.Implementation
{
    /// <summary>
    /// An action of a layout logic run that moves a LayoutVertex.
    /// </summary>
    internal class VertexMoveAction : VertexAction, IVertexMoveAction
    {
        public Point2D From { get; }
        public Point2D To { get; }
        public Point2D By { get; }

        public VertexMoveAction(LayoutVertexBase vertex, Point2D @from, Point2D to)
            : base("MoveVertex", vertex, to.X - @from.X)
        {
            From = @from;
            To = to;
            By = To - From;
        }

        private bool Equals(VertexMoveAction other)
        {
            return base.Equals(other) && By.Equals(other.By);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VertexMoveAction) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ By.GetHashCode();
            }
        }

        public static bool operator ==(VertexMoveAction left, VertexMoveAction right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(VertexMoveAction left, VertexMoveAction right)
        {
            return !Equals(left, right);
        }
    }
}