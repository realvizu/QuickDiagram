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
    }
}