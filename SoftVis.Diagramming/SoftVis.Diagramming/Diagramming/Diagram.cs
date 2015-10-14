using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs.Layout;
using Codartis.SoftVis.Graphs.Layout.EdgeRouting;
using Codartis.SoftVis.Graphs.Layout.VertexPlacement;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A diagram is a partial, graphical representation of a model. 
    /// A diagram shows a subset of the model and there can be many diagrams depicting different areas/aspects of the same model.
    /// A diagram consists of shapes that represent model elements.
    /// The shapes form a directed graph: some shapes are nodes in the graph and others are connectors between nodes.
    /// The layout of the shapes (relative positions and size) also conveys meaning.
    /// </summary>
    [DebuggerDisplay("VertexCount={_graph.VertexCount}, EdgeCount={_graph.EdgeCount}")]
    public abstract class Diagram
    {
        protected static readonly Point2D DefaultNodePosition = Point2D.Zero;

        protected static readonly double MinimumNodeWidth = 40;
        protected static readonly double DefaultNodeWidth = 100;
        protected static readonly double MaximumNodeWidth = 250;
        protected static readonly double MinimumNodeHeight = 20;
        protected static readonly double DefaultNodeHeight = 38;
        protected static readonly double MaximumNodeHeight = 50;

        protected static readonly Size2D DefaultNodeSize = new Size2D(DefaultNodeWidth, DefaultNodeHeight);

        private readonly DiagramGraph _graph = new DiagramGraph();

        public List<RectMoveEventArgs> LastConnectorTriggeredNodeMoves => _graph.LastEdgeTriggeredVertexMoves;
        public int TotalNodeMoveCount => _graph.TotalVertexMoveCount;

        public IEnumerable<DiagramNode> Nodes => _graph.Vertices;
        public IEnumerable<DiagramConnector> Connectors => _graph.Edges;

        public event EventHandler<DiagramShape> ShapeAdded;
        public event EventHandler<DiagramShape> ShapeModified;
        public event EventHandler<DiagramShape> ShapeRemoved;
        public event EventHandler<DiagramShape> ShapeSelected;
        public event EventHandler<DiagramShape> ShapeActivated;
        public event EventHandler Cleared;
         
        /// <summary>
        /// Show a node on the diagram that represents the given model element.
        /// </summary>
        /// <param name="modelEntity">A type or package model element.</param>
        public virtual void ShowNode(IModelEntity modelEntity)
        {
            if (!NodeExists(modelEntity))
            {
                var node = CreateDiagramNode(modelEntity);
                _graph.AddVertex(node);
                OnShapeAdded(node);
            }
        }

        /// <summary>
        /// Show a connector on the diagram that represents the given model element.
        /// </summary>
        /// <param name="modelRelationship">A relationship model item.</param>
        public void ShowConnector(IModelRelationship modelRelationship)
        {
            var connector = FindConnector(modelRelationship);
            if (connector == null)
            {
                connector = CreateDiagramConnector(modelRelationship);
                _graph.AddEdge(connector);
                OnShapeAdded(connector);
            }

            HideRedundantDirectEdges();
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
                        HideConnector(pathToHide[0].ModelRelationship);
                }
            }
        }

        /// <summary>
        /// Hide a node from the diagram that represents the given model element.
        /// </summary>
        /// <param name="modelEntity">A type or package model element.</param>
        public void HideNode(IModelEntity modelEntity)
        {
            if (!NodeExists(modelEntity))
                return;

            var node = FindNode(modelEntity);
            _graph.RemoveVertex(node);
            OnShapeRemoved(node);
        }

        /// <summary>
        /// Hodes a connector from the diagram that represents the given model element.
        /// </summary>
        /// <param name="modelRelationship">A modelRelationship model item.</param>
        public void HideConnector(IModelRelationship modelRelationship)
        {
            if (!ConnectorExists(modelRelationship))
                return;

            var connector = FindConnector(modelRelationship);
            _graph.RemoveEdge(connector);
            OnShapeRemoved(connector);
        }

        /// <summary>
        /// Recalculates the layout of the diagram and applies the new shape positions and edge routes.
        /// </summary>
        public void Layout(LayoutType layoutType, ILayoutParameters layoutParameters = null)
        {
            switch (layoutType)
            {
                case (LayoutType.Tree):
                    ApplySimpleTreeLayoutAndStraightEdgeRouting();
                    break;
                default:
                    throw new ArgumentException($"Unexpected layout type: {layoutType}");
            }
        }

        /// <summary>
        /// Clear the diagram (that is, hides all nodes and connectors).
        /// </summary>
        public void Clear()
        {
            _graph.Clear();
            OnCleared();
        }

        protected abstract DiagramNode CreateDiagramNode(IModelEntity modelEntity);

        protected abstract DiagramConnector CreateDiagramConnector(IModelRelationship relationship);

        private void ApplySimpleTreeLayoutAndStraightEdgeRouting()
        {
            var layoutAlgorithm = new SimpleTreeLayoutAlgorithm<DiagramNode, DiagramConnector>(_graph);
            layoutAlgorithm.Compute();

            ApplyVertexCenters(layoutAlgorithm.VertexCenters);

            var routingAlgorithm = new StraightEdgeRoutingAlgorithm<DiagramNode, DiagramConnector>(_graph);
            routingAlgorithm.Compute();

            ApplyConnectorRoutes(routingAlgorithm.EdgeRoutes);
        }

        private void ApplyEdgeRouting(DiagramConnectorRoute specification)
        {
            var routingAlgorithm = new EdgeRoutingAlgorithm<DiagramNode, DiagramConnector>(
                _graph.Edges, specification.EdgeRoutingType, specification.InterimRoutePointsOfEdges);
            routingAlgorithm.Compute();

            ApplyConnectorRoutes(routingAlgorithm.EdgeRoutes);
        }

        private void ApplyVertexCenters(IDictionary<DiagramNode, Point2D> vertexCenters)
        {
            foreach (var node in Nodes)
            {
                Point2D center;
                if (vertexCenters.TryGetValue(node, out center))
                {
                    node.Center = center;
                    OnShapeModified(node);
                }
            }
        }

        private void ApplyConnectorRoutes(IDictionary<DiagramConnector, Route> edgeRoutes)
        {
            foreach (var connector in Connectors)
            {
                Route route;
                if (edgeRoutes.TryGetValue(connector, out route))
                {
                    connector.RoutePoints = route;
                    OnShapeModified(connector);
                }
            }
        }

        protected DiagramNode FindNode(IModelEntity modelEntity)
        {
            return Nodes.FirstOrDefault(i => Equals(i.ModelEntity, modelEntity));
        }

        protected bool NodeExists(IModelEntity modelEntity)
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

        private void OnShapeAdded(DiagramShape shape)
        {
            ShapeAdded?.Invoke(this, shape);
        }

        private void OnShapeModified(DiagramShape shape)
        {
            ShapeModified?.Invoke(this, shape);
        }

        private void OnShapeRemoved(DiagramShape shape)
        {
            ShapeRemoved?.Invoke(this, shape);
        }

        private void OnCleared()
        {
            Cleared?.Invoke(this, EventArgs.Empty);
        }

        public void OnShapeSelected(DiagramShape diagramShape)
        {
            ShapeSelected?.Invoke(this, diagramShape);
        }

        public void OnShapeActivated(DiagramShape diagramShape)
        {
            ShapeActivated?.Invoke(this, diagramShape);
        }
    }
}
