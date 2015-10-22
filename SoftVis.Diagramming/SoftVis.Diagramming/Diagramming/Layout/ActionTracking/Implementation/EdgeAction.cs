using System;

namespace Codartis.SoftVis.Diagramming.Layout.ActionTracking.Implementation
{
    /// <summary>
    /// An action of a layout logic run that effects a LayoutEdge.
    /// </summary>
    internal class EdgeAction : LayoutAction, IEdgeAction
    {
        public LayoutEdge Edge { get; }

        public EdgeAction(string action, LayoutEdge edge, double? amount = null)
            :base(action, amount)
        {
            if (edge == null) throw new ArgumentNullException(nameof(edge));

            Edge = edge;
        }

        public override string SubjectName => Edge.ToString();
        public DiagramConnector DiagramConnector => Edge.DiagramConnector;

        private bool Equals(EdgeAction other)
        {
            return base.Equals(other) && Equals(Edge, other.Edge);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EdgeAction) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ (Edge != null ? Edge.GetHashCode() : 0);
            }
        }

        public static bool operator ==(EdgeAction left, EdgeAction right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EdgeAction left, EdgeAction right)
        {
            return !Equals(left, right);
        }
    }
}