using Codartis.SoftVis.Diagramming.Layout.ActionTracking;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.ActionTracking
{
    /// <summary>
    /// A layout action that reroutes a LayoutPath.
    /// </summary>
    internal class ReroutePathAction : PathAction, IRerouteDiagramConnectorAction
    {
        public Route OldRoute { get; }
        public Route NewRoute { get; }

        public ReroutePathAction(string action, LayoutPath path, Route oldRoute, Route newRoute, 
            ILayoutAction causingLayoutAction = null)
            :base(action, path, causingLayoutAction)
        {
            OldRoute = oldRoute;
            NewRoute = newRoute;
        }

        protected bool Equals(ReroutePathAction other)
        {
            return base.Equals(other) && Equals(OldRoute, other.OldRoute) && Equals(NewRoute, other.NewRoute);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ReroutePathAction) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ (OldRoute != null ? OldRoute.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (NewRoute != null ? NewRoute.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(ReroutePathAction left, ReroutePathAction right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ReroutePathAction left, ReroutePathAction right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"{Action} ({SubjectName}) from {OldRoute} to {NewRoute}";
        }
    }
}