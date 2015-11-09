using Codartis.SoftVis.Diagramming.Layout.ActionTracking;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.ActionTracking
{
    /// <summary>
    /// A layout action that moves a DummyLayoutVertex.
    /// </summary>
    internal class MoveDummyVertexAction : VertexAction, IMoveVertexAction
    {
        public Point2D From { get; }
        public Point2D To { get; }
        public Point2D By { get; }

        public MoveDummyVertexAction(DummyLayoutVertex vertex, Point2D @from, Point2D to, ILayoutAction causingLayoutAction = null)
            : base("MoveDummyVertex", vertex, to.X - @from.X, causingLayoutAction)
        {
            From = @from;
            To = to;
            By = To - From;
        }

        private bool Equals(MoveDummyVertexAction other)
        {
            return base.Equals(other) && By.Equals(other.By);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MoveDummyVertexAction) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ By.GetHashCode();
            }
        }

        public static bool operator ==(MoveDummyVertexAction left, MoveDummyVertexAction right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MoveDummyVertexAction left, MoveDummyVertexAction right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"{Action} ({SubjectName}) from {From} to {To}";
        }
    }
}