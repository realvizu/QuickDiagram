namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Describes the classifier and stereotype of a model relationship.
    /// </summary>
    public struct ModelRelationshipType
    {
        public ModelRelationshipClassifier Classifier { get; }
        public ModelRelationshipStereotype Stereotype { get; }

        public ModelRelationshipType(ModelRelationshipClassifier classifier, ModelRelationshipStereotype stereotype)
        {
            Classifier = classifier;
            Stereotype = stereotype;
        }

        public bool Equals(ModelRelationshipType other)
        {
            return Classifier == other.Classifier && Stereotype.Equals(other.Stereotype);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ModelRelationshipType && Equals((ModelRelationshipType) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) Classifier*397) ^ Stereotype.GetHashCode();
            }
        }

        public static bool operator ==(ModelRelationshipType left, ModelRelationshipType right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ModelRelationshipType left, ModelRelationshipType right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"({Classifier}/{Stereotype})";
        }
    }
}
