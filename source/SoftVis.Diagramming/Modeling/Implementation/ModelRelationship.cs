using System;
using System.Diagnostics;
using QuickGraph;

namespace Codartis.SoftVis.Modeling.Implementation
{
    /// <summary>
    /// An implementation of the IModelRelationship interface with a QuickGraph edge.
    /// </summary>
    [DebuggerDisplay("{Source.Name}--{Classifier}/{Stereotype}-->{Target.Name}")]
    public class ModelRelationship : IModelRelationship, IEdge<IModelEntity>
    {
        public IModelEntity Source { get; }
        public IModelEntity Target { get; }
        public ModelRelationshipClassifier Classifier { get; }
        public ModelRelationshipStereotype Stereotype { get; }

        public ModelRelationshipType Type => new ModelRelationshipType(Classifier, Stereotype);

        public ModelRelationship(IModelEntity source, IModelEntity target, ModelRelationshipType type)
            : this(source, target, type.Classifier, type.Stereotype)
        { }

        public ModelRelationship(IModelEntity source, IModelEntity target,
            ModelRelationshipClassifier classifier, ModelRelationshipStereotype stereotype)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (target == null) throw new ArgumentNullException(nameof(target));

            Source = source;
            Target = target;
            Classifier = classifier;
            Stereotype = stereotype;
        }

        public bool IsEntityInRelationship(IModelEntity entity, EntityRelationDirection direction)
        {
            return (direction == EntityRelationDirection.Outgoing && entity == Source)
                || (direction == EntityRelationDirection.Incoming && entity == Target);
        }

        public bool Equals(IModelRelationship other)
        {
            return Equals((object)other);
        }

        protected bool Equals(ModelRelationship other)
        {
            return Source.Equals(other.Source)
                && Target.Equals(other.Target)
                && Classifier == other.Classifier
                && Stereotype.Equals(other.Stereotype);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ModelRelationship)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Source.GetHashCode();
                hashCode = (hashCode * 397) ^ Target.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)Classifier;
                hashCode = (hashCode * 397) ^ Stereotype.GetHashCode();
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

        public override string ToString() => $"{Source.Name}--{Classifier}/{Stereotype}-->{Target.Name}";
    }
}
