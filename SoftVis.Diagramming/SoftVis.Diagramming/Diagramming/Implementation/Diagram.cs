using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs;
using Codartis.SoftVis.Modeling;

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
        public event Action<IDiagramShape> ShapeActivated;
        public event Action Cleared;

        public event Action BatchAddStarted;
        public event Action BatchAddFinished;
        public event Action BatchRemoveStarted;
        public event Action BatchRemoveFinished;

        public event Action<IDiagramNode, Size2D, Size2D> NodeSizeChanged;
        public event Action<IDiagramNode, Point2D, Point2D> NodeTopLeftChanged;
        public event Action<IDiagramConnector, Route, Route> ConnectorRouteChanged;

        public Diagram(IReadOnlyModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            Model = model;
            Model.RelationshipAdded += OnRelationshipAdded;

            _graph = new DiagramGraph();
        }

        public IEnumerable<DiagramNode> Nodes => _graph.Vertices;
        IEnumerable<IDiagramNode> IDiagram.Nodes => Nodes;

        public IEnumerable<DiagramConnector> Connectors => _graph.Edges;
        IEnumerable<IDiagramConnector> IDiagram.Connectors => Connectors;

        public IEnumerable<DiagramShape> Shapes => Nodes.OfType<DiagramShape>().Union(Connectors);
        IEnumerable<IDiagramShape> IDiagram.Shapes => Shapes;

        public Rect2D ContentRect
        {
            get
            {
                lock (this)
                    return Shapes.Where(i => i.IsRectDefined).Select(i => i.Rect).Union();
            }
        }

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
            OnCleared();
        }

        public void ShowItem(IModelItem modelItem) => ShowItems(new[] { modelItem });
        public void HideItem(IModelItem modelItem) => HideItems(new[] { modelItem });

        public virtual void ShowItems(IEnumerable<IModelItem> modelItems)
        {
            BatchAddStarted?.Invoke();

            foreach (var modelItem in modelItems)
            {
                if (modelItem is IModelEntity)
                    ShowEntityCore((IModelEntity)modelItem);

                if (modelItem is IModelRelationship)
                    ShowRelationshipCore((IModelRelationship)modelItem);
            }

            BatchAddFinished?.Invoke();
        }

        public virtual void HideItems(IEnumerable<IModelItem> modelItems)
        {
            BatchRemoveStarted?.Invoke();

            foreach (var modelItem in modelItems)
            {
                if (modelItem is IModelEntity)
                    HideEntityCore((IModelEntity)modelItem);

                if (modelItem is IModelRelationship)
                    HideRelationshipCore((IModelRelationship)modelItem);
            }

            BatchRemoveFinished?.Invoke();
        }

        public void SelectShape(IDiagramShape diagramShape)
        {
            OnShapeSelected(diagramShape);
        }

        public void ActivateShape(IDiagramShape diagramShape)
        {
            OnShapeActivated(diagramShape);
        }

        public void RemoveShape(IDiagramShape diagramShape)
        {
            HideItems(new[] { diagramShape.ModelItem });
        }

        public IEnumerable<IModelEntity> GetUndisplayedRelatedEntities(IDiagramNode diagramNode, EntityRelationType relationType)
        {
            return Model
                .GetRelatedEntities(diagramNode.ModelEntity, relationType)
                .Where(i => Nodes.All(j => j.ModelEntity != i));
        }

        public void ResizeNode(IDiagramNode diagramNode, Size2D newSize)
        {
            diagramNode.Size = newSize;
        }

        public void MoveNodeCenter(IDiagramNode diagramNode, Point2D newCenter)
        {
            diagramNode.Center = newCenter;
        }

        public void RerouteConnector(IDiagramConnector diagramConnector, Route newRoute)
        {
            diagramConnector.RoutePoints = newRoute;
        }

        /// <summary>
        /// Show a node on the diagram that represents the given model element.
        /// </summary>
        /// <param name="modelEntity">A type or package model element.</param>
        protected virtual void ShowEntityCore(IModelEntity modelEntity)
        {
            if (NodeExists(modelEntity))
                return;

            var diagramNode = CreateDiagramNode(modelEntity);
            AddDiagramNode(diagramNode);
            ShowRelationshipsIfBothEndsAreVisible(modelEntity);
        }

        /// <summary>
        /// Show a connector on the diagram that represents the given model element.
        /// </summary>
        /// <param name="modelRelationship">A relationship model item.</param>
        protected virtual void ShowRelationshipCore(IModelRelationship modelRelationship)
        {
            if (ConnectorExists(modelRelationship) ||
                !NodeExists(modelRelationship.Source) ||
                !NodeExists(modelRelationship.Target))
                return;

            var diagramConnector = CreateDiagramConnector(modelRelationship);
            AddDiagramConnector(diagramConnector);
            HideRedundantDirectEdges();
        }

        /// <summary>
        /// Hide a node from the diagram that represents the given model element.
        /// </summary>
        /// <param name="modelEntity">A type or package model element.</param>
        protected virtual void HideEntityCore(IModelEntity modelEntity)
        {
            if (!NodeExists(modelEntity))
                return;

            var diagramNode = FindNode(modelEntity);

            var connectedEntities = new HashSet<IModelEntity>();
            foreach (var edge in _graph.GetAllEdges(diagramNode).ToArray())
            {
                var connectedEntity = edge.GetOtherEnd(diagramNode).ModelEntity;
                connectedEntities.Add(connectedEntity);
                HideRelationshipCore(edge.ModelRelationship);
            }

            RemoveDiagramNode(diagramNode);

            foreach (var connectedEntity in connectedEntities)
                ShowRelationshipsIfBothEndsAreVisible(connectedEntity);
        }

        /// <summary>
        /// Hides a connector from the diagram that represents the given model element.
        /// </summary>
        /// <param name="modelRelationship">A modelRelationship model item.</param>
        protected virtual void HideRelationshipCore(IModelRelationship modelRelationship)
        {
            if (!ConnectorExists(modelRelationship))
                return;

            var diagramConnector = FindConnector(modelRelationship);
            RemoveDiagramConnector(diagramConnector);
        }

        private void AddDiagramNode(DiagramNode diagramNode)
        {
            lock (this)
            {
                diagramNode.SizeChanged += OnDiagramNodeSizeChanged;
                diagramNode.TopLeftChanged += OnDiagramNodeTopLeftChanged;

                _graph.AddVertex(diagramNode);
                OnShapeAdded(diagramNode);
            }
        }

        private void RemoveDiagramNode(DiagramNode diagramNode)
        {
            lock (this)
            {
                diagramNode.SizeChanged -= OnDiagramNodeSizeChanged;
                diagramNode.TopLeftChanged -= OnDiagramNodeTopLeftChanged;

                _graph.RemoveVertex(diagramNode);
                OnShapeRemoved(diagramNode);
            }
        }

        private void AddDiagramConnector(DiagramConnector diagramConnector)
        {
            lock (this)
            {
                diagramConnector.RouteChanged += OnDiagramConnectorRouteChanged;

                _graph.AddEdge(diagramConnector);
                OnShapeAdded(diagramConnector);
            }
        }

        private void RemoveDiagramConnector(DiagramConnector diagramConnector)
        {
            lock (this)
            {
                diagramConnector.RouteChanged -= OnDiagramConnectorRouteChanged;

                _graph.RemoveEdge(diagramConnector);
                OnShapeRemoved(diagramConnector);
            }
        }

        private static DiagramNode CreateDiagramNode(IModelEntity modelEntity)
        {
            return new DiagramNode(modelEntity);
        }

        private DiagramConnector CreateDiagramConnector(IModelRelationship relationship)
        {
            var sourceNode = FindNode(relationship.Source);
            var targetNode = FindNode(relationship.Target);
            return new DiagramConnector(relationship, sourceNode, targetNode);
        }

        private void HideRedundantDirectEdges()
        {
            // TODO: should only hide same-type connectors!!!

            foreach (var connector in Connectors.ToList())
            {
                var paths = _graph.GetShortestPaths(connector.Source, connector.Target, 2).ToList();
                if (paths.Count > 1)
                {
                    var pathToHide = paths.FirstOrDefault(i => i.Length == 1);
                    if (pathToHide != null)
                        HideRelationshipCore(pathToHide[0].ModelRelationship);
                }
            }
        }

        private void ShowRelationshipIfBothEndsAreVisible(IModelRelationship modelRelationship)
        {
            if (NodeExists(modelRelationship.Source) && NodeExists(modelRelationship.Target))
                ShowRelationshipCore(modelRelationship);
        }

        private void ShowRelationshipsIfBothEndsAreVisible(IModelEntity modelEntity)
        {
            foreach (var modelRelationship in Model.GetRelationships(modelEntity).ToList())
                ShowRelationshipIfBothEndsAreVisible(modelRelationship);
        }

        private DiagramNode FindNode(IModelEntity modelEntity)
        {
            return Nodes.FirstOrDefault(i => Equals(i.ModelEntity, modelEntity));
        }

        private bool NodeExists(IModelEntity modelEntity)
        {
            return Nodes.Any(i => Equals(i.ModelEntity, modelEntity));
        }

        private DiagramConnector FindConnector(IModelRelationship modelRelationship)
        {
            return Connectors.FirstOrDefault(i => Equals(i.ModelRelationship, modelRelationship));
        }

        private bool ConnectorExists(IModelRelationship modelRelationship)
        {
            return Connectors.Any(i => Equals(i.ModelRelationship, modelRelationship));
        }

        private void OnShapeAdded(IDiagramShape diagramShape)
        {
            //Debug.WriteLine($"Firing ShapeAdded({diagramShape}).");
            ShapeAdded?.Invoke(diagramShape);
        }

        private void OnShapeRemoved(IDiagramShape diagramShape)
        {
            //Debug.WriteLine($"Firing ShapeRemoved({diagramShape}).");
            ShapeRemoved?.Invoke(diagramShape);
        }

        private void OnCleared() => Cleared?.Invoke();
        private void OnShapeSelected(IDiagramShape diagramShape) => ShapeSelected?.Invoke(diagramShape);
        private void OnShapeActivated(IDiagramShape diagramShape) => ShapeActivated?.Invoke(diagramShape);

        private void OnDiagramNodeSizeChanged(IDiagramNode diagramNode, Size2D oldSize, Size2D newSize)
        {
            //Debug.WriteLine($"Firing NodeResized({diagramNode}, {oldSize}, {newSize}.");
            NodeSizeChanged?.Invoke(diagramNode, oldSize, newSize);
        }

        private void OnDiagramNodeTopLeftChanged(IDiagramNode diagramNode, Point2D oldTopLeft, Point2D newTopLeft)
        {
            //Debug.WriteLine($"Firing NodeMoved({diagramNode}, {oldTopLeft}, {newTopLeft}.");
            NodeTopLeftChanged?.Invoke(diagramNode, oldTopLeft, newTopLeft);
        }

        private void OnDiagramConnectorRouteChanged(IDiagramConnector diagramConnector, Route oldRoute, Route newRoute)
        {
            //Debug.WriteLine($"Firing ConnectorRerouted({diagramConnector}, {oldRoute}, {newRoute}.");
            ConnectorRouteChanged?.Invoke(diagramConnector, oldRoute, newRoute);
        }

        private void OnRelationshipAdded(object sender, IModelRelationship modelRelationship)
        {
            ShowRelationshipIfBothEndsAreVisible(modelRelationship);
        }
    }
}
