using System.Collections.Generic;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Modeling.Implementation
{
    /// <summary>
    /// Implements an immutable model relationships.
    /// Validates the type of the source and target nodes.
    /// </summary>
    /// <remarks>
    /// The Id field is used to track identity through mutated instances so its value must be kept unchanged by all mutators.
    /// </remarks>
    public sealed class ModelRelationship : IModelRelationship
    {
        public static IEqualityComparer<IModelRelationship> IdComparer { get; } = new IdEqualityComparer();

        public ModelRelationshipId Id { get; }
        public ModelNodeId Source { get; }
        public ModelNodeId Target { get; }
        public ModelRelationshipStereotype Stereotype { get; }
        public object Payload { get; }

        public ModelRelationship(
            ModelRelationshipId id,
            ModelNodeId source,
            ModelNodeId target,
            ModelRelationshipStereotype stereotype,
            [CanBeNull] object payload = null)
        {
            Id = id;
            Source = source;
            Target = target;
            Stereotype = stereotype;
            Payload = payload;
        }

        public IModelRelationship WithPayload(object newPayload) => CreateInstance(Id, Source, Target, Stereotype, newPayload);

        public override string ToString() => $"{Source}--{Stereotype}-->{Target} [{Id}]";

        [NotNull]
        private static ModelRelationship CreateInstance(
            ModelRelationshipId id,
            ModelNodeId source,
            ModelNodeId target,
            ModelRelationshipStereotype stereotype,
            [CanBeNull] object payload)
            => new ModelRelationship(id, source, target, stereotype, payload);

        private sealed class IdEqualityComparer : IEqualityComparer<IModelRelationship>
        {
            public bool Equals(IModelRelationship x, IModelRelationship y)
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

            public int GetHashCode(IModelRelationship obj)
            {
                return obj.Id.GetHashCode();
            }
        }
    }
}