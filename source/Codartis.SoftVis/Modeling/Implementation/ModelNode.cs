using System.Collections.Generic;
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
    public sealed class ModelNode : IModelNode
    {
        public static IEqualityComparer<IModelNode> IdComparer { get; } = new IdEqualityComparer();

        public ModelNodeId Id { get; }
        public string Name { get; }
        public ModelNodeStereotype Stereotype { get; }
        public ModelOrigin Origin { get; }

        public ModelNode(ModelNodeId id, [NotNull] string name, ModelNodeStereotype stereotype, ModelOrigin origin)
        {
            Id = id;
            Name = name;
            Stereotype = stereotype;
            Origin = origin;
        }


        public override string ToString() => $"<<{Stereotype}>> {Name} [{Id}]";

        public IModelNode WithName(string newName) => CreateInstance(Id, newName, Stereotype, Origin);

        [NotNull]
        private static IModelNode CreateInstance(ModelNodeId id, [NotNull] string name, ModelNodeStereotype stereotype, ModelOrigin origin)
        {
            return new ModelNode(id, name, stereotype, origin);
        }

        private sealed class IdEqualityComparer : IEqualityComparer<IModelNode>
        {
            public bool Equals(IModelNode x, IModelNode y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null))
                    return false;
                if (ReferenceEquals(y, null))
                    return false;
                if (x.GetType() != y.GetType())
                    return false;

                return x.Id.Equals(y.Id);
            }

            public int GetHashCode(IModelNode obj)
            {
                return obj.Id.GetHashCode();
            }
        }
    }
}