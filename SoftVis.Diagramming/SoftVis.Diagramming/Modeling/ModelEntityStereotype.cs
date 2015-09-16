namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Provides an extensible categorization for model entities.
    /// </summary>
    public abstract class ModelEntityStereotype
    {
        public string Name { get; }

        protected ModelEntityStereotype(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
