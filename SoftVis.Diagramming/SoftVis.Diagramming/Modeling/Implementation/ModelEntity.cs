using System.Diagnostics;

namespace Codartis.SoftVis.Modeling.Implementation
{
    /// <summary>
    /// A simple implementation of the IModelEntity interface.
    /// </summary>
    [DebuggerDisplay("{Name} ({Classifier}/{Stereotype})")]
    public class ModelEntity : IModelEntity
    {
        public string Name { get; }
        public string FullName { get; }
        public ModelEntityClassifier Classifier { get; }
        public ModelEntityStereotype Stereotype { get; }
        public ModelOrigin Origin { get; }

        public virtual int Priority => 0;
        public virtual bool IsAbstract => false;

        protected ModelEntity(string name, string fullName,
            ModelEntityClassifier classifier, ModelEntityStereotype stereotype, ModelOrigin origin)
        {
            Name = name;
            FullName = fullName;
            Classifier = classifier;
            Stereotype = stereotype;
            Origin = origin;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
