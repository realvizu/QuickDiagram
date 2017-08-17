namespace Codartis.SoftVis.Modeling2
{
    /// <summary>
    /// Extensible list of model node types.
    /// </summary>
    public class ModelNodeStereotype
    {
        public static readonly ModelNodeStereotype Class = new ModelNodeStereotype(nameof(Class));

        public string Name { get; }

        protected ModelNodeStereotype(string name)
        {
            Name = name;
        }

        public override string ToString() => Name;
    }
}
