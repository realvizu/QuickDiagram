using System;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Modeling.Definition
{
    /// <summary>
    /// Defines model-related operations.
    /// The underlying model is immutable so each modification creates a new snapshot of the model.
    /// Keeps the latest model version, implements mutator operations and publishes change events.
    /// </summary>
    public interface IModelService
    {
        [NotNull] IModel LatestModel { get; }

        event Action<ModelEventBase> ModelChanged;

        /// <summary>
        /// Adds a node to the model.
        /// If a parentNodeId is specified then also creates a containment relationship.
        /// </summary>
        [NotNull]
        IModelNode AddNode(
            [NotNull] string name,
            ModelNodeStereotype? stereotype = null,
            [CanBeNull] object payload = null,
            ModelNodeId? parentNodeId = null);

        void UpdateNode([NotNull] IModelNode newNode);

        void RemoveNode(ModelNodeId nodeId);

        [NotNull]
        IModelRelationship AddRelationship(
            ModelNodeId sourceId,
            ModelNodeId targetId,
            ModelRelationshipStereotype? stereotype = null,
            [CanBeNull] object payload = null);

        void RemoveRelationship(ModelRelationshipId relationshipId);

        // Note that relationships cannot be updated just removed+added.

        void ClearModel();
    }
}