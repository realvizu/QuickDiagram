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

        protected bool Equals(ModelRelationship other)
        {
            return Equals(Source, other.Source) && Equals(Target, other.Target) && Type == other.Type;
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
                var hashCode = (Source != null ? Source.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Target != null ? Target.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (int) Type;
                return hashCode;
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
    }
}
