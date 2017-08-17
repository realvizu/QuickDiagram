using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// A diagram is a partial, graphical representation of a model. 
    /// A diagram shows a subset of the model and there can be many diagrams depicting different areas/aspects of the same model.
    /// A diagram consists of shapes that represent model elements.
    /// The shapes form a directed graph: some shapes are nodes in the graph and others are connectors between nodes.
    /// The layout (relative positions and size) also conveys meaning.
    /// </summary>
    [DebuggerDisplay("NodeCount={_graph.VertexCount}, ConnectorCount={_graph.EdgeCount}")]
    public class Diagram : IArrangedDiagram
    {
        public IModelBuilder ModelBuilder { get; }

        private readonly DiagramBuilderBase _diagramBuilder;
        private readonly DiagramGraph _graph;
        private readonly IDiagramNodeFactory _diagramNodeFactory;

        public event Action<IDiagramShape> ShapeAdded;
        public event Action<IDiagramShape> ShapeRemoved;
        public event Action<IDiagramShape> ShapeSelected;

        public event Action DiagramCleared;

        public event Action<IDiagramNode, Size2D, Size2D> NodeSizeChanged;
        public event Action<IDiagramNode, Point2D, Point2D> NodeCenterChanged;
        public event Action<IDiagramConnector, Route, Route> ConnectorRouteChanged;

        public Diagram(IModelBuilder modelBuilder, DiagramBuilderBase diagramBuilder, IDiagramNodeFactory diagramNodeFactory)
        {
            ModelBuilder = modelBuilder;
            _diagramBuilder = diagramBuilder;
            _diagramNodeFactory = diagramNodeFactory;

            ModelBuilder.NodeUpdated += OnModelNodeUpdated;
            ModelBuilder.NodeRemoved += OnModelNodeRemoved;
            ModelBuilder.RelationshipAdded += OnModelRelationshipAdded;
            ModelBuilder.RelationshipRemoved += OnModelRelationshipRemoved;
            ModelBuilder.ModelCleared += OnModelCleared;

            _graph = new DiagramGraph();
            _graph.VertexAdded += OnDiagramNodeAddedToGraph;
            _graph.EdgeAdded += OnDiagramEdgeAddedToGraph;
            _graph.VertexRemoved += OnDiagramNodeRemovedFromGraph;
            _graph.EdgeRemoved += OnDiagramEdgeRemovedFromGraph;
        }

        public IReadOnlyList<IDiagramNode> Nodes => _graph.Vertices;
        public IReadOnlyList<IDiagramConnector> Connectors => _graph.Edges;
        public IReadOnlyList<IDiagramShape> Shapes => Nodes.OfType<IDiagramShape>().Union(Connectors).ToArray();

        public Rect2D ContentRect => Shapes.Select(i => i.Rect).Where(i => i.IsDefined()).Union();

        /// <summary>
        /// Clear the diagram (that is, hide all nodes and connectors).
        /// </summary>
        public virtual void Clear()
        {
            _graph.Clear();
            OnDiagramCleared();
        }

        public ConnectorType GetConnectorType(ModelRelationshipStereotype modelRelationshipStereotype) =>
            _diagramBuilder.GetConnectorType(modelRelationshipStereotype);

        public IDiagramShape ShowModelItem(IModelItem modelItem) => ShowModelItems(new[] { modelItem }).FirstOrDefault();
        public void HideModelItem(ModelItemId modelItemId) => HideModelItems(new[] { modelItemId });

        public virtual IReadOnlyList<IDiagramShape> ShowModelItems(IEnumerable<IModelItem> modelItems,
            CancellationToken cancellationToken = default(CancellationToken),
            IIncrementalProgress progress = null)
        {
            var diagramShapes = new List<IDiagramShape>();

            foreach (var modelItem in modelItems)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                if (modelItem is IModelNode)
                {
                    var diagramNode = GetOrAddDiagramNode((IModelNode)modelItem);
                    if (diagramNode != null)
                        diagramShapes.Add(diagramNode);
                }

                if (modelItem is IModelRelationship)
                {
                    var diagramConnector = GetOrAddDiagramConnector((IModelRelationship)modelItem);
                    if (diagramConnector != null)
                        diagramShapes.Add(diagramConnector);
                }

                progress?.Report(1);
            }

            return diagramShapes;
        }

        public virtual void HideModelItems(IEnumerable<ModelItemId> modelItemIds,
            CancellationToken cancellationToken = default(CancellationToken),
            IIncrementalProgress progress = null)
        {
            foreach (var modelItemId in modelItemIds)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var diagramShape = GetDiagramShapeByModelItemId(modelItemId);

                if (diagramShape != null)
                    RemoveDiagramShape(diagramShape);

                progress?.Report(1);
            }
        }

        public void RemoveDiagramShape(IDiagramShape diagramShape)
        {
            switch (diagramShape)
            {
                case IDiagramNode diagramNode:
                    RemoveDiagramNode(diagramNode);
                    break;

                case DiagramConnector diagramConnector:
                    RemoveDiagramConnector(diagramConnector);
                    break;

                default:
                    throw new Exception($"Unexpected diagramShape type: {diagramShape.GetType().Name}");
            }
        }

        public void SelectDiagramShape(IDiagramShape diagramShape) => OnDiagramShapeSelected(diagramShape);

        public IReadOnlyList<IModelNode> GetUndisplayedRelatedModelNodes(IDiagramNode diagramNode, DirectedModelRelationshipType modelRelationshipType)
        {
            var displayedDiagramNodes = Nodes;
            return ModelBuilder.CurrentModel
                .GetRelatedNodes(diagramNode.ModelItem.Id, modelRelationshipType)
                .Except(displayedDiagramNodes.Select(j => j.ModelNode))
                .ToArray();
        }

        public void ResizeDiagramNode(IDiagramNode diagramNode, Size2D newSize) => diagramNode.Size = newSize;
        public void MoveDiagramNodeCenter(IDiagramNode diagramNode, Point2D newCenter) => diagramNode.Center = newCenter;
        public void RerouteDiagramConnector(IDiagramConnector diagramConnector, Route newRoute) => diagramConnector.RoutePoints = newRoute;

        private IDiagramNode GetOrAddDiagramNode(IModelNode modelNode)
        {
            var getOrAddResult = _graph.GetOrAddVertex(
                vertex => vertex.ModelItem.Id == modelNode.Id,
                () => CreateDiagramNode(modelNode));

            if (getOrAddResult.IsAdd)
                ShowModelRelationshipsIfBothEndsAreVisible(modelNode);

            return getOrAddResult.Result;
        }

        private IDiagramNode CreateDiagramNode(IModelNode modelNode)
        {
            var diagramNode = _diagramNodeFactory.CreateDiagramNode(modelNode);

            diagramNode.SizeChanged += OnDiagramNodeSizeChanged;
            diagramNode.CenterChanged += OnDiagramNodeCenterChanged;

            return diagramNode;
        }

        private DiagramConnector GetOrAddDiagramConnector(IModelRelationship modelRelationship)
        {
            GetOrAddResult<DiagramConnector> getOrAddResult;

            lock (_graph.SyncRoot)
            {
                if (!DiagramNodeExists(modelRelationship.Source.Id) ||
                    !DiagramNodeExists(modelRelationship.Target.Id))
                    return null;

                getOrAddResult = _graph.GetOrAddEdge(
                    edge => edge.ModelItem.Id == modelRelationship.Id,
                    () => CreateDiagramConnector(modelRelationship));
            }

            if (getOrAddResult.IsAdd)
                HideRedundantDirectDiagramConnectors();

            return getOrAddResult.Result;
        }

        private DiagramConnector CreateDiagramConnector(IModelRelationship relationship)
        {
            var sourceNode = FindDiagramNodeByModelItemId(relationship.Source.Id);
            var targetNode = FindDiagramNodeByModelItemId(relationship.Target.Id);
            var connectorType = GetConnectorType(relationship.Stereotype);
            var diagramConnector = new DiagramConnector(relationship, sourceNode, targetNode, connectorType);

            diagramConnector.RouteChanged += OnDiagramConnectorRouteChanged;

            return diagramConnector;
        }

        private void RemoveDiagramNode(IDiagramNode diagramNode)
        {
            var result = _graph.RemoveVertex(diagramNode);

            var surroundingNodes = result.RemovedEdges.EmptyIfNull()
                .Select(i => i.GetOtherEnd(result.RemovedVertex)).Distinct();

            foreach (var neighbourNode in surroundingNodes)
                ShowModelRelationshipsIfBothEndsAreVisible(neighbourNode.ModelItem);
        }

        private void RemoveDiagramConnector(DiagramConnector diagramConnector)
        {
            var edgeRemoved = _graph.RemoveEdge(diagramConnector);

            if (edgeRemoved)
            {
                ShowModelRelationshipsIfBothEndsAreVisible(diagramConnector.Source.ModelItem);
                ShowModelRelationshipsIfBothEndsAreVisible(diagramConnector.Target.ModelItem);
            }
        }

        private void HideRedundantDirectDiagramConnectors()
        {
            //TODO: should only hide same-type connectors!!!
            foreach (var connector in _graph.Edges)
            {
                var paths = _graph.GetShortestPaths(connector.Source, connector.Target, 2).EmptyIfNull().ToArray();
                if (paths.Length > 1)
                {
                    var pathToHide = paths.FirstOrDefault(i => i.Length == 1);
                    if (pathToHide != null)
                    {
                        var diagramConnector = FindDiagramConnectorByModelItemId(pathToHide[0].ModelRelationship.Id) as DiagramConnector;
                        RemoveDiagramConnector(diagramConnector);
                    }
                }
            }
        }

        private void ShowModelRelationshipsIfBothEndsAreVisible(IModelItem modelItem)
        {
            foreach (var modelRelationship in ModelBuilder.CurrentModel.GetRelationships(modelItem.Id))
                ShowModelRelationshipIfBothEndsAreVisible(modelRelationship);
        }

        private void ShowModelRelationshipIfBothEndsAreVisible(IModelRelationship modelRelationship)
        {
            bool shouldShowModelRelationsip;

            lock (_graph.SyncRoot)
            {
                shouldShowModelRelationsip =
                    DiagramNodeExists(modelRelationship.Source.Id) &&
                    DiagramNodeExists(modelRelationship.Target.Id) &&
                    !DiagramConnectorWouldBeRedundant(modelRelationship);
            }

            if (shouldShowModelRelationsip)
                GetOrAddDiagramConnector(modelRelationship);
        }

        private bool DiagramConnectorWouldBeRedundant(IModelRelationship modelRelationship)
        {
            lock (_graph.SyncRoot)
            {
                var sourceNode = FindDiagramNodeByModelItemId(modelRelationship.Source.Id);
                var targetNode = FindDiagramNodeByModelItemId(modelRelationship.Target.Id);
                var paths = _graph.GetShortestPaths(sourceNode, targetNode, 1).ToList();
                return paths.Any();
            }
        }

        private IDiagramShape GetDiagramShapeByModelItemId(ModelItemId modelItemId)
        {
            return Shapes.FirstOrDefault(i => i.ModelItem.Id == modelItemId);
        }

        private IDiagramNode FindDiagramNodeByModelItemId(ModelItemId modelItemId)
        {
            return Nodes.FirstOrDefault(i => i.ModelNode.Id == modelItemId);
        }

        private bool DiagramNodeExists(ModelItemId modelItemId)
        {
            return Nodes.Any(i => i.ModelNode.Id == modelItemId);
        }

        private IDiagramConnector FindDiagramConnectorByModelItemId(ModelItemId modelItemId)
        {
            return Connectors.FirstOrDefault(i => i.ModelRelationship.Id == modelItemId);
        }

        private bool DiagramConnectorExists(ModelItemId modelItemId)
        {
            return Connectors.Any(i => i.ModelRelationship.Id == modelItemId);
        }

        private void OnDiagramShapeAdded(IDiagramShape diagramShape) => ShapeAdded?.Invoke(diagramShape);
        private void OnDiagramShapeRemoved(IDiagramShape diagramShape) => ShapeRemoved?.Invoke(diagramShape);
        private void OnDiagramCleared() => DiagramCleared?.Invoke();
        private void OnDiagramShapeSelected(IDiagramShape diagramShape) => ShapeSelected?.Invoke(diagramShape);

        private void OnDiagramNodeSizeChanged(IDiagramNode diagramNode, Size2D oldSize, Size2D newSize)
        {
            NodeSizeChanged?.Invoke(diagramNode, oldSize, newSize);
        }

        private void OnDiagramNodeCenterChanged(IDiagramNode diagramNode, Point2D oldCenter, Point2D newCenter)
        {
            NodeCenterChanged?.Invoke(diagramNode, oldCenter, newCenter);
        }

        private void OnDiagramConnectorRouteChanged(IDiagramConnector diagramConnector, Route oldRoute, Route newRoute)
        {
            ConnectorRouteChanged?.Invoke(diagramConnector, oldRoute, newRoute);
        }

        private void OnModelRelationshipAdded(IModelRelationship modelRelationship, IModel model)
        {
            ShowModelRelationshipIfBothEndsAreVisible(modelRelationship);
        }

        private void OnModelNodeRemoved(IModelNode modelNode, IModel model)
        {
            HideModelItem(modelNode.Id);
        }

        private void OnModelRelationshipRemoved(IModelRelationship modelRelationship, IModel model)
        {
            HideModelItem(modelRelationship.Id);
        }

        private void OnModelNodeUpdated(IModelNode oldNode, IModelNode newNode, IModel model)
        {
            var diagramNode = FindDiagramNodeByModelItemId(oldNode.Id);
            diagramNode?.Update(newNode);
        }

        private void OnModelCleared(IModel model)
        {
            Clear();
        }

        private void OnDiagramNodeAddedToGraph(IDiagramNode diagramNode)
        {
            OnDiagramShapeAdded(diagramNode);
        }

        private void OnDiagramEdgeAddedToGraph(DiagramConnector diagramConnector)
        {
            OnDiagramShapeAdded(diagramConnector);
        }

        private void OnDiagramNodeRemovedFromGraph(IDiagramNode diagramNode)
        {
            diagramNode.SizeChanged -= OnDiagramNodeSizeChanged;
            diagramNode.CenterChanged -= OnDiagramNodeCenterChanged;
            OnDiagramShapeRemoved(diagramNode);
        }

        private void OnDiagramEdgeRemovedFromGraph(DiagramConnector diagramConnector)
        {
            diagramConnector.RouteChanged -= OnDiagramConnectorRouteChanged;
            OnDiagramShapeRemoved(diagramConnector);
        }
    }
}
