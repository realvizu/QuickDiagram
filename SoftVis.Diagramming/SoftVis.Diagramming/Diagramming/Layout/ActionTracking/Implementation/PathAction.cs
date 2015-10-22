using System;
using System.Linq;

namespace Codartis.SoftVis.Diagramming.Layout.ActionTracking.Implementation
{
    /// <summary>
    /// An action of a layout logic run that effects a LayoutPath.
    /// </summary>
    internal class PathAction : LayoutAction, IPathAction
    {
        public LayoutPath Path { get; }

        public PathAction(string action, LayoutPath path, double? amount = null)
            :base(action, amount)
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