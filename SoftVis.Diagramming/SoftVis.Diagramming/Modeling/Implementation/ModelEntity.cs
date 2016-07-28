using System.Diagnostics;

namespace Codartis.SoftVis.Modeling.Implementation
{
    /// <summary>
    /// A simple implementation of the IModelEntity interface.
    /// </summary>
    [DebuggerDisplay("{Name} ({Type}/{Stereotype})")]
    public class ModelEntity : IModelEntity
    {
        public string Name { get; }
        public ModelEntityType Type { get; }
        public ModelEntityStereotype Stereotype { get; }
        public virtual int Priority => 0;
        public virtual bool IsAbstract => false;

        protected ModelEntity(string name, ModelEntityType type, ModelEntityStereotype stereotype)
        {
            Name = name;
            Type = type;
            Stereotype = stereotype;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
