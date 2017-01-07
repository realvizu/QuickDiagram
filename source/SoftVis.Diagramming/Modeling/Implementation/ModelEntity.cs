using System.Diagnostics;

namespace Codartis.SoftVis.Modeling.Implementation
{
    /// <summary>
    /// A simple implementation of the IModelEntity interface.
    /// </summary>
    [DebuggerDisplay("{Name} ({Classifier}/{Stereotype})")]
    public class ModelEntity : IModelEntity
    {
        public string Name { get; private set; }
        public string FullName { get; private set; }
        public string Description { get; private set; }
        public ModelEntityClassifier Classifier { get; }
        public ModelEntityStereotype Stereotype { get; }
        public ModelOrigin Origin { get; }

        public virtual int Priority => 0;
        public virtual bool IsAbstract => false;

        public ModelEntity(string name, string fullName, string description,
            ModelEntityClassifier classifier, ModelEntityStereotype stereotype, ModelOrigin origin)
        {
            UpdateName(name, fullName, description);

            Classifier = classifier;
            Stereotype = stereotype;
            Origin = origin;
        }

        public void UpdateName(string name, string fullName, string description)
        {
            Name = name;
            FullName = fullName;
            Description = description;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
