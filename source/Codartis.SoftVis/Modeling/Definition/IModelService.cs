using System;

namespace Codartis.SoftVis.Modeling.Definition
{
    /// <summary>
    /// Defines model-related operations.
    /// </summary>
    public interface IModelService
    {
        IModel Model { get; }

        event Action<ModelEventBase> ModelChanged;

        void AddNode(IModelNode node, IModelNode parentNode = null);
        void RemoveNode(ModelNodeId nodeId);
        void UpdateNode(IModelNode newNode);
        void AddRelationship(IModelRelationship relationship);
        // Note that relationships cannot be updated just removed+added.
        void RemoveRelationship(ModelRelationshipId relationshipId);
        void ClearModel();
    }
}
