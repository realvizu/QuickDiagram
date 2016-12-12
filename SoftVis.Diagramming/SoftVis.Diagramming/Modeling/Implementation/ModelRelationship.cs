using System.Diagnostics;
using QuickGraph;

namespace Codartis.SoftVis.Modeling.Implementation
{
    /// <summary>
    /// An implementation of the IModelRelationship interface with a QuickGraph edge.
    /// </summary>
    [DebuggerDisplay("{Source.Name}--{Classifier}/{Stereotype}-->{Target.Name}")]
    public class ModelRelationship : IModelRelationship, IEdge<IModelEntity>
    {
        private readonly IModelEntity _source;
        private readonly IModelEntity _target;

        public IModelEntity Source => _source;
        public IModelEntity Target => _target;

        public ModelRelationshipClassifier Classifier { get; }
        public ModelRelationshipStereotype Stereotype { get; }
        public ModelRelationshipType Type => new ModelRelationshipType(Classifier, Stereotype);

        public ModelRelationship(IModelEntity source, IModelEntity target, ModelRelationshipType type)
            : this(source, target, type.Classifier, type.Stereotype)
        { }

        public ModelRelationship(IModelEntity source, IModelEntity target,
            ModelRelationshipClassifier classifier, ModelRelationshipStereotype stereotype)
        {
            _source = source;
            _target = target;
            Classifier = classifier;
            Stereotype = stereotype;
        }

        public override string ToString() => $"{Source.Name}--{Classifier}/{Stereotype}-->{Target.Name}";
    }
}
