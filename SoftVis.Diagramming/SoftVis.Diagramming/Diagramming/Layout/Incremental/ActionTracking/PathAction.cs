using System;
using System.Linq;
using Codartis.SoftVis.Diagramming.Layout.ActionTracking;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.ActionTracking
{
    /// <summary>
    /// A layout action that affects a LayoutPath.
    /// </summary>
    internal class PathAction : LayoutAction, IDiagramConnectorAction
    {
        public LayoutPath Path { get; }

        public PathAction(string action, LayoutPath path, ILayoutAction causingLayoutAction = null)
            :base(action, null, causingLayoutAction)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            Path = path;
        }

        public override string SubjectName => Path.ToString();
        public DiagramConnector DiagramConnector => Path.FirstOrDefault()?.DiagramConnector;

        protected bool Equals(PathAction other)
        {
            return base.Equals(other) && Equals(Path, other.Path);
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
                return (base.GetHashCode()*397) ^ (Path != null ? Path.GetHashCode() : 0);
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
    }
}