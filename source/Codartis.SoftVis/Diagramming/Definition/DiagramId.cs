using System;

namespace Codartis.SoftVis.Diagramming.Definition
{
    /// <summary>
    /// Identifies a diagram.
    /// </summary>
    public struct DiagramId : IEquatable<DiagramId>
    {
        private readonly Guid _id;

        public DiagramId(Guid id)
        {
            _id = id;
        }

        public static DiagramId Create() => new DiagramId(Guid.NewGuid());

        public override string ToString() => $"{GetType().Name}({_id})";

        public bool Equals(DiagramId other)
        {
            return _id.Equals(other._id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DiagramId && Equals((DiagramId)obj);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public static bool operator ==(DiagramId left, DiagramId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DiagramId left, DiagramId right)
        {
            return !left.Equals(right);
        }
    }
}
