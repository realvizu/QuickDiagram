using System;
using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Modeling.Definition
{
    /// <summary>
    /// A relationship type with a direction.
    /// </summary>
    public struct DirectedModelRelationshipType : IEquatable<DirectedModelRelationshipType>
    {
        public ModelRelationshipStereotype Stereotype { get; }
        public EdgeDirection Direction { get; }

        public DirectedModelRelationshipType(ModelRelationshipStereotype stereotype, EdgeDirection direction)
        {
            Stereotype = stereotype;
            Direction = direction;
        }

        public override string ToString() => $"<<{Stereotype}>>/{Direction}";

        public bool Equals(DirectedModelRelationshipType other)
        {
            return Stereotype.Equals(other.Stereotype) && Direction == other.Direction;
        }

        public override bool Equals(object obj)
        {
            return obj is DirectedModelRelationshipType other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Stereotype.GetHashCode() * 397) ^ (int)Direction;
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