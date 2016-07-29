using System.Diagnostics.Contracts;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Specifies related entities, by defining the relationship type and the direction to navigate it.
    /// </summary>
    public struct RelatedEntitySpecification
    {
        public ModelRelationshipTypeSpecification TypeSpecification { get; }
        public EntityRelationDirection Direction { get; }

        public RelatedEntitySpecification(ModelRelationshipType type, ModelRelationshipStereotype stereotype,
            EntityRelationDirection direction)
            : this(new ModelRelationshipTypeSpecification(type, stereotype), direction)
        {
        }

        public RelatedEntitySpecification(ModelRelationshipTypeSpecification typeSpecification, 
            EntityRelationDirection direction)
        {
            TypeSpecification = typeSpecification;
            Direction = direction;
        }

        public ModelRelationshipType Type => TypeSpecification.Type;
        public ModelRelationshipStereotype Stereotype => TypeSpecification.Stereotype;

        [Pure]
        public bool IsSpecifiedBy(RelatedEntitySpecification? relatedEntitySpecification)
        {
            return relatedEntitySpecification == null || relatedEntitySpecification.Value == this;
        }

        public bool Equals(RelatedEntitySpecification other)
        {
            return TypeSpecification.Equals(other.TypeSpecification) && Direction == other.Direction;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is RelatedEntitySpecification && Equals((RelatedEntitySpecification) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (TypeSpecification.GetHashCode()*397) ^ (int) Direction;
            }
        }

        public static bool operator ==(RelatedEntitySpecification left, RelatedEntitySpecification right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RelatedEntitySpecification left, RelatedEntitySpecification right)
        {
            return !left.Equals(right);
        }
    }
}
