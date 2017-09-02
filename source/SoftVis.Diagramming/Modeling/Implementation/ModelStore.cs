using System;
using Codartis.SoftVis.Modeling.Events;

namespace Codartis.SoftVis.Modeling.Implementation
{
    /// <summary>
    /// Implements a model mutator.
    /// Keeps a current model version, implements mutator operations and publishes change events.
    /// The underlying model is immutable so each modification creates a new snapshot of the model.
    /// </summary>
    /// <remarks>
    /// Mutators must not run concurrently. A lock ensures it.
    /// Descendants must implement their mutators using the same lock object.
    /// </remarks>
    public class ModelStore : IModelMutator
    {
        public IModel Model { get; protected set; }

        protected readonly object ModelUpdateLockObject = new object();

        public event Action<ModelEventBase> ModelChanged;

        public ModelStore(IModel model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public void AddNode(IModelNode node, IModelNode parentNode = null)
        {
            lock (ModelUpdateLockObject)
            {
                Model = Model.AddNode(node);
                ModelChanged?.Invoke(new ModelNodeAddedEvent(Model, node));
            }
        }

        public void RemoveNode(IModelNode node)
        {
            lock (ModelUpdateLockObject)
            {
                Model = Model.RemoveNode(node);
                ModelChanged?.Invoke(new ModelNodeRemovedEvent(Model, node));
            }
        }

        public void UpdateNode(IModelNode oldNode, IModelNode newNode)
        {
            lock (ModelUpdateLockObject)
            {
                Model = Model.ReplaceNode(oldNode, newNode);
                ModelChanged?.Invoke(new ModelNodeUpdatedEvent(Model, oldNode, newNode));
            }
        }

        public void AddRelationship(IModelRelationship relationship)
        {
            lock (ModelUpdateLockObject)
            {
                Model = Model.AddRelationship(relationship);
                ModelChanged?.Invoke(new ModelRelationshipAddedEvent(Model, relationship));
            }
        }

        public void RemoveRelationship(IModelRelationship relationship)
        {
            lock (ModelUpdateLockObject)
            {
                Model = Model.RemoveRelationship(relationship);
                ModelChanged?.Invoke(new ModelRelationshipRemovedEvent(Model, relationship));
            }
        }

        public void ClearModel()
        {
            lock (ModelUpdateLockObject)
            {
                Model = Model.Clear();
                ModelChanged?.Invoke(new ModelClearedEvent(Model));
            }
        }
    }
}
