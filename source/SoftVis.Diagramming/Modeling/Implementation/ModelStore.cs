using System;
using Codartis.SoftVis.Modeling.Events;

namespace Codartis.SoftVis.Modeling.Implementation
{
    /// <summary>
    /// Implements a model store.
    /// Keeps a current model version, implements mutator operations and publishes change events.
    /// The underlying model is immutable so each modification creates a new snapshot of the model.
    /// </summary>
    /// <remarks>
    /// Mutators must not run concurrently. A lock ensures it.
    /// Descendants must implement their mutators using the same lock object.
    /// </remarks>
    public class ModelStore : IModelStore
    {
        public IModel CurrentModel { get; protected set; }

        protected readonly object ModelUpdateLockObject = new object();

        public event Action<ModelEventBase> ModelChanged;

        public ModelStore(Model model)
        {
            CurrentModel = model ?? throw new ArgumentNullException(nameof(model));
        }

        public void AddNode(IModelNode node, IModelNode parentNode = null)
        {
            lock (ModelUpdateLockObject)
            {
                CurrentModel = CurrentModel.AddNode(node);
                ModelChanged?.Invoke(new ModelNodeAddedEvent(CurrentModel, node));
            }
        }

        public void RemoveNode(IModelNode node)
        {
            lock (ModelUpdateLockObject)
            {
                CurrentModel = CurrentModel.RemoveNode(node);
                ModelChanged?.Invoke(new ModelNodeRemovedEvent(CurrentModel, node));
            }
        }

        public void UpdateNode(IModelNode oldNode, IModelNode newNode)
        {
            lock (ModelUpdateLockObject)
            {
                CurrentModel = CurrentModel.ReplaceNode(oldNode, newNode);
                ModelChanged?.Invoke(new ModelNodeUpdatedEvent(CurrentModel, oldNode, newNode));
            }
        }

        public void AddRelationship(IModelRelationship relationship)
        {
            lock (ModelUpdateLockObject)
            {
                CurrentModel = CurrentModel.AddRelationship(relationship);
                ModelChanged?.Invoke(new ModelRelationshipAddedEvent(CurrentModel, relationship));
            }
        }

        public void RemoveRelationship(IModelRelationship relationship)
        {
            lock (ModelUpdateLockObject)
            {
                CurrentModel = CurrentModel.RemoveRelationship(relationship);
                ModelChanged?.Invoke(new ModelRelationshipRemovedEvent(CurrentModel, relationship));
            }
        }

        public void ClearModel()
        {
            lock (ModelUpdateLockObject)
            {
                CurrentModel = CurrentModel.Clear();
                ModelChanged?.Invoke(new ModelClearedEvent(CurrentModel));
            }
        }
    }
}
