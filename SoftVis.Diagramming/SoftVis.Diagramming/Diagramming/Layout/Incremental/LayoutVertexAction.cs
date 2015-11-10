using System;
using Codartis.SoftVis.Diagramming.Layout.BaseActions;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// A layout action that affects a LayoutVertex.
    /// </summary>
    internal class LayoutVertexAction : LayoutAction
    {
        public LayoutVertexBase Vertex { get; }

        public LayoutVertexAction(string action, LayoutVertexBase vertex, double? amount = null, ILayoutAction causingLayoutAction = null)
            :base(action, amount, causingLayoutAction)
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));

            Vertex = vertex;
        }

        public override string SubjectName => Vertex.ToString();

        private bool Equals(LayoutVertexAction other)
        {
            return base.Equals(other) && Equals(Vertex, other.Vertex);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LayoutVertexAction) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ (Vertex != null ? Vertex.GetHashCode() : 0);
            }
        }

        public static bool operator ==(LayoutVertexAction left, LayoutVertexAction right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(LayoutVertexAction left, LayoutVertexAction right)
        {
            return !Equals(left, right);
        }
    }
}