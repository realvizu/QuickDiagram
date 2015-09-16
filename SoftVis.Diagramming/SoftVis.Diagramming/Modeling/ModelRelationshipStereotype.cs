namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Provides an extensible categorization for model relationships.
    /// </summary>
    public abstract class ModelRelationshipStereotype
    {
        public string Name { get; }

        protected ModelRelationshipStereotype(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
