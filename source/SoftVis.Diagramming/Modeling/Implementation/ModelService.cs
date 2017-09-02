using System;

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
        public void RemoveNode(IModelNode node) => ModelStore.RemoveNode(node);
        public void UpdateNode(IModelNode oldNode, IModelNode newNode) => ModelStore.UpdateNode(oldNode, newNode);
        public void AddRelationship(IModelRelationship relationship) => ModelStore.AddRelationship(relationship);
        public void RemoveRelationship(IModelRelationship relationship) => ModelStore.RemoveRelationship(relationship);
        public void ClearModel() => ModelStore.ClearModel();
    }
}
