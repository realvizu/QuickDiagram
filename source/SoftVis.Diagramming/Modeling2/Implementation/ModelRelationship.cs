using System;
using System.Diagnostics;
using QuickGraph;

namespace Codartis.SoftVis.Modeling2.Implementation
{
    /// <summary>
    /// An implementation of the IModelRelationship interface with a QuickGraph edge.
    /// Immutable.
    /// </summary>
    [DebuggerDisplay("{Source.DisplayName}--{GetType().Name}-->{Target.DisplayName}")]
    public class ModelRelationship : IModelRelationship, IEdge<IModelNode>
    {
        public IModelNode Source { get; }
        public IModelNode Target { get; }

        public ModelRelationship(IModelNode source, IModelNode target)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Target = target ?? throw new ArgumentNullException(nameof(target));
        }

        public bool Equals(ModelRelationship other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Source, other.Source) && Equals(Target, other.Target);
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
            unchecked
            {
                return ((Source != null ? Source.GetHashCode() : 0) * 397) ^ (Target != null ? Target.GetHashCode() : 0);
            }
        }

        public static bool operator ==(ModelRelationship left, ModelRelationship right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ModelRelationship left, ModelRelationship right)
        {
            return !Equals(left, right);
        }

        public override string ToString() => $"{Source.DisplayName}--{GetType().Name}-->{Target.DisplayName}";

    }
}
