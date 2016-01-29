using System.Diagnostics;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// A simple implementation of the IModelRelationship interface.
    /// </summary>
    [DebuggerDisplay("{Source.Name}--{Type}/{Stereotype}-->{Target.Name}")]
    public class ModelRelationship : IModelRelationship
    {
        public IModelEntity Source { get; }
        public IModelEntity Target { get; }
        public ModelRelationshipType Type { get; }
        public ModelRelationshipStereotype Stereotype { get; }

        public ModelRelationship(IModelEntity source, IModelEntity target, ModelRelationshipType type, 
            ModelRelationshipStereotype stereotype = null)
        {
            Source = source;
            Target = target;
            Type = type;
            Stereotype = stereotype;
        }

        public bool IsOfType(RelationshipTypeSpecification typeSpecification)
        {
            return Type == typeSpecification.Type && Stereotype == typeSpecification.Stereotype;
        }
    }
}
