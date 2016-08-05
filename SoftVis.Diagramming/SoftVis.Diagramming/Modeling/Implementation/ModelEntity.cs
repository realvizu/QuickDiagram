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
        public ModelEntityClassifier Classifier { get; }
        public ModelEntityStereotype Stereotype { get; }
        public virtual int Priority => 0;
        public virtual bool IsAbstract => false;

        protected ModelEntity(string name, ModelEntityClassifier classifier, ModelEntityStereotype stereotype)
        {
            Name = name;
            Classifier = classifier;
            Stereotype = stereotype;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
