using System;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Modeling.Definition
{
    /// <summary>
    /// A model relationship type.
    /// </summary>
    public struct ModelRelationshipStereotype : IEquatable<ModelRelationshipStereotype>
    {
        public static readonly ModelRelationshipStereotype Default = default;
        public static readonly ModelRelationshipStereotype Containment = new ModelRelationshipStereotype(nameof(Containment));

        [NotNull] public string Name { get; }

        public ModelRelationshipStereotype([NotNull] string name)
        {
            Name = name;
        }

        public override string ToString() => Name;

        public bool Equals(ModelRelationshipStereotype other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            return obj is ModelRelationshipStereotype other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
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