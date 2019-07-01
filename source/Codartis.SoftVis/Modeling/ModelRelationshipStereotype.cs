using System;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Extensible list of model relationship types.
    /// </summary>
    [Immutable]
    public sealed class ModelRelationshipStereotype
    {
        public static readonly ModelRelationshipStereotype Containment = new ModelRelationshipStereotype(nameof(Containment));

        public string Name { get; }

        public ModelRelationshipStereotype(string name)
        {
            Name = name;
        }

        public override string ToString() => Name;
    }
}
