using System;

namespace Codartis.SoftVis.Modeling2
{
    /// <summary>
    /// Identifies a model item throughout its lifetime.
    /// </summary>
    public struct ModelItemId : IEquatable<ModelItemId>
    {
        private readonly Guid _value;

        private ModelItemId(Guid guid)
        {
            _value = guid;
        }

        public static ModelItemId Create() => new ModelItemId(Guid.NewGuid());

        public override string ToString() => _value.ToString();

        public bool Equals(ModelItemId other)
        {
            return _value.Equals(other._value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ModelItemId && Equals((ModelItemId) obj);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static bool operator ==(ModelItemId left, ModelItemId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ModelItemId left, ModelItemId right)
        {
            return !left.Equals(right);
        }
    }
}
