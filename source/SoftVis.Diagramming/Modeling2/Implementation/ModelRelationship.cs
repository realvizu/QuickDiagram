using System;
using System.Diagnostics;
using QuickGraph;

namespace Codartis.SoftVis.Modeling2.Implementation
{
    /// <summary>
    /// An immutable implementation of the IModelRelationship interface with a QuickGraph edge.
    /// </summary>
    [DebuggerDisplay("{Source.DisplayName}--{GetType().Name}-->{Target.DisplayName}")]
    public class ModelRelationship : IModelRelationship, IEdge<IModelNode>
    {
        public ModelItemId Id { get; }
        public IModelNode Source { get; }
        public IModelNode Target { get; }

        public ModelRelationship(ModelItemId id, IModelNode source, IModelNode target)
        {
            Id = id;
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Target = target ?? throw new ArgumentNullException(nameof(target));
        }

        protected bool Equals(ModelRelationship other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ModelRelationship) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(ModelRelationship left, ModelRelationship right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ModelRelationship left, ModelRelationship right)
        {
            return !Equals(left, right);
        }

        public override string ToString() => $"{Source.DisplayName}--{GetType().Name}-->{Target.DisplayName} [{Id}]";
    }
}
