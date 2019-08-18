using System;
using System.Collections.Generic;
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
    /// Descendants must implement their mutators using the same lock object.
    /// </remarks>
    public class ModelService : IModelService
    {
        public IModel Model { get; protected set; }

        [NotNull] protected readonly object ModelUpdateLockObject;

        public event Action<ModelEventBase> ModelChanged;

        public ModelService()
        {
            Model = Implementation.Model.Empty;
            ModelUpdateLockObject = new object();
        }

        public void AddNode(IModelNode node, ModelNodeId? parentNodeId = null) => RaiseEvents(AddNodeCore(node, parentNodeId));
        public void UpdateNode(IModelNode newNode) => RaiseEvents(UpdateNodeCore(newNode));
        public void RemoveNode(ModelNodeId nodeId) => RaiseEvents(RemoveNodeCore(nodeId));
        public void AddRelationship(IModelRelationship relationship) => RaiseEvents(AddRelationshipCore(relationship));
        public void RemoveRelationship(ModelRelationshipId relationshipId) => RaiseEvents(RemoveRelationshipCore(relationshipId));
        public void ClearModel() => RaiseEvents(ClearModelCore());

        protected void RaiseEvents([NotNull] [ItemNotNull] IEnumerable<ModelEventBase> events)
        {
            // It is important to materialize the collection with ToList() to allow releasing ModelUpdateLockObject as soon as possible.
            foreach (var @event in events.ToList())
                ModelChanged?.Invoke(@event);
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<ModelEventBase> AddNodeCore(IModelNode node, ModelNodeId? parentNodeId = null)
        {
            lock (ModelUpdateLockObject)
            {
                Model = Model.AddNode(node);
                yield return new ModelNodeAddedEvent(Model, node);

                if (!parentNodeId.HasValue)
                    yield break;

                var containsRelationship = CreateContainsRelationship(parentNodeId.Value, node.Id);

                foreach (var @event in AddRelationshipCore(containsRelationship))
                    yield return @event;
            }
        }

        [NotNull]
        private static ModelRelationship CreateContainsRelationship(ModelNodeId source, ModelNodeId target)
        {
            return new ModelRelationship(ModelRelationshipId.Create(), source, target, ModelRelationshipStereotype.Containment);
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<ModelEventBase> UpdateNodeCore([NotNull] IModelNode newNode)
        {
            lock (ModelUpdateLockObject)
            {
                var oldNode = Model.GetNode(newNode.Id);
                Model = Model.ReplaceNode(newNode);
                yield return new ModelNodeUpdatedEvent(Model, oldNode, newNode);
            }
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<ModelEventBase> RemoveNodeCore(ModelNodeId nodeId)
        {
            lock (ModelUpdateLockObject)
            {
                foreach (var @event in Model.GetRelationships(nodeId).SelectMany(i => RemoveRelationshipCore(i.Id)))
                    yield return @event;

                var oldNode = Model.GetNode(nodeId);
                Model = Model.RemoveNode(nodeId);
                yield return new ModelNodeRemovedEvent(Model, oldNode);
            }
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<ModelEventBase> AddRelationshipCore(IModelRelationship relationship)
        {
            lock (ModelUpdateLockObject)
            {
                Model = Model.AddRelationship(relationship);
                yield return new ModelRelationshipAddedEvent(Model, relationship);
            }
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<ModelEventBase> RemoveRelationshipCore(ModelRelationshipId relationshipId)
        {
            lock (ModelUpdateLockObject)
            {
                var oldRelationship = Model.GetRelationship(relationshipId);
                Model = Model.RemoveRelationship(relationshipId);
                yield return new ModelRelationshipRemovedEvent(Model, oldRelationship);
            }
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<ModelEventBase> ClearModelCore()
        {
            lock (ModelUpdateLockObject)
            {
                Model = Model.Clear();
                yield return new ModelClearedEvent(Model);
            }
        }
    }
}