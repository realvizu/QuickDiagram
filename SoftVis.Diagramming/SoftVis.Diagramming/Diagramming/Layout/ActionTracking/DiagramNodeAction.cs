using System;

namespace Codartis.SoftVis.Diagramming.Layout.ActionTracking
{
    /// <summary>
    /// A layout action that affects a diagram node.
    /// </summary>
    internal class DiagramNodeAction : LayoutAction, IDiagramNodeAction
    {
        public DiagramNode DiagramNode { get; }

        public DiagramNodeAction(string action, DiagramNode diagramNode, ILayoutAction causingLayoutAction = null)
            :base(action, null, causingLayoutAction)
        {
            if (diagramNode == null) throw new ArgumentNullException(nameof(diagramNode));

            DiagramNode = diagramNode;
        }

        public override string SubjectName => DiagramNode.ToString();

        protected bool Equals(DiagramNodeAction other)
        {
            return base.Equals(other) && Equals(DiagramNode, other.DiagramNode);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DiagramNodeAction) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ (DiagramNode != null ? DiagramNode.GetHashCode() : 0);
            }
        }

        public static bool operator ==(DiagramNodeAction left, DiagramNodeAction right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DiagramNodeAction left, DiagramNodeAction right)
        {
            return !Equals(left, right);
        }
    }
}