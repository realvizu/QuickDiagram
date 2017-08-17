namespace Codartis.SoftVis.Modeling2
{
    /// <summary>
    /// Extensible list of model relationship types.
    /// </summary>
    public class ModelRelationshipStereotype
    {
        public static readonly ModelRelationshipStereotype Containment = new ModelRelationshipStereotype(nameof(Containment));

        public string Name { get; }

        protected ModelRelationshipStereotype(string name)
        {
            Name = name;
        }

        public override string ToString() => Name;
    }
}
