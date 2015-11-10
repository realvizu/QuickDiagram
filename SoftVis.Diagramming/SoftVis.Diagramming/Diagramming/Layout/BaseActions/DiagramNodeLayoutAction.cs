using System;

namespace Codartis.SoftVis.Diagramming.Layout.BaseActions
{
    /// <summary>
    /// A layout action that affects a diagram node.
    /// </summary>
    internal class DiagramNodeLayoutAction : LayoutAction, IDiagramNodeLayoutAction
    {
        public DiagramNode DiagramNode { get; }

        public DiagramNodeLayoutAction(string action, DiagramNode diagramNode, ILayoutAction causingLayoutAction = null)
            :base(action, null, causingLayoutAction)
        {
            if (diagramNode == null) throw new ArgumentNullException(nameof(diagramNode));

            DiagramNode = diagramNode;
        }

        public override string SubjectName => DiagramNode.ToString();

        protected bool Equals(DiagramNodeLayoutAction other)
        {
            return base.Equals(other) && Equals(DiagramNode, other.DiagramNode);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DiagramNodeLayoutAction) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ (DiagramNode != null ? DiagramNode.GetHashCode() : 0);
            }
        }

        public static bool operator ==(DiagramNodeLayoutAction left, DiagramNodeLayoutAction right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DiagramNodeLayoutAction left, DiagramNodeLayoutAction right)
        {
            return !Equals(left, right);
        }
    }
}