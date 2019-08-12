using System;
using System.Linq;
using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.Modeling.Implementation
{
    /// <summary>
    /// Implements model-related operations.
    /// </summary>
    public abstract class ModelServiceBase : IModelService
    {
        protected readonly ModelStore ModelStore;
        protected readonly IModelRelationshipFactory ModelRelationshipFactory;

        protected ModelServiceBase(ModelStore modelStore, IModelRelationshipFactory modelRelationshipFactory)
        {
            ModelStore = modelStore;
            ModelRelationshipFactory = modelRelationshipFactory;
        }

        public IModel Model => ModelStore.Model;

        public event Action<ModelEventBase> ModelChanged
        {
            add => ModelStore.ModelChanged += value;
            remove => ModelStore.ModelChanged -= value;
        }

        public void AddNode(IModelNode node, IModelNode parentNode)
        {
            if (!Model.Nodes.Contains(parentNode))
                throw new ArgumentException($"{parentNode} is not found in the model.");

            AddNode(node);

            var containsRelationship = ModelRelationshipFactory.CreateRelationship(parentNode, node, ModelRelationshipStereotype.Containment);

            AddRelationship(containsRelationship);
        }

        public bool TryGetParentNode(ModelNodeId modelNodeId, out IModelNode parentNode)
        {
            var parentNodes = Model.GetRelatedNodes(modelNodeId, CommonDirectedModelRelationshipTypes.Container).ToArray();

            if (parentNodes.Length > 1)
                throw new Exception($"There are {parentNodes.Length} parent nodes for node {modelNodeId}.");

            parentNode = parentNodes.SingleOrDefault();
            return parentNode != null;
        }

        public void AddNode(IModelNode node) => ModelStore.AddNode(node);

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