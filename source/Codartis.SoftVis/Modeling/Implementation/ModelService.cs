using System;
using System.Linq;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Modeling.Definition.Events;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Modeling.Implementation
{
    /// <summary>
    /// Implements model-related operations.
    /// </summary>
    /// <remarks>
    /// Mutators must not run concurrently. A lock ensures it.
    /// Events are raised after the lock was released to avoid potential deadlocks.
    /// </remarks>
    public sealed class ModelService : IModelService
    {
        public IModel LatestModel { get; private set; }

        [NotNull] private readonly object _modelUpdateLockObject;

        public event Action<ModelEvent> ModelChanged;

        public ModelService([NotNull] params IModelRuleProvider[] modelRuleProviders)
        {
            LatestModel = Model.Create(modelRuleProviders);
            _modelUpdateLockObject = new object();
        }

        public IModelNode AddNode(
            string name,
            ModelNodeStereotype stereotype,
            object payload = null,
            ModelNodeId? parentNodeId = null)
        {
            var modelEvent = MutateWithLockThenRaiseEvents(() => LatestModel.AddNode(name, stereotype, payload, parentNodeId));
            return modelEvent.ItemEvents.OfType<ModelNodeAddedEvent>().First().AddedNode;
        }

        public void RemoveNode(ModelNodeId nodeId)
        {
            MutateWithLockThenRaiseEvents(() => LatestModel.RemoveNode(nodeId));
        }

        public IModelRelationship AddRelationship(
            ModelNodeId sourceId,
            ModelNodeId targetId,
            ModelRelationshipStereotype stereotype,
            object payload = null)
        {
            var modelEvent = MutateWithLockThenRaiseEvents(() => LatestModel.AddRelationship(sourceId, targetId, stereotype, payload));
            return modelEvent.ItemEvents.OfType<ModelRelationshipAddedEvent>().First().AddedRelationship;
        }

        public void RemoveRelationship(ModelRelationshipId relationshipId)
        {
            MutateWithLockThenRaiseEvents(() => LatestModel.RemoveRelationship(relationshipId));
        }

        public void ClearModel()
        {
            MutateWithLockThenRaiseEvents(() => LatestModel.Clear());
        }

        private ModelEvent MutateWithLockThenRaiseEvents([NotNull] Func<ModelEvent> modelMutatorFunc)
        {
            ModelEvent modelEvent;

            lock (_modelUpdateLockObject)
            {
                modelEvent = modelMutatorFunc.Invoke();
                LatestModel = modelEvent.NewModel;
            }

            ModelChanged?.Invoke(modelEvent);

            return modelEvent;
        }
    }
}