namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// A kind of relationship from the perspective of an entity, given with the relationship type and direction.
    /// </summary>
    public struct EntityRelationType
    {
        public string Name { get; }
        public ModelRelationshipType Type { get; }
        public EntityRelationDirection Direction { get; }

        public EntityRelationType(string name, ModelRelationshipClassifier classifier, ModelRelationshipStereotype stereotype, 
            EntityRelationDirection direction)
            : this(name, new ModelRelationshipType(classifier, stereotype), direction)
        {
        }

        public EntityRelationType(string name, ModelRelationshipType type, EntityRelationDirection direction)
        {
            Name = name;
            Type = type;
            Direction = direction;
        }

        public ModelRelationshipClassifier Classifier => Type.Classifier;
        public ModelRelationshipStereotype Stereotype => Type.Stereotype;

        public bool Equals(EntityRelationType other)
        {
            return Type.Equals(other.Type) && Direction == other.Direction;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is EntityRelationType && Equals((EntityRelationType) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Type.GetHashCode()*397) ^ (int) Direction;
            }
        }

        public static bool operator ==(EntityRelationType left, EntityRelationType right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EntityRelationType left, EntityRelationType right)
        {
            return !left.Equals(right);
        }
    }
}
