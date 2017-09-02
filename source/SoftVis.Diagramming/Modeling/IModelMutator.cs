using System;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// Keeps track of the latest model instance through mutated instances and publishes change events.
    /// </summary>
    public interface IModelMutator
    {
        IModel Model { get; }

        event Action<ModelEventBase> ModelChanged;

        void AddNode(IModelNode node, IModelNode parentNode = null);
        void RemoveNode(IModelNode node);
        void UpdateNode(IModelNode oldNode, IModelNode newNode);
        void AddRelationship(IModelRelationship relationship);
        void RemoveRelationship(IModelRelationship relationship);
        void ClearModel();
    }
}
