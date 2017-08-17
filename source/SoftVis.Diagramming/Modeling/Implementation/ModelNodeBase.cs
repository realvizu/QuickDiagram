using System;

namespace Codartis.SoftVis.Modeling.Implementation
{
    /// <summary>
    /// Abstract base class for implementations of the IModelNode interface.
    /// Immutable.
    /// </summary>
    /// <remarks>
    /// Descendants must keep the immutability: mutators must return a new (mutated) instance.
    /// The Id field is used to track identity through mutated instances so its value must be kept unchanged by all mutators.
    /// </remarks>
    public abstract class ModelNodeBase : IModelNode
    {
        public ModelItemId Id { get; }
        public string Name { get; }
        public ModelNodeStereotype Stereotype { get; }
        public ModelOrigin Origin { get; }

        protected ModelNodeBase(ModelItemId id, string name, ModelNodeStereotype stereotype, ModelOrigin origin)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Stereotype = stereotype ?? throw new ArgumentNullException(nameof(stereotype));
            Origin = origin;
        }

        public abstract int LayoutPriority { get; }

        public override string ToString() => $"{Stereotype} {Name} [{Id}]";
    }
}
