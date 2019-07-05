using System;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Identifies a model relationship through its lifetime.
    /// </summary>
    [Immutable]
    public struct ModelRelationshipId : IEquatable<ModelRelationshipId>, IComparable<ModelRelationshipId>
    {
        private readonly Guid _id;

        public ModelRelationshipId(Guid id)
        {
            _id = id;
        }

        public static ModelRelationshipId Create() => new ModelRelationshipId(Guid.NewGuid());

        public override string ToString() => $"{GetType().Name}({_id})";

        public bool Equals(ModelRelationshipId other)
        {
            return _id.Equals(other._id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ModelRelationshipId && Equals((ModelRelationshipId)obj);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public static bool operator ==(ModelRelationshipId left, ModelRelationshipId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ModelRelationshipId left, ModelRelationshipId right)
        {
            return !left.Equals(right);
        }

        public int CompareTo(ModelRelationshipId other)
        {
            return _id.CompareTo(other._id);
        }
    }
}
