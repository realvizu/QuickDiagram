using System;
using System.Linq;
using Codartis.SoftVis.Diagramming.Layout.BaseActions;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// A layout action that affects a LayoutPath.
    /// </summary>
    internal class LayoutPathAction : DiagramConnectorAction
    {
        public LayoutPath Path { get; }

        public LayoutPathAction(string action, LayoutPath path, ILayoutAction causingLayoutAction = null)
            :base(action, path.FirstOrDefault()?.DiagramConnector, causingLayoutAction)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            Path = path;
        }

        public override string SubjectName => Path.ToString();

        protected bool Equals(LayoutPathAction other)
        {
            return base.Equals(other) && Equals(Path, other.Path);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LayoutPathAction) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ (Path != null ? Path.GetHashCode() : 0);
            }
        }

        public static bool operator ==(LayoutPathAction left, LayoutPathAction right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(LayoutPathAction left, LayoutPathAction right)
        {
            return !Equals(left, right);
        }
    }
}