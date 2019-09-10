using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Codartis.SoftVis.Diagramming.Events;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// Implements diagram-related operations.
    /// </summary>
    /// <remarks>
    /// Mutators must not run concurrently. A lock ensures it.
    /// Events are raised after the lock was released to avoid potential deadlocks.
    /// </remarks>
    public class DiagramService : IDiagramService
    {
        public IDiagram LatestDiagram { get; private set; }

        [NotNull] private readonly object _diagramUpdateLockObject;
        [NotNull] private readonly IConnectorTypeResolver _connectorTypeResolver;

        public event Action<DiagramEventBase> DiagramChanged;

        public DiagramService([NotNull] IModel model, [NotNull] IConnectorTypeResolver connectorTypeResolver)
        {
            LatestDiagram = Diagram.Create(model);
            _diagramUpdateLockObject = new object();
            _connectorTypeResolver = connectorTypeResolver;
        }

        public void AddNode(ModelNodeId nodeId, ModelNodeId? parentNodeId = null)
        {
            MutateWithLockThenRaiseEvents(() => AddNodeCore(nodeId, parentNodeId));
        }

        public void RemoveNode(ModelNodeId nodeId)
        {
            MutateWithLockThenRaiseEvents(() => RemoveNodeCore(nodeId));
        }

        public void AddConnector(ModelRelationshipId relationshipId)
        {
            MutateWithLockThenRaiseEvents(() => AddConnectorCore(relationshipId));
        }

        public void RemoveConnector(ModelRelationshipId relationshipId)
        {
            MutateWithLockThenRaiseEvents(() => RemoveConnectorCore(relationshipId));
        }

        public void UpdateModel(IModel model)
        {
            MutateWithLockThenRaiseEvents(() => UpdateModelCore(model));
        }

        public void UpdateModelNode(IModelNode updatedModelNode)
        {
            MutateWithLockThenRaiseEvents(() => UpdateModelNodeCore(updatedModelNode));
        }

        public void UpdateDiagramNodeSize(ModelNodeId nodeId, Size2D newSize)
        {
            MutateWithLockThenRaiseEvents(() => UpdateDiagramNodeSizeCore(nodeId, newSize));
        }

        public void UpdateDiagramNodeCenter(ModelNodeId nodeId, Point2D newCenter)
        {
            MutateWithLockThenRaiseEvents(() => UpdateDiagramNodeCenterCore(nodeId, newCenter));
        }

        public void UpdateDiagramNodeTopLeft(ModelNodeId nodeId, Point2D newTopLeft)
        {
            MutateWithLockThenRaiseEvents(() => UpdateDiagramNodeTopLeftCore(nodeId, newTopLeft));
        }

        public void UpdateConnectorRoute(ModelRelationshipId relationshipId, Route newRoute)
        {
            MutateWithLockThenRaiseEvents(() => UpdateConnectorRouteCore(relationshipId, newRoute));
        }

        public void ClearDiagram()
        {
            MutateWithLockThenRaiseEvents(ClearDiagramCore);
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<DiagramEventBase> AddNodeCore(ModelNodeId nodeId, ModelNodeId? parentNodeId)
        {
            var maybeDiagramNode = LatestDiagram.TryGetNode(nodeId);
            if (maybeDiagramNode.HasValue)
                yield break;

            var maybeModelNode = LatestDiagram.Model.TryGetNode(nodeId);
            if (!maybeModelNode.HasValue)
                throw new Exception($"Node {nodeId} not found in model.");

            var newNode = CreateNode(maybeModelNode.Value).WithParentNodeId(parentNodeId.ToMaybe());
            LatestDiagram = LatestDiagram.AddNode(newNode);
            yield return new DiagramNodeAddedEvent(LatestDiagram, newNode);
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<DiagramEventBase> RemoveNodeCore(ModelNodeId nodeId)
        {
            if (!LatestDiagram.NodeExists(nodeId))
                yield break;

            var oldNode = LatestDiagram.GetNode(nodeId);
            LatestDiagram = LatestDiagram.RemoveNode(nodeId);
            yield return new DiagramNodeRemovedEvent(LatestDiagram, oldNode);
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<DiagramEventBase> UpdateModelCore([NotNull] IModel model)
        {
            LatestDiagram = LatestDiagram.WithModel(model);
            yield return new DiagramModelUpdatedEvent(LatestDiagram);

            // TODO: remove those shapes whose model item no longer exists
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<DiagramEventBase> UpdateModelNodeCore([NotNull] IModelNode updatedModelNode)
        {
            var maybeDiagramNode = LatestDiagram.TryGetNode(updatedModelNode.Id);
            if (!maybeDiagramNode.HasValue)
                throw new InvalidOperationException($"Node {updatedModelNode} does not exist.");

            var oldNode = maybeDiagramNode.Value;
            var updatedNode = oldNode.WithModelNode(updatedModelNode);
            LatestDiagram = LatestDiagram.UpdateNode(updatedNode);
            yield return new DiagramNodeModelNodeChangedEvent(LatestDiagram, oldNode, updatedNode);
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<DiagramEventBase> UpdateDiagramNodeSizeCore(ModelNodeId nodeId, Size2D newSize)
        {
            var maybeDiagramNode = LatestDiagram.TryGetNode(nodeId);
            if (!maybeDiagramNode.HasValue)
                throw new InvalidOperationException($"Trying to resize node {nodeId} but it does not exist.");

            var oldNode = maybeDiagramNode.Value;
            var updatedNode = oldNode.WithSize(newSize);
            LatestDiagram = LatestDiagram.UpdateNode(updatedNode);
            yield return new DiagramNodeSizeChangedEvent(LatestDiagram, oldNode, updatedNode);
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<DiagramEventBase> UpdateDiagramNodeCenterCore(ModelNodeId nodeId, Point2D newCenter)
        {
            var maybeDiagramNode = LatestDiagram.TryGetNode(nodeId);
            if (!maybeDiagramNode.HasValue)
                throw new InvalidOperationException($"Trying to move node {nodeId} but it does not exist.");

            var oldNode = maybeDiagramNode.Value;
            var updatedNode = oldNode.WithCenter(newCenter);
            LatestDiagram = LatestDiagram.UpdateNode(updatedNode);
            yield return new DiagramNodePositionChangedEvent(LatestDiagram, oldNode, updatedNode);
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<DiagramEventBase> UpdateDiagramNodeTopLeftCore(ModelNodeId nodeId, Point2D newTopLeft)
        {
            var maybeDiagramNode = LatestDiagram.TryGetNode(nodeId);
            if (!maybeDiagramNode.HasValue)
                throw new InvalidOperationException($"Trying to move node {nodeId} but it does not exist.");

            var oldNode = maybeDiagramNode.Value;
            var updatedNode = oldNode.WithTopLeft(newTopLeft);
            LatestDiagram = LatestDiagram.UpdateNode(updatedNode);
            yield return new DiagramNodePositionChangedEvent(LatestDiagram, oldNode, updatedNode);
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<DiagramEventBase> AddConnectorCore(ModelRelationshipId relationshipId)
        {
            var maybeConnector = LatestDiagram.TryGetConnector(relationshipId);
            if (maybeConnector.HasValue)
                yield break;

            var maybeRelationship = LatestDiagram.Model.TryGetRelationship(relationshipId);
            if (!maybeRelationship.HasValue)
                throw new InvalidOperationException($"Relationship {relationshipId} does not exist.");

            var connector = CreateConnector(maybeRelationship.Value);
            LatestDiagram = LatestDiagram.AddConnector(connector);
            yield return new DiagramConnectorAddedEvent(LatestDiagram, connector);
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<DiagramEventBase> RemoveConnectorCore(ModelRelationshipId relationshipId)
        {
            if (!LatestDiagram.ConnectorExists(relationshipId))
                yield break;

            var oldConnector = LatestDiagram.GetConnector(relationshipId);
            LatestDiagram = LatestDiagram.RemoveConnector(relationshipId);
            yield return new DiagramConnectorRemovedEvent(LatestDiagram, oldConnector);
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<DiagramEventBase> UpdateConnectorRouteCore(ModelRelationshipId relationshipId, Route newRoute)
        {
            var maybeDiagramConnector = LatestDiagram.TryGetConnector(relationshipId);
            if (!maybeDiagramConnector.HasValue)
                throw new InvalidOperationException($"Connector {relationshipId} does not exist.");

            var oldConnector = maybeDiagramConnector.Value;
            var newConnector = oldConnector.WithRoute(newRoute);
            LatestDiagram = LatestDiagram.UpdateConnector(newConnector);
            yield return new DiagramConnectorRouteChangedEvent(LatestDiagram, oldConnector, newConnector);
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<DiagramEventBase> ClearDiagramCore()
        {
            LatestDiagram = LatestDiagram.Clear();
            yield return new DiagramClearedEvent(LatestDiagram);
        }

        private void MutateWithLockThenRaiseEvents([NotNull] Func<IEnumerable<DiagramEventBase>> diagramMutatorFunc)
        {
            IList<DiagramEventBase> events;
            lock (_diagramUpdateLockObject)
            {
                // Using ToList to force evaluation to be inside the lock block.
                events = diagramMutatorFunc.Invoke().EmptyIfNull().ToList();
            }

            RaiseEvents(events);
        }

        private void RaiseEvents([NotNull] [ItemNotNull] IEnumerable<DiagramEventBase> events)
        {
            foreach (var @event in events)
                DiagramChanged?.Invoke(@event);
        }

        public ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype)
        {
            return _connectorTypeResolver.GetConnectorType(stereotype);
        }

        public void AddNodes(
            IEnumerable<ModelNodeId> modelNodeIds,
            CancellationToken cancellationToken,
            IIncrementalProgress progress)
        {
            foreach (var modelNodeId in modelNodeIds)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var parentNodeId = GetParentDiagramNodeId(modelNodeId);
                AddNode(modelNodeId, parentNodeId);

                progress?.Report(1);
            }
        }

        private ModelNodeId? GetParentDiagramNodeId(ModelNodeId modelNodeId)
        {
            var containerNodes = LatestDiagram.Model
                .GetRelatedNodes(modelNodeId, CommonDirectedModelRelationshipTypes.Container, recursive: false)
                .ToList();

            if (!containerNodes.Any())
                return null;

            if (containerNodes.Count > 1)
                throw new Exception($"{modelNodeId} has more than 1 containers.");

            var potentialContainerNode = containerNodes.First();
            if (LatestDiagram.NodeExists(potentialContainerNode.Id))
                return potentialContainerNode.Id;

            return null;
        }

        [NotNull]
        private static IDiagramNode CreateNode([NotNull] IModelNode modelNode) => new DiagramNode(modelNode);

        [NotNull]
        private IDiagramConnector CreateConnector([NotNull] IModelRelationship relationship)
        {
            var connectorType = _connectorTypeResolver.GetConnectorType(relationship.Stereotype);
            return new DiagramConnector(relationship, connectorType);
        }
    }
}