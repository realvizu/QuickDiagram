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
        public IModel Model { get; private set; }

        [NotNull] private readonly object _modelUpdateLockObject;
        [NotNull] [ItemNotNull] private readonly IModelRuleProvider[] _modelRuleProviders;

        public event Action<ModelEventBase> ModelChanged;

        public ModelService([NotNull] params IModelRuleProvider[] modelRuleProviders)
        {
            Model = Implementation.Model.Empty;
            _modelUpdateLockObject = new object();
            _modelRuleProviders = modelRuleProviders;
        }

        public void AddNode(IModelNode node, ModelNodeId? parentNodeId = null)
        {
            MutateWithLockThenRaiseEvents(() => AddNodeCore(node, parentNodeId));
        }

        public void UpdateNode(IModelNode newNode)
        {
            MutateWithLockThenRaiseEvents(() => UpdateNodeCore(newNode));
        }

        public void RemoveNode(ModelNodeId nodeId)
        {
            MutateWithLockThenRaiseEvents(() => RemoveNodeCore(nodeId));
        }

        public void AddRelationship(IModelRelationship relationship)
        {
            MutateWithLockThenRaiseEvents(() => AddRelationshipCore(relationship));
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

        [NotNull]
        [ItemNotNull]
        private IEnumerable<ModelEventBase> AddNodeCore([NotNull] IModelNode node, ModelNodeId? parentNodeId = null)
        {
            Model = Model.AddNode(node);
            yield return new ModelNodeAddedEvent(Model, node);

            if (!parentNodeId.HasValue)
                yield break;

            var containsRelationship = CreateContainsRelationship(parentNodeId.Value, node.Id);

            foreach (var @event in AddRelationshipCore(containsRelationship))
                yield return @event;
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<ModelEventBase> UpdateNodeCore([NotNull] IModelNode newNode)
        {
            var maybeOldNode = Model.TryGetNode(newNode.Id);
            if (!maybeOldNode.HasValue)
                throw new InvalidOperationException($"Node with id {newNode.Id} was not found in the model.");

            Model = Model.ReplaceNode(newNode);
            yield return new ModelNodeUpdatedEvent(Model, maybeOldNode.Value, newNode);
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<ModelEventBase> RemoveNodeCore(ModelNodeId nodeId)
        {
            foreach (var @event in Model.GetRelationships(nodeId).SelectMany(i => RemoveRelationshipCore(i.Id)))
                yield return @event;

            var oldNode = Model.GetNode(nodeId);
            Model = Model.RemoveNode(nodeId);
            yield return new ModelNodeRemovedEvent(Model, oldNode);
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<ModelEventBase> AddRelationshipCore([NotNull] IModelRelationship relationship)
        {
            if (!IsRelationshipValid(relationship))
                throw new ArgumentException($"{relationship} is invalid.");

            Model = Model.AddRelationship(relationship);
            yield return new ModelRelationshipAddedEvent(Model, relationship);
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<ModelEventBase> RemoveRelationshipCore(ModelRelationshipId relationshipId)
        {
            var oldRelationship = Model.GetRelationship(relationshipId);
            Model = Model.RemoveRelationship(relationshipId);
            yield return new ModelRelationshipRemovedEvent(Model, oldRelationship);
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<ModelEventBase> ClearModelCore()
        {
            Model = Model.Clear();
            yield return new ModelClearedEvent(Model);
        }

        private bool IsRelationshipValid([NotNull] IModelRelationship relationship)
        {
            var sourceNode = Model.GetNode(relationship.Source);
            var targetNode = Model.GetNode(relationship.Target);

            return _modelRuleProviders.All(i => i.IsRelationshipStereotypeValid(relationship.Stereotype, sourceNode, targetNode));
        }

        [NotNull]
        private static ModelRelationship CreateContainsRelationship(ModelNodeId source, ModelNodeId target)
        {
            return new ModelRelationship(ModelRelationshipId.Create(), source, target, ModelRelationshipStereotype.Containment);
        }

        private void RaiseEvents([NotNull] [ItemNotNull] IEnumerable<ModelEventBase> events)
        {
            foreach (var @event in events)
                ModelChanged?.Invoke(@event);
        }
    }
}