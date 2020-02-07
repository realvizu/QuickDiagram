using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
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
    public sealed class DiagramService : IDiagramService
    {
        private const double DefaultChildrenAreaPadding = 1;

        public IDiagram LatestDiagram { get; private set; }

        [NotNull] private readonly IConnectorTypeResolver _connectorTypeResolver;
        private readonly double _childrenAreaPadding;
        [NotNull] private readonly object _diagramUpdateLockObject;
        [NotNull] private readonly ISubject<DiagramEvent> _diagramChangedEventStream;

        public event Action<DiagramEvent> DiagramChanged;
        public event Action<DiagramEvent> AfterDiagramChanged;

        public DiagramService(
            [NotNull] IDiagram diagram,
            [NotNull] IConnectorTypeResolver connectorTypeResolver,
            double childrenAreaPadding = DefaultChildrenAreaPadding)
        {
            LatestDiagram = diagram;
            _connectorTypeResolver = connectorTypeResolver;
            _childrenAreaPadding = childrenAreaPadding;
            _diagramUpdateLockObject = new object();
            _diagramChangedEventStream = new Subject<DiagramEvent>();
        }

        public DiagramService(
            [NotNull] IModel model,
            [NotNull] IConnectorTypeResolver connectorTypeResolver,
            double childrenAreaPadding = DefaultChildrenAreaPadding)
            : this(ImmutableDiagram.Create(model), connectorTypeResolver, childrenAreaPadding)
        {
        }

        public IObservable<DiagramEvent> DiagramChangedEventStream => _diagramChangedEventStream;

        public void AddNode(ModelNodeId nodeId, ModelNodeId? parentNodeId = null)
        {
            MutateWithLockThenRaiseEvents(diagramMutator => diagramMutator.AddNode(nodeId, parentNodeId));

            // Temp
            foreach (var relatedNode in LatestDiagram.Model.GetRelatedNodes(nodeId, CommonDirectedModelRelationshipTypes.Contained))
                AddNode(relatedNode.Id, nodeId);
        }

        public void UpdateNodeHeaderSize(ModelNodeId nodeId, Size2D newSize)
        {
            MutateWithLockThenRaiseEvents(diagramMutator => diagramMutator.UpdateNodeHeaderSize(nodeId, newSize));
        }

        public void UpdateNodeCenter(ModelNodeId nodeId, Point2D newCenter)
        {
            MutateWithLockThenRaiseEvents(diagramMutator => diagramMutator.UpdateNodeCenter(nodeId, newCenter));
        }

        public void UpdateNodeTopLeft(ModelNodeId nodeId, Point2D newTopLeft)
        {
            MutateWithLockThenRaiseEvents(diagramMutator => diagramMutator.UpdateNodeTopLeft(nodeId, newTopLeft));
        }

        public void RemoveNode(ModelNodeId nodeId)
        {
            MutateWithLockThenRaiseEvents(diagramMutator => diagramMutator.RemoveNode(nodeId));
        }

        public void AddConnector(ModelRelationshipId relationshipId)
        {
            MutateWithLockThenRaiseEvents(diagramMutator => diagramMutator.AddConnector(relationshipId));
        }

        public void UpdateConnectorRoute(ModelRelationshipId relationshipId, Route newRoute)
        {
            MutateWithLockThenRaiseEvents(diagramMutator => diagramMutator.UpdateConnectorRoute(relationshipId, newRoute));
        }

        public void RemoveConnector(ModelRelationshipId relationshipId)
        {
            MutateWithLockThenRaiseEvents(diagramMutator => diagramMutator.RemoveConnector(relationshipId));
        }

        public void UpdateModel(IModel model)
        {
            MutateWithLockThenRaiseEvents(diagramMutator => diagramMutator.UpdateModel(model));
        }

        public void UpdateModelNode(IModelNode updatedModelNode)
        {
            MutateWithLockThenRaiseEvents(diagramMutator => diagramMutator.UpdateModelNode(updatedModelNode));
        }

        public void ApplyLayout(LayoutInfo layoutInfo)
        {
            MutateWithLockThenRaiseEvents(diagramMutator => diagramMutator.ApplyLayout(layoutInfo));
        }

        public void Clear()
        {
            MutateWithLockThenRaiseEvents(diagramMutator => diagramMutator.Clear());
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

        private void MutateWithLockThenRaiseEvents([NotNull] Action<IDiagramMutator> diagramMutatorFunc)
        {
            DiagramEvent diagramEvent;

            lock (_diagramUpdateLockObject)
            {
                var diagramMutator = CreateDiagramMutator();
                diagramMutatorFunc.Invoke(diagramMutator);
                diagramEvent = diagramMutator.GetDiagramEvent();
                LatestDiagram = diagramEvent.NewDiagram;
            }

            if (diagramEvent.IsEmpty)
                return;

            DiagramChanged?.Invoke(diagramEvent);
            _diagramChangedEventStream.OnNext(diagramEvent);
            AfterDiagramChanged?.Invoke(diagramEvent);
        }

        [NotNull]
        private DiagramMutator CreateDiagramMutator()
        {
            return new DiagramMutator(LatestDiagram, _connectorTypeResolver, _childrenAreaPadding);
        }
    }
}