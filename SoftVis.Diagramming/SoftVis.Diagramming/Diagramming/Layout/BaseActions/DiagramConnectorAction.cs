using System;

namespace Codartis.SoftVis.Diagramming.Layout.BaseActions
{
    /// <summary>
    /// A layout action that affects a diagram connector.
    /// </summary>
    internal class DiagramConnectorAction : LayoutAction, IDiagramConnectorLayoutAction
    {
        public DiagramConnector DiagramConnector { get; }

        public DiagramConnectorAction(string action, DiagramConnector diagramConnector, ILayoutAction causingLayoutAction = null)
            :base(action, null, causingLayoutAction)
        {
            if (diagramConnector == null) throw new ArgumentNullException(nameof(diagramConnector));

            DiagramConnector = diagramConnector;
        }

        public override string SubjectName => DiagramConnector.ToString();

        protected bool Equals(DiagramConnectorAction other)
        {
            return base.Equals(other) && Equals(DiagramConnector, other.DiagramConnector);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DiagramConnectorAction) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ (DiagramConnector != null ? DiagramConnector.GetHashCode() : 0);
            }
        }

        public static bool operator ==(DiagramConnectorAction left, DiagramConnectorAction right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DiagramConnectorAction left, DiagramConnectorAction right)
        {
            return !Equals(left, right);
        }
    }
}