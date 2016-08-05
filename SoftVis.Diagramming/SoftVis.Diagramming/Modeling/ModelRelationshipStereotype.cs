namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Provides an extensible categorization for model relationships.
    /// </summary>
    /// <remarks>
    /// To extend the stereotypes just create new public static readonly instances in your class.
    /// </remarks>
    public struct ModelRelationshipStereotype
    {
        public static readonly ModelRelationshipStereotype None = new ModelRelationshipStereotype(string.Empty);

        public string Name { get; }

        public ModelRelationshipStereotype(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(ModelRelationshipStereotype other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ModelRelationshipStereotype && Equals((ModelRelationshipStereotype) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public static bool operator ==(ModelRelationshipStereotype left, ModelRelationshipStereotype right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ModelRelationshipStereotype left, ModelRelationshipStereotype right)
        {
            return !left.Equals(right);
        }
    }
}
