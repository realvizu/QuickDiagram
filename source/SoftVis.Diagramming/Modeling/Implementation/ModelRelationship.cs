using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Graphs;
using Codartis.SoftVis.Graphs.Immutable;

namespace Codartis.SoftVis.Modeling.Implementation
{
    /// <summary>
    /// Implements an immutable model relationships.
    /// Validates the type of the source and target nodes.
    /// </summary>
    /// <remarks>
    /// The Id field is used to track identity through mutated instances so its value must be kept unchanged by all mutators.
    /// </remarks>
    public class ModelRelationship : IModelRelationship
    {
        public ModelRelationshipId Id { get; }
        public IModelNode Source { get; }
        public IModelNode Target { get; }
        public ModelRelationshipStereotype Stereotype { get; }

        public ModelRelationship(ModelRelationshipId id, IModelNode source, IModelNode target, ModelRelationshipStereotype stereotype)
        {
            Id = id;
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Target = target ?? throw new ArgumentNullException(nameof(target));
            Stereotype = stereotype ?? throw new ArgumentNullException(nameof(stereotype));

            if (!IsValidSourceAndTargetType(source, target))
                throw new ArgumentException($"{source.Stereotype} and {target.Stereotype} can not be in {Stereotype} relationship.");
        }

        public override string ToString() => $"{Source.Name}--{Stereotype}-->{Target.Name} [{Id}]";

        public bool IsNodeRelated(IModelNode modelNode, DirectedModelRelationshipType directedModelRelationshipType)
        {
            return Stereotype == directedModelRelationshipType.Stereotype 
                && this.ContainsNodeOnGivenEnd(modelNode, directedModelRelationshipType.Direction);
        }

        public IModelRelationship WithSource(IModelNode newSourceNode) => CreateInstance(Id, newSourceNode, Target, Stereotype);
        public IModelRelationship WithTarget(IModelNode newTargetNode) => CreateInstance(Id, Source, newTargetNode, Stereotype);

        protected virtual ModelRelationship CreateInstance(ModelRelationshipId id, IModelNode source, IModelNode target, ModelRelationshipStereotype stereotype) 
            => new ModelRelationship(id, source, target, stereotype);

        /// <summary>
        /// Returns all valid source node type - target node type pairs.
        /// </summary>
        /// <returns>A collection of valid source and target node type pairs. Null means skip validation.</returns>
        protected virtual IEnumerable<(ModelNodeStereotype, ModelNodeStereotype)> GetValidSourceAndTargetNodeTypePairs()
            => null;

        private bool IsValidSourceAndTargetType(IModelNode source, IModelNode target)
        {
            return GetValidSourceAndTargetNodeTypePairs() == null 
                || GetValidSourceAndTargetNodeTypePairs().Contains((source.Stereotype, target.Stereotype));
        }

        public static IEqualityComparer<IModelRelationship> IdComparer { get; } = new IdEqualityComparer();

        private sealed class IdEqualityComparer : IEqualityComparer<IModelRelationship>
        {
            public bool Equals(IModelRelationship x, IModelRelationship y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Id.Equals(y.Id);
            }

            public int GetHashCode(IModelRelationship obj)
            {
                return obj.Id.GetHashCode();
            }
        }
    }
}
