using System.Diagnostics;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// A simple implementation of the IModelRelationship interface.
    /// </summary>
    [DebuggerDisplay("{Source.Name}-->{Target.Name}")]
    public class ModelRelationship : IModelRelationship
    {
        public IModelEntity Source { get; }
        public IModelEntity Target { get; }
        public ModelRelationshipType Type { get; }

        public ModelRelationship(IModelEntity source, IModelEntity target, ModelRelationshipType type)
        {
            Source = source;
            Target = target;
            Type = type;
        }
    }
}
