using System;
using System.Collections.Generic;
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
        [NotNull] private readonly IModelRelationshipFeatureProvider _modelRelationshipFeatureProvider;
        private readonly double _childrenAreaPadding;
        [NotNull] private readonly object _diagramUpdateLockObject;
        [NotNull] private readonly ISubject<DiagramEvent> _diagramChangedEventStream;

        public event Action<DiagramEvent> DiagramChanged;
        public event Action<DiagramEvent> AfterDiagramChanged;

        public DiagramService(
            [NotNull] IDiagram diagram,
            [NotNull] IConnectorTypeResolver connectorTypeResolver,
            [NotNull] IModelRelationshipFeatureProvider modelRelationshipFeatureProvider,
            double childrenAreaPadding = DefaultChildrenAreaPadding)
        {
            LatestDiagram = diagram;
            _connectorTypeResolver = connectorTypeResolver;
            _modelRelationshipFeatureProvider = modelRelationshipFeatureProvider;
            _childrenAreaPadding = childrenAreaPadding;
            _diagramUpdateLockObject = new object();
            _diagramChangedEventStream = new Subject<DiagramEvent>();
        }

        public DiagramService(
            [NotNull] IModel model,
            [NotNull] IConnectorTypeResolver connectorTypeResolver,
            [NotNull] IModelRelationshipFeatureProvider modelRelationshipFeatureProvider,
            double childrenAreaPadding = DefaultChildrenAreaPadding)
            : this(
                ImmutableDiagram.Create(model, modelRelationshipFeatureProvider),
                connectorTypeResolver,
                modelRelationshipFeatureProvider,
                childrenAreaPadding)
        {
        }

        public IObservable<DiagramEvent> DiagramChangedEventStream => _diagramChangedEventStream;

        public void AddNode(ModelNodeId nodeId)
        {
            MutateWithLockThenRaiseEvents(diagramMutator => diagramMutator.AddNode(nodeId));
        }

        public void UpdateParent(ModelNodeId nodeId, ModelNodeId? parentNodeId)
        {
            MutateWithLockThenRaiseEvents(diagramMutator => diagramMutator.UpdateParent(nodeId, parentNodeId));
        }

        public void UpdateSize(ModelNodeId nodeId, Size2D newSize)
        {
            MutateWithLockThenRaiseEvents(diagramMutator => diagramMutator.UpdateSize(nodeId, newSize));
        }

        public void UpdateNodeRelativeTopLeft(ModelNodeId nodeId, Point2D newRelativeTopLeft)
        {
            MutateWithLockThenRaiseEvents(diagramMutator => diagramMutator.UpdateNodeRelativeTopLeft(nodeId, newRelativeTopLeft));
        }

        public void UpdateChildrenAreaTopLeft(ModelNodeId nodeId, Point2D newTopLeft)
        {
            MutateWithLockThenRaiseEvents(diagramMutator => diagramMutator.UpdateChildrenAreaTopLeft(nodeId, newTopLeft));
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

                AddNode(modelNodeId);

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
            return new DiagramMutator(
                LatestDiagram,
                _connectorTypeResolver,
                _modelRelationshipFeatureProvider,
                _childrenAreaPadding);
        }
    }
}