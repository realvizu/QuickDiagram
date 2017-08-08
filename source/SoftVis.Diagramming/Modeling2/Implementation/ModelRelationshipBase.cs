using System;
using System.Collections.Generic;
using System.Linq;
using QuickGraph;

namespace Codartis.SoftVis.Modeling2.Implementation
{
    /// <summary>
    /// Abstract base class for model relationships. 
    /// Implements a QuickGraph edge. 
    /// Immutable.
    /// </summary>
    /// <remarks>
    /// The Id field is used to track identity through mutated instances so its value must be kept unchanged by all mutators.
    /// </remarks>
    public abstract class ModelRelationshipBase : IModelRelationship, IEdge<IModelNode>
    {
        public ModelItemId Id { get; }
        public IModelNode Source { get; }
        public IModelNode Target { get; }

        protected ModelRelationshipBase(ModelItemId id, IModelNode source, IModelNode target)
        {
            Id = id;
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Target = target ?? throw new ArgumentNullException(nameof(target));

            ValidateSourceAndTargetTypes(source, target);
        }

        /// <summary>
        /// Returns all valid source node type - target node type pairs.
        /// </summary>
        /// <returns>A collection of valid source and target node type pairs.</returns>
        protected abstract IEnumerable<(Type, Type)> GetValidSourceAndTargetNodeTypePairs();

        protected bool Equals(ModelRelationshipBase other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ModelRelationshipBase) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(ModelRelationshipBase left, ModelRelationshipBase right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ModelRelationshipBase left, ModelRelationshipBase right)
        {
            return !Equals(left, right);
        }

        public override string ToString() => $"{Source.DisplayName}--{GetType().Name}-->{Target.DisplayName} [{Id}]";

        private void ValidateSourceAndTargetTypes(IModelNode source, IModelNode target)
        {
            if (!GetValidSourceAndTargetNodeTypePairs().Contains((source.GetType(), target.GetType())))
                throw new ArgumentException($"{source.GetType().Name} and {target.GetType().Name} can not be in {GetType().Name} relationship.");
        }

    }
}
