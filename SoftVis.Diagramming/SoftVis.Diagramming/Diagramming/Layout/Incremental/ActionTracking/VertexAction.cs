using System;
using Codartis.SoftVis.Diagramming.Layout.ActionTracking;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.ActionTracking
{
    /// <summary>
    /// A layout action that affects a LayoutVertex.
    /// </summary>
    internal class VertexAction : LayoutAction
    {
        public LayoutVertexBase Vertex { get; }

        public VertexAction(string action, LayoutVertexBase vertex, double? amount = null, ILayoutAction causingLayoutAction = null)
            :base(action, amount, causingLayoutAction)
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));

            Vertex = vertex;
        }

        public override string SubjectName => Vertex.ToString();

        private bool Equals(VertexAction other)
        {
            return base.Equals(other) && Equals(Vertex, other.Vertex);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VertexAction) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ (Vertex != null ? Vertex.GetHashCode() : 0);
            }
        }

        public static bool operator ==(VertexAction left, VertexAction right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(VertexAction left, VertexAction right)
        {
            return !Equals(left, right);
        }
    }
}