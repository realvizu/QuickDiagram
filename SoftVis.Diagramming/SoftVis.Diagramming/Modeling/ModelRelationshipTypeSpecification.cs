namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Describes the type and stereotype of a model relationship.
    /// </summary>
    public struct ModelRelationshipTypeSpecification
    {
        public ModelRelationshipType Type { get; }
        public ModelRelationshipStereotype Stereotype { get; }

        public ModelRelationshipTypeSpecification(ModelRelationshipType type, ModelRelationshipStereotype stereotype)
        {
            Type = type;
            Stereotype = stereotype;
        }

        public bool Equals(ModelRelationshipTypeSpecification other)
        {
            return Type == other.Type && Stereotype.Equals(other.Stereotype);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ModelRelationshipTypeSpecification && Equals((ModelRelationshipTypeSpecification) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) Type*397) ^ Stereotype.GetHashCode();
            }
        }

        public static bool operator ==(ModelRelationshipTypeSpecification left, ModelRelationshipTypeSpecification right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ModelRelationshipTypeSpecification left, ModelRelationshipTypeSpecification right)
        {
            return !left.Equals(right);
        }
    }
}
