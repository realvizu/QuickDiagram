using System;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Keeps track of the latest model instance through mutated instances and publishes change events.
    /// </summary>
    public interface IModelStore
    {
        IModel Model { get; }

        event Action<ModelEventBase> ModelChanged;

        void AddNode(IModelNode node);
        void RemoveNode(ModelNodeId nodeId);
        void UpdateNode(IModelNode newNode);
        void AddRelationship(IModelRelationship relationship);
        void RemoveRelationship(ModelRelationshipId relationshipId);
        void ClearModel();
    }
}
