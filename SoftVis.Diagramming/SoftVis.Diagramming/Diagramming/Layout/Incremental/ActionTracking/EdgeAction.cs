using System;
using Codartis.SoftVis.Diagramming.Layout.ActionTracking;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.ActionTracking
{
    /// <summary>
    /// A layout action that affects a PositioningEdge.
    /// </summary>
    internal class EdgeAction : LayoutAction, IDiagramConnectorAction
    {
        public PositioningEdge Edge { get; }

        public EdgeAction(string action, PositioningEdge edge, double? amount = null, ILayoutAction causingLayoutAction = null)
            :base(action, amount, causingLayoutAction)
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