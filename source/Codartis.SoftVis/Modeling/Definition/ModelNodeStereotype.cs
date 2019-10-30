using JetBrains.Annotations;

namespace Codartis.SoftVis.Modeling.Definition
{
    /// <summary>
    /// A model node type.
    /// </summary>
    public struct ModelNodeStereotype
    {
        public static ModelNodeStereotype Default = default;

        [NotNull] public string Name { get; }

        public ModelNodeStereotype([NotNull] string name)
        {
            Name = name;
        }

        public override string ToString() => Name;

        public bool Equals(ModelNodeStereotype other)
        {
            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            return obj is ModelNodeStereotype other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(ModelNodeStereotype left, ModelNodeStereotype right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ModelNodeStereotype left, ModelNodeStereotype right)
        {
            return !left.Equals(right);
        }
    }
}