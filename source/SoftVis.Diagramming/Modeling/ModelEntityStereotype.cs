namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Provides an extensible categorization for model entities.
    /// </summary>
    /// <remarks>
    /// To extend the stereotypes just create new public static readonly instances in your class.
    /// </remarks>
    public struct ModelEntityStereotype
    {
        public static readonly ModelEntityStereotype None = new ModelEntityStereotype(string.Empty);

        public string Name { get; }

        public ModelEntityStereotype(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(ModelEntityStereotype other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ModelEntityStereotype && Equals((ModelEntityStereotype) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public static bool operator ==(ModelEntityStereotype left, ModelEntityStereotype right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ModelEntityStereotype left, ModelEntityStereotype right)
        {
            return !left.Equals(right);
        }
    }
}
