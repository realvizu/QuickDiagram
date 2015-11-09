using Codartis.SoftVis.Diagramming.Layout.ActionTracking;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.ActionTracking
{
    /// <summary>
    /// A layout action that moves a DiagramNodeLayoutVertex.
    /// </summary>
    internal class MoveDiagramNodeAction : DiagramNodeAction, IMoveDiagramNodeAction, IMoveVertexAction
    {
        public DiagramNodeLayoutVertex Vertex { get; }
        public Point2D From { get; }
        public Point2D To { get; }
        public Point2D By { get; }

        public MoveDiagramNodeAction(DiagramNodeLayoutVertex diagramNodeLayoutVertex, Point2D @from, Point2D to, ILayoutAction causingLayoutAction = null)
            : base("MoveDiagramNode", diagramNodeLayoutVertex.DiagramNode, causingLayoutAction)
        {
            Vertex = diagramNodeLayoutVertex;
            From = @from;
            To = to;
            By = To - From;
        }

        LayoutVertexBase IMoveVertexAction.Vertex => Vertex;

        public override void AcceptVisitor(LayoutActionVisitorBase visitor)
        {
            base.AcceptVisitor(visitor);
            visitor.Visit(this);
        }

        protected bool Equals(MoveDiagramNodeAction other)
        {
            return base.Equals(other) && From.Equals(other.By);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MoveDiagramNodeAction) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ By.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(MoveDiagramNodeAction left, MoveDiagramNodeAction right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MoveDiagramNodeAction left, MoveDiagramNodeAction right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"{Action} ({SubjectName}) from {From} to {To}";
        }
    }
}