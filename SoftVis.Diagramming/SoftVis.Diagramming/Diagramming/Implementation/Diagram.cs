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
        public IReadOnlyModel Model { get; }

        private readonly DiagramGraph _graph;

        public event Action<IDiagramShape> ShapeAdded;
        public event Action<IDiagramShape> ShapeRemoved;
        public event Action<IDiagramShape> ShapeSelected;

        public event Action DiagramCleared;

        public event Action<IDiagramNode, Size2D, Size2D> NodeSizeChanged;
        public event Action<IDiagramNode, Point2D, Point2D> NodeCenterChanged;
        public event Action<IDiagramConnector, Route, Route> ConnectorRouteChanged;

        public Diagram(IReadOnlyModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            Model = model;
            Model.EntityRenamed += OnModelEntityRenamed;
            Model.RelationshipAdded += OnModelRelationshipAdded;
            Model.EntityRemoved += OnModelEntityRemoved;
            Model.RelationshipRemoved += OnModelRelationshipRemoved;
            Model.ModelCleared += OnModelCleared;

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

        public virtual IEnumerable<EntityRelationType> GetEntityRelationTypes()
        {
            yield return EntityRelationTypes.BaseType;
            yield return EntityRelationTypes.Subtype;
        }

        public virtual ConnectorType GetConnectorType(ModelRelationshipType type) => ConnectorTypes.Generalization;

        /// <summary>
        /// Clear the diagram (that is, hide all nodes and connectors).
        /// </summary>
        public virtual void Clear()
        {
            _graph.Clear();
            OnDiagramCleared();
        }

        public IDiagramShape ShowModelItem(IModelItem modelItem) => ShowModelItems(new[] { modelItem }).FirstOrDefault();
        public void HideModelItem(IModelItem modelItem) => HideModelItems(new[] { modelItem });

        public virtual IReadOnlyList<IDiagramShape> ShowModelItems(IEnumerable<IModelItem> modelItems,
            CancellationToken cancellationToken = default(CancellationToken),
            IIncrementalProgress progress = null)
        {
            var diagramShapes = new List<IDiagramShape>();

            foreach (var modelItem in modelItems)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                if (modelItem is IModelEntity)
                {
                    var diagramNode = GetOrAddDiagramNode((IModelEntity)modelItem);
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

        public virtual void HideModelItems(IEnumerable<IModelItem> modelItems,
            CancellationToken cancellationToken = default(CancellationToken),
            IIncrementalProgress progress = null)
        {
            foreach (var modelItem in modelItems)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (modelItem is IModelEntity)
                    RemoveDiagramNode((IModelEntity)modelItem);

                if (modelItem is IModelRelationship)
                    RemoveDiagramConnector((IModelRelationship)modelItem);

                progress?.Report(1);
            }
        }

        public void SelectDiagramShape(IDiagramShape diagramShape) => OnDiagramShapeSelected(diagramShape);
        public void RemoveDiagramShape(IDiagramShape diagramShape) => HideModelItems(new[] { diagramShape.ModelItem });

        public IReadOnlyList<IModelEntity> GetUndisplayedRelatedModelEntities(IDiagramNode diagramNode, EntityRelationType relationType)
        {
            var displayedDiagramNodes = Nodes;
            return Model
                .GetRelatedEntities(diagramNode.ModelEntity, relationType)
                .Where(i => displayedDiagramNodes.All(j => j.ModelEntity != i)).ToArray();
        }

        public void ResizeDiagramNode(IDiagramNode diagramNode, Size2D newSize) => diagramNode.Size = newSize;
        public void MoveDiagramNodeCenter(IDiagramNode diagramNode, Point2D newCenter) => diagramNode.Center = newCenter;
        public void RerouteDiagramConnector(IDiagramConnector diagramConnector, Route newRoute) => diagramConnector.RoutePoints = newRoute;

        private IDiagramNode GetOrAddDiagramNode(IModelEntity modelEntity)
        {
            var getOrAddResult = _graph.GetOrAddVertex(
                vertex => vertex.ModelEntity == modelEntity,
                () => CreateDiagramNode(modelEntity));

            if (getOrAddResult.IsAdd)
                ShowModelRelationshipsIfBothEndsAreVisible(modelEntity);

            return getOrAddResult.Result;
        }

        private DiagramNode CreateDiagramNode(IModelEntity modelEntity)
        {
            var diagramNode = new DiagramNode(modelEntity);

            diagramNode.SizeChanged += OnDiagramNodeSizeChanged;
            diagramNode.CenterChanged += OnDiagramNodeCenterChanged;

            return diagramNode;
        }

        private DiagramConnector GetOrAddDiagramConnector(IModelRelationship modelRelationship)
        {
            GetOrAddResult<DiagramConnector> getOrAddResult;

            lock (_graph.SyncRoot)
            {
                if (!DiagramNodeExists(modelRelationship.Source) ||
                    !DiagramNodeExists(modelRelationship.Target))
                    return null;

                getOrAddResult = _graph.GetOrAddEdge(
                    edge => edge.ModelRelationship.Equals(modelRelationship),
                    () => CreateDiagramConnector(modelRelationship));
            }

            if (getOrAddResult.IsAdd)
                HideRedundantDirectDiagramConnectors();

            return getOrAddResult.Result;
        }

        private DiagramConnector CreateDiagramConnector(IModelRelationship relationship)
        {
            var sourceNode = FindDiagramNode(relationship.Source);
            var targetNode = FindDiagramNode(relationship.Target);
            var diagramConnector = new DiagramConnector(relationship, sourceNode, targetNode);

            diagramConnector.RouteChanged += OnDiagramConnectorRouteChanged;

            return diagramConnector;
        }

        private void RemoveDiagramNode(IModelEntity modelEntity)
        {
            var result = _graph.RemoveVertex(i => i.ModelEntity == modelEntity);

            var surroundingNodes = result.RemovedEdges.EmptyIfNull()
                .Select(i => i.GetOtherEnd(result.RemovedVertex)).Distinct();

            foreach (var diagramNode in surroundingNodes)
                ShowModelRelationshipsIfBothEndsAreVisible(diagramNode.ModelEntity);
        }

        private void RemoveDiagramConnector(IModelRelationship modelRelationship)
        {
            var edgeRemoved = _graph.RemoveEdge(i => i.ModelRelationship.Equals(modelRelationship));

            if (edgeRemoved)
            {
                ShowModelRelationshipsIfBothEndsAreVisible(modelRelationship.Source);
                ShowModelRelationshipsIfBothEndsAreVisible(modelRelationship.Target);
            }
        }

        private void HideRedundantDirectDiagramConnectors()
        {
            // TODO: should only hide same-type connectors!!!
            foreach (var connector in _graph.Edges)
            {
                var paths = _graph.GetShortestPaths(connector.Source, connector.Target, 2).EmptyIfNull().ToArray();
                if (paths.Length > 1)
                {
                    var pathToHide = paths.FirstOrDefault(i => i.Length == 1);
                    if (pathToHide != null)
                        RemoveDiagramConnector(pathToHide[0].ModelRelationship);
                }
            }
        }

        private void ShowModelRelationshipsIfBothEndsAreVisible(IModelEntity modelEntity)
        {
            foreach (var modelRelationship in Model.GetRelationships(modelEntity))
                ShowModelRelationshipIfBothEndsAreVisible(modelRelationship);
        }

        private void ShowModelRelationshipIfBothEndsAreVisible(IModelRelationship modelRelationship)
        {
            bool shouldShowModelRelationsip;

            lock (_graph.SyncRoot)
            {
                shouldShowModelRelationsip =
                    DiagramNodeExists(modelRelationship.Source) &&
                    DiagramNodeExists(modelRelationship.Target) &&
                    !DiagramConnectorWouldBeRedundant(modelRelationship);
            }

            if (shouldShowModelRelationsip)
                GetOrAddDiagramConnector(modelRelationship);
        }

        private bool DiagramConnectorWouldBeRedundant(IModelRelationship modelRelationship)
        {
            lock (_graph.SyncRoot)
            {
                var sourceNode = FindDiagramNode(modelRelationship.Source);
                var targetNode = FindDiagramNode(modelRelationship.Target);
                var paths = _graph.GetShortestPaths(sourceNode, targetNode, 1).ToList();
                return paths.Any();
            }
        }

        private DiagramNode FindDiagramNode(IModelEntity modelEntity)
        {
            return _graph.Vertices.FirstOrDefault(i => Equals(i.ModelEntity, modelEntity));
        }

        private bool DiagramNodeExists(IModelEntity modelEntity)
        {
            return Nodes.Any(i => Equals(i.ModelEntity, modelEntity));
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

        private void OnModelRelationshipAdded(IModelRelationship modelRelationship)
        {
            ShowModelRelationshipIfBothEndsAreVisible(modelRelationship);
        }

        private void OnModelEntityRemoved(IModelEntity modelEntity)
        {
            HideModelItem(modelEntity);
        }

        private void OnModelRelationshipRemoved(IModelRelationship modelRelationship)
        {
            HideModelItem(modelRelationship);
        }

        private void OnModelEntityRenamed(IModelEntity modelEntity, string name, string fullName)
        {
            var diagramNode = FindDiagramNode(modelEntity);
            diagramNode?.Rename(name, fullName);
        }

        private void OnModelCleared()
        {
            Clear();
        }

        private void OnDiagramNodeAddedToGraph(DiagramNode diagramNode)
        {
            OnDiagramShapeAdded(diagramNode);
        }

        private void OnDiagramEdgeAddedToGraph(DiagramConnector diagramConnector)
        {
            OnDiagramShapeAdded(diagramConnector);
        }

        private void OnDiagramNodeRemovedFromGraph(DiagramNode diagramNode)
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
