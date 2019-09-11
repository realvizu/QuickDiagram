using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Modeling.Definition.Events;
using Codartis.Util;
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
        [NotNull] [ItemNotNull] private readonly IModelRuleProvider[] _modelRuleProviders;

        public event Action<ModelEventBase> ModelChanged;

        public ModelService([NotNull] params IModelRuleProvider[] modelRuleProviders)
        {
            LatestModel = Model.Empty;
            _modelUpdateLockObject = new object();
            _modelRuleProviders = modelRuleProviders;
        }

        public IModelNode AddNode(
            string name,
            ModelNodeStereotype? stereotype = null,
            object payload = null,
            ModelNodeId? parentNodeId = null)
        {
            var newNode = CreateNode(name, stereotype ?? ModelNodeStereotype.Default, payload);
            MutateWithLockThenRaiseEvents(() => AddNodeCore(newNode, parentNodeId));
            return newNode;
        }

        public void UpdateNode(IModelNode newNode)
        {
            MutateWithLockThenRaiseEvents(() => UpdateNodeCore(newNode));
        }

        public void RemoveNode(ModelNodeId nodeId)
        {
            MutateWithLockThenRaiseEvents(() => RemoveNodeCore(nodeId));
        }

        public IModelRelationship AddRelationship(
            ModelNodeId sourceId,
            ModelNodeId targetId,
            ModelRelationshipStereotype? stereotype = null,
            object payload = null)
        {
            var newRelationship = CreateRelationship(sourceId, targetId, stereotype ?? ModelRelationshipStereotype.Default, payload);
            MutateWithLockThenRaiseEvents(() => AddRelationshipCore(newRelationship));
            return newRelationship;
        }

        public void RemoveRelationship(ModelRelationshipId relationshipId)
        {
            MutateWithLockThenRaiseEvents(() => RemoveRelationshipCore(relationshipId));
        }

        public void ClearModel()
        {
            MutateWithLockThenRaiseEvents(ClearModelCore);
        }

        private void MutateWithLockThenRaiseEvents([NotNull] Func<IEnumerable<ModelEventBase>> modelMutatorFunc)
        {
            IList<ModelEventBase> events;

            lock (_modelUpdateLockObject)
            {
                // Using ToList to force evaluation to be inside the lock block.
                events = modelMutatorFunc.Invoke().EmptyIfNull().ToList();
            }

            RaiseEvents(events);
        }

        private void RaiseEvents([NotNull] [ItemNotNull] IEnumerable<ModelEventBase> events)
        {
            foreach (var @event in events)
                ModelChanged?.Invoke(@event);
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<ModelEventBase> AddNodeCore([NotNull] IModelNode node, ModelNodeId? parentNodeId = null)
        {
            LatestModel = LatestModel.AddNode(node);
            yield return new ModelNodeAddedEvent(LatestModel, node);

            if (!parentNodeId.HasValue)
                yield break;

            var containsRelationship = CreateRelationship(parentNodeId.Value, node.Id, ModelRelationshipStereotype.Containment);

            foreach (var @event in AddRelationshipCore(containsRelationship))
                yield return @event;
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<ModelEventBase> UpdateNodeCore([NotNull] IModelNode newNode)
        {
            var maybeOldNode = LatestModel.TryGetNode(newNode.Id);
            if (!maybeOldNode.HasValue)
                throw new InvalidOperationException($"Node with id {newNode.Id} was not found in the model.");

            LatestModel = LatestModel.UpdateNode(newNode);
            yield return new ModelNodeUpdatedEvent(LatestModel, maybeOldNode.Value, newNode);
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<ModelEventBase> RemoveNodeCore(ModelNodeId nodeId)
        {
            foreach (var @event in LatestModel.GetRelationships(nodeId).SelectMany(i => RemoveRelationshipCore(i.Id)))
                yield return @event;

            var oldNode = LatestModel.GetNode(nodeId);
            LatestModel = LatestModel.RemoveNode(nodeId);
            yield return new ModelNodeRemovedEvent(LatestModel, oldNode);
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<ModelEventBase> AddRelationshipCore([NotNull] IModelRelationship relationship)
        {
            if (!IsRelationshipValid(relationship))
                throw new ArgumentException($"{relationship} is invalid.");

            LatestModel = LatestModel.AddRelationship(relationship);
            yield return new ModelRelationshipAddedEvent(LatestModel, relationship);
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<ModelEventBase> RemoveRelationshipCore(ModelRelationshipId relationshipId)
        {
            var oldRelationship = LatestModel.GetRelationship(relationshipId);
            LatestModel = LatestModel.RemoveRelationship(relationshipId);
            yield return new ModelRelationshipRemovedEvent(LatestModel, oldRelationship);
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<ModelEventBase> ClearModelCore()
        {
            LatestModel = LatestModel.Clear();
            yield return new ModelClearedEvent(LatestModel);
        }

        [NotNull]
        private static IModelNode CreateNode([NotNull] string name, ModelNodeStereotype stereotype, [CanBeNull] object payload)
        {
            return new ModelNode(ModelNodeId.Create(), name, stereotype, payload);
        }

        [NotNull]
        private static IModelRelationship CreateRelationship(
            ModelNodeId sourceId,
            ModelNodeId targetId,
            ModelRelationshipStereotype stereotype,
            object payload = null)
        {
            return new ModelRelationship(ModelRelationshipId.Create(), sourceId, targetId, stereotype, payload);
        }

        private bool IsRelationshipValid([NotNull] IModelRelationship relationship)
        {
            var sourceNode = LatestModel.GetNode(relationship.Source);
            var targetNode = LatestModel.GetNode(relationship.Target);

            return _modelRuleProviders.All(i => i.IsRelationshipStereotypeValid(relationship.Stereotype, sourceNode, targetNode));
        }
    }
}