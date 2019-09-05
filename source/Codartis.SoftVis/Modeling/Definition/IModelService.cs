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
        void AddNode([NotNull] IModelNode node, ModelNodeId? parentNodeId = null);

        void UpdateNode([NotNull] IModelNode newNode);
        void RemoveNode(ModelNodeId nodeId);

        void AddRelationship([NotNull] IModelRelationship relationship);
        void RemoveRelationship(ModelRelationshipId relationshipId);

        // Note that relationships cannot be updated just removed+added.

        void ClearModel();
    }
}