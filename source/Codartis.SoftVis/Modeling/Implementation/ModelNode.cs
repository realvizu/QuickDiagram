using System;
using System.Collections.Generic;

namespace Codartis.SoftVis.Modeling.Implementation
{
    /// <summary>
    /// Implements an immutable model node.
    /// </summary>
    /// <remarks>
    /// The Id field is used to track identity through mutated instances so its value must be kept unchanged by all mutators.
    /// </remarks>
    [Immutable]
    public class ModelNode : IModelNode
    {
        public ModelNodeId Id { get; }
        public string Name { get; }
        public ModelNodeStereotype Stereotype { get; }
        public ModelOrigin Origin { get; }

        public ModelNode(ModelNodeId id, string name, ModelNodeStereotype stereotype, ModelOrigin origin)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Stereotype = stereotype ?? throw new ArgumentNullException(nameof(stereotype));
            Origin = origin;
        }

        public override string ToString() => $"{Stereotype} {Name} [{Id}]";

        public static IEqualityComparer<IModelNode> IdComparer { get; } = new IdEqualityComparer();

        private sealed class IdEqualityComparer : IEqualityComparer<IModelNode>
        {
            public bool Equals(IModelNode x, IModelNode y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Id.Equals(y.Id);
            }

            public int GetHashCode(IModelNode obj)
            {
                return obj.Id.GetHashCode();
            }
        }
    }
}
