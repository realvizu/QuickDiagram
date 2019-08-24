using System;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Modeling.Implementation
{
    /// <summary>
    /// Implements an immutable model node.
    /// </summary>
    /// <remarks>
    /// The Id field is used to track identity through mutated instances so its value must be kept unchanged by all mutators.
    /// </remarks>
    public class ModelNode : IModelNode
    {
        public ModelNodeId Id { get; }
        [NotNull] public string Name { get; }
        [NotNull] public ModelNodeStereotype Stereotype { get; }
        public ModelOrigin Origin { get; }

        public ModelNode(ModelNodeId id, [NotNull] string name, [NotNull] ModelNodeStereotype stereotype, ModelOrigin origin)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Stereotype = stereotype ?? throw new ArgumentNullException(nameof(stereotype));
            Origin = origin;
        }

        public override string ToString() => $"{Stereotype} {Name} [{Id}]";

        public IModelNode WithName(string newName) => CreateInstance(Id, newName, Stereotype, Origin);

        [NotNull]
        protected virtual IModelNode CreateInstance(ModelNodeId id, [NotNull] string name, [NotNull] ModelNodeStereotype stereotype, ModelOrigin origin)
        {
            return new ModelNode(id, name, stereotype, origin);
        }
    }
}