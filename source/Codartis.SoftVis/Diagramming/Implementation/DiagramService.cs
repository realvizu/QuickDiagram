using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Codartis.SoftVis.Diagramming.Definition;
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

        public event Action<DiagramChangedEvent> DiagramChanged;

        public DiagramService([NotNull] IDiagram diagram, [NotNull] IConnectorTypeResolver connectorTypeResolver)
        {
            LatestDiagram = diagram;
            _diagramUpdateLockObject = new object();
            _connectorTypeResolver = connectorTypeResolver;
        }

        public DiagramService([NotNull] IModel model, [NotNull] IConnectorTypeResolver connectorTypeResolver)
            : this(Diagram.Create(model, connectorTypeResolver), connectorTypeResolver)
        {
        }

        public void AddNode(ModelNodeId nodeId, ModelNodeId? parentNodeId = null)
        {
            MutateWithLockThenRaiseEvents(() => LatestDiagram.AddNode(nodeId, parentNodeId));
        }

        public void UpdateNodePayloadAreaSize(ModelNodeId nodeId, Size2D newSize)
        {
            MutateWithLockThenRaiseEvents(() => LatestDiagram.UpdateNodePayloadAreaSize(nodeId, newSize));
        }

        public void UpdateNodeChildrenAreaSize(ModelNodeId nodeId, Size2D newSize)
        {
            MutateWithLockThenRaiseEvents(() => LatestDiagram.UpdateNodeChildrenAreaSize(nodeId, newSize));
        }

        public void UpdateNodeCenter(ModelNodeId nodeId, Point2D newCenter)
        {
            MutateWithLockThenRaiseEvents(() => LatestDiagram.UpdateNodeCenter(nodeId, newCenter));
        }

        public void UpdateNodeTopLeft(ModelNodeId nodeId, Point2D newTopLeft)
        {
            MutateWithLockThenRaiseEvents(() => LatestDiagram.UpdateNodeTopLeft(nodeId, newTopLeft));
        }

        public void RemoveNode(ModelNodeId nodeId)
        {
            MutateWithLockThenRaiseEvents(() => LatestDiagram.RemoveNode(nodeId));
        }

        public void AddConnector(ModelRelationshipId relationshipId)
        {
            MutateWithLockThenRaiseEvents(() => LatestDiagram.AddConnector(relationshipId));
        }

        public void UpdateConnectorRoute(ModelRelationshipId relationshipId, Route newRoute)
        {
            MutateWithLockThenRaiseEvents(() => LatestDiagram.UpdateConnectorRoute(relationshipId, newRoute));
        }

        public void RemoveConnector(ModelRelationshipId relationshipId)
        {
            MutateWithLockThenRaiseEvents(() => LatestDiagram.RemoveConnector(relationshipId));
        }

        public void UpdateModel(IModel model)
        {
            MutateWithLockThenRaiseEvents(() => LatestDiagram.UpdateModel(model));
        }

        public void UpdateModelNode(IModelNode updatedModelNode)
        {
            MutateWithLockThenRaiseEvents(() => LatestDiagram.UpdateModelNode(updatedModelNode));
        }

        public void ApplyLayout(DiagramLayoutInfo diagramLayout)
        {
            MutateWithLockThenRaiseEvents(() => LatestDiagram.ApplyLayout(diagramLayout));
        }

        public void ClearDiagram()
        {
            MutateWithLockThenRaiseEvents(() => LatestDiagram.Clear());
        }

        private void MutateWithLockThenRaiseEvents([NotNull] Func<DiagramChangedEvent> diagramMutator)
        {
            DiagramChangedEvent diagramChangedEvent;

            lock (_diagramUpdateLockObject)
            {
                diagramChangedEvent = diagramMutator.Invoke();
                LatestDiagram = diagramChangedEvent.NewDiagram;
            }

            DiagramChanged?.Invoke(diagramChangedEvent);
        }

        public ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype)
        {
            return _connectorTypeResolver.GetConnectorType(stereotype);
        }

        public void AddNodes(
            IEnumerable<ModelNodeId> modelNodeIds,
            CancellationToken cancellationToken = default,
            IIncrementalProgress progress = null)
        {
            foreach (var modelNodeId in modelNodeIds)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var parentNodeId = GetParentDiagramNodeId(modelNodeId);
                AddNode(modelNodeId, parentNodeId);

                progress?.Report(1);
            }
        }

        public void AddConnectors(
            IEnumerable<ModelRelationshipId> modelRelationshipIds,
            CancellationToken cancellationToken = default,
            IIncrementalProgress progress = null)
        {
            foreach (var modelRelationshipId in modelRelationshipIds)
            {
                cancellationToken.ThrowIfCancellationRequested();

                AddConnector(modelRelationshipId);

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
    }
}