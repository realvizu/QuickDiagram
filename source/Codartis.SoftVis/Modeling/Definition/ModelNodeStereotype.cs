namespace Codartis.SoftVis.Modeling.Definition
{
    /// <summary>
    /// Extensible list of model node types.
    /// </summary>
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
