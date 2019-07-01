using System;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Extensible list of model node types.
    /// </summary>
    [Immutable]
    public sealed class ModelNodeStereotype
    {
        public string Name { get; }

        public ModelNodeStereotype(string name)
        {
            Name = name;
        }

        public override string ToString() => Name;
    }
}
