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
        public ModelRelationshipStereotype Stereotype { get; }

        protected ModelRelationshipBase(ModelItemId id, IModelNode source, IModelNode target, ModelRelationshipStereotype stereotype)
        {
            Id = id;
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Target = target ?? throw new ArgumentNullException(nameof(target));
            Stereotype = stereotype ?? throw new ArgumentNullException(nameof(stereotype));

            ValidateSourceAndTargetTypes(source, target);
        }

        public override string ToString() => $"{Source.Name}--{Stereotype}-->{Target.Name} [{Id}]";

        /// <summary>
        /// Evaluates whether a given node participates in this relationship with the given type and direction.
        /// </summary>
        /// <param name="modelNode">A model node.</param>
        /// <param name="directedModelRelationshipType">A relationship type with direction.</param>
        /// <returns>True if the given node participates in this relationship on the side specified by the given direction.</returns>
        public bool IsNodeInRelationship(IModelNode modelNode, DirectedModelRelationshipType directedModelRelationshipType)
        {
            if (Stereotype != directedModelRelationshipType.Stereotype)
                return false;

            switch (directedModelRelationshipType.Direction)
            {
                case RelationshipDirection.Outgoing: return modelNode.Equals(Source);
                case RelationshipDirection.Incoming: return modelNode.Equals(Target);
                default: throw new ArgumentException($"Unexpected RelationshipDirection: {directedModelRelationshipType.Direction}");
            }
        }

        /// <summary>
        /// Returns all valid source node type - target node type pairs.
        /// </summary>
        /// <returns>A collection of valid source and target node type pairs.</returns>
        protected abstract IEnumerable<(ModelNodeStereotype, ModelNodeStereotype)> GetValidSourceAndTargetNodeTypePairs();

        private void ValidateSourceAndTargetTypes(IModelNode source, IModelNode target)
        {
            if (!GetValidSourceAndTargetNodeTypePairs().Contains((source.Stereotype, target.Stereotype)))
                throw new ArgumentException($"{source.Stereotype} and {target.Stereotype} can not be in {Stereotype} relationship.");
        }
    }
}
