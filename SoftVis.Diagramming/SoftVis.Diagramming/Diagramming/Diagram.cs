using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Codartis.SoftVis.Diagramming.Shapes;
using Codartis.SoftVis.Diagramming.Shapes.Graph;
using Codartis.SoftVis.Diagramming.Shapes.Graph.Layout;
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
        protected static readonly DiagramPoint DefaultNodePosition = DiagramPoint.Zero;
        protected static readonly DiagramSize DefaultNodeSize = new DiagramSize(100,25);

        private readonly DiagramGraph _graph = new DiagramGraph();

        public IEnumerable<DiagramNode> Nodes => _graph.Vertices;
        public IEnumerable<DiagramConnector> Connectors => _graph.Edges;

        public event EventHandler<DiagramShape> ShapeAdded;
        public event EventHandler<DiagramShape> ShapeModified;
        public event EventHandler<DiagramShape> ShapeRemoved;
        public event EventHandler Cleared;

        /// <summary>
        /// Show a node on the diagram that represents the given model element.
        /// </summary>
        /// <param name="modelEntity">A type or package model element.</param>
        public void ShowNode(IModelEntity modelEntity)
        {
            if (NodeExists(modelEntity))
                return;

            var node = CreateDiagramNode(modelEntity);
            _graph.AddVertex(node);
            SignalShapeAddedEvent(node);

            foreach (var modelRelationship in modelEntity.OutgoingRelationships)
            {
                if (NodeExists(modelRelationship.Source) &&
                    NodeExists(modelRelationship.Target))
                {
                    ShowConnector(modelRelationship);
                }
            }
        }

        /// <summary>
        /// Show a connector on the diagram that represents the given model element.
        /// </summary>
        /// <param name="modelRelationship">A relationship model item.</param>
        public void ShowConnector(IModelRelationship modelRelationship)
        {
            if (ConnectorExists(modelRelationship))
                return;

            var connector = CreateDiagramConnector(modelRelationship);
            _graph.AddEdge(connector);
            SignalShapeAddedEvent(connector);
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
            SignalShapeRemovedEvent(node);
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
            SignalShapeRemovedEvent(connector);
        }

        /// <summary>
        /// Recalculates the layout of the diagram and applies the new shape positions.
        /// </summary>
        public void Layout()
        {
            var algorithm = new SimpleTreeLayoutAlgorithm(_graph);
            var newNodePositions = algorithm.ComputeNewVertexPositions();

            foreach (var node in Nodes)
            {
                node.Position = newNodePositions[node];
                SignalShapeModifiedEvent(node);
            }
        }

        /// <summary>
        /// Clear the diagram (that is, hides all nodes and connectors).
        /// </summary>
        public void Clear()
        {
            _graph.Clear();
            SignalClearedEvent();
        }

        protected abstract DiagramNode CreateDiagramNode(IModelEntity modelEntity);

        private DiagramConnector CreateDiagramConnector(IModelRelationship relationship)
        {
            var sourceNode = FindNode(relationship.Source);
            var targetNode = FindNode(relationship.Target);
            return new DiagramConnector(relationship, sourceNode, targetNode);
        }

        private DiagramNode FindNode(IModelEntity modelEntity)
        {
            return Nodes.FirstOrDefault(i => i.ModelEntity == modelEntity);
        }

        private bool NodeExists(IModelEntity modelEntity)
        {
            return Nodes.Any(i => i.ModelEntity == modelEntity);
        }

        private DiagramConnector FindConnector(IModelRelationship modelRelationship)
        {
            return Connectors.FirstOrDefault(i => i.ModelRelationship == modelRelationship);
        }

        private bool ConnectorExists(IModelRelationship modelRelationship)
        {
            return Connectors.Any(i => i.ModelRelationship == modelRelationship);
        }

        private void SignalShapeAddedEvent(DiagramShape shape)
        {
            ShapeAdded?.Invoke(this, shape);
        }

        private void SignalShapeModifiedEvent(DiagramShape shape)
        {
            ShapeModified?.Invoke(this, shape);
        }

        private void SignalShapeRemovedEvent(DiagramShape shape)
        {
            ShapeRemoved?.Invoke(this, shape);
        }

        private void SignalClearedEvent()
        {
            Cleared?.Invoke(this, EventArgs.Empty);
        }
    }
}
