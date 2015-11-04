using System;
using System.Linq;
using Codartis.SoftVis.Diagramming.Layout.ActionTracking;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.ActionTracking
{
    /// <summary>
    /// A layout action that affects a LayoutPath.
    /// </summary>
    internal class PathAction : LayoutAction, IRerouteDiagramConnectorAction
    {
        public LayoutPath Path { get; }
        public Route OldRoute { get; }
        public Route NewRoute { get; }

        public PathAction(string action, LayoutPath path, Route oldRoute, Route newRoute, 
            ILayoutAction causingLayoutAction = null)
            :base(action, null, causingLayoutAction)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            Path = path;
            OldRoute = oldRoute;
            NewRoute = newRoute;
        }

        public override string SubjectName => Path.ToString();
        public DiagramConnector DiagramConnector => Path.FirstOrDefault()?.DiagramConnector;

        protected bool Equals(PathAction other)
        {
            return base.Equals(other) && Equals(Path, other.Path) && Equals(OldRoute, other.OldRoute) && Equals(NewRoute, other.NewRoute);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PathAction) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ (Path != null ? Path.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (OldRoute != null ? OldRoute.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (NewRoute != null ? NewRoute.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(PathAction left, PathAction right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PathAction left, PathAction right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"{Action} ({SubjectName}) from {OldRoute} to {NewRoute}";
        }
    }
}