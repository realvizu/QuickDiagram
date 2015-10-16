using System;
using System.Diagnostics;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    [DebuggerDisplay("{Vertex}{To}")]
    internal class VertexMove : IEquatable<VertexMove>
    {
        public LayoutVertex Vertex { get; }
        public Point2D From { get; }
        public Point2D To { get; }
        public Point2D Move { get; }

        public VertexMove(LayoutVertex vertex, Point2D @from, Point2D to)
        {
            Vertex = vertex;
            From = @from;
            To = to;
            Move = to - @from;
        }

        public bool Equals(VertexMove other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Vertex.Equals(other.Vertex) && Move.Equals(other.Move);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VertexMove) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Vertex.GetHashCode()*397) ^ Move.GetHashCode();
            }
        }

        public static bool operator ==(VertexMove left, VertexMove right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(VertexMove left, VertexMove right)
        {
            return !Equals(left, right);
        }
    }
}