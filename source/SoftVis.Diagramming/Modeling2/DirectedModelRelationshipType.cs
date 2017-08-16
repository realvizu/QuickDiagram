using System;

namespace Codartis.SoftVis.Modeling2
{
    /// <summary>
    /// A relatinship type with a direction.
    /// </summary>
    public struct DirectedModelRelationshipType : IEquatable<DirectedModelRelationshipType>
    {
        public Type Type { get; }
        public RelationshipDirection Direction { get; }

        public DirectedModelRelationshipType(Type type, RelationshipDirection direction)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Direction = direction;
        }

        public bool Equals(DirectedModelRelationshipType other)
        {
            return Type.Equals(other.Type) && Direction == other.Direction;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DirectedModelRelationshipType && Equals((DirectedModelRelationshipType) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Type.GetHashCode() * 397) ^ (int) Direction;
            }
        }

        public static bool operator ==(DirectedModelRelationshipType left, DirectedModelRelationshipType right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DirectedModelRelationshipType left, DirectedModelRelationshipType right)
        {
            return !left.Equals(right);
        }
    }
}
