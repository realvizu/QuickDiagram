using System;
using System.Linq;

namespace Codartis.SoftVis.Modeling.Implementation
{
    /// <summary>
    /// Implements model-related operations.
    /// </summary>
    public abstract class ModelService : IModelService
    {
        protected readonly ModelStore ModelStore;

        protected ModelService(ModelStore modelStore)
        {
            ModelStore = modelStore;
        }

        public IModel Model => ModelStore.Model;

        public event Action<ModelEventBase> ModelChanged
        {
            add => ModelStore.ModelChanged += value;
            remove => ModelStore.ModelChanged -= value;
        }

        public void AddNode(IModelNode node, IModelNode parentNode = null) => ModelStore.AddNode(node, parentNode);

        public void RemoveNode(ModelNodeId nodeId)
        {
            var relationshipsToRemove = Model.GetRelationships(nodeId).ToArray();
            foreach (var relationship in relationshipsToRemove)
                RemoveRelationship(relationship.Id);
            
            ModelStore.RemoveNode(nodeId);
        }

        public void UpdateNode(IModelNode newNode) => ModelStore.UpdateNode(newNode);
        public void AddRelationship(IModelRelationship relationship) => ModelStore.AddRelationship(relationship);
        public void RemoveRelationship(ModelRelationshipId relationshipId) => ModelStore.RemoveRelationship(relationshipId);
        public void ClearModel() => ModelStore.ClearModel();
    }
}
