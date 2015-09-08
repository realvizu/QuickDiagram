using Codartis.SoftVis.Diagramming.Shapes;
using Codartis.SoftVis.Diagramming.Shapes.Graph;
using Codartis.SoftVis.Diagramming.Shapes.Graph.Layout;
using Codartis.SoftVis.Modeling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Codartis.SoftVis.Diagramming.Shapes.Factories;

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
    public class Diagram
    {
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
        /// <param name="umlTypeOrPackage">A type or package model element.</param>
        public void ShowNode(UmlTypeOrPackage umlTypeOrPackage)
        {
            if (NodeExists(umlTypeOrPackage))
                return;

            var node = CreateDiagramNode(umlTypeOrPackage);
            _graph.AddVertex(node);
            SignalShapeAddedEvent(node);

            foreach (var relationship in umlTypeOrPackage.OutgoingRelationships)
            {
                if (NodeExists(relationship.SourceElement) &&
                    NodeExists(relationship.TargetElement))
                {
                    ShowConnector(relationship);
                }
            }
        }

        /// <summary>
        /// Show a connector on the diagram that represents the given model element.
        /// </summary>
        /// <param name="umlRelationship">A relationship model element.</param>
        public void ShowConnector(UmlRelationship umlRelationship)
        {
            if (ConnectorExists(umlRelationship))
                return;

            var connector = CreateDiagramConnector(umlRelationship);
            _graph.AddEdge(connector);
            SignalShapeAddedEvent(connector);
        }

        /// <summary>
        /// Hide a node from the diagram that represents the given model element.
        /// </summary>
        /// <param name="umlTypeOrPackage">A type or package model element.</param>
        public void HideNode(UmlTypeOrPackage umlTypeOrPackage)
        {
            if (!NodeExists(umlTypeOrPackage))
                return;

            var node = FindNode(umlTypeOrPackage);
            _graph.RemoveVertex(node);
            SignalShapeRemovedEvent(node);
        }

        /// <summary>
        /// Hodes a connector from the diagram that represents the given model element.
        /// </summary>
        /// <param name="umlRelationship">A relationship model element.</param>
        public void HideConnector(UmlRelationship umlRelationship)
        {
            if (!ConnectorExists(umlRelationship))
                return;

            var connector = FindConnector(umlRelationship);
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

        protected virtual DiagramNode CreateDiagramNode(UmlTypeOrPackage umlTypeOrPackage)
        {
            return ModelToDiagramNodeTranslator.Translate(umlTypeOrPackage);
        }

        protected virtual DiagramConnector CreateDiagramConnector(UmlRelationship umlRelationship)
        {
            var sourceNode = FindNode(umlRelationship.SourceElement);
            var targetNode = FindNode(umlRelationship.TargetElement);
            return ModelToDiagramConnectorTranslator.Translate(umlRelationship, sourceNode, targetNode);
        }

        private DiagramNode FindNode(UmlTypeOrPackage typeOrPackage)
        {
            return Nodes.FirstOrDefault(i => i.UmlTypeOrPackage == typeOrPackage);
        }

        private bool NodeExists(UmlTypeOrPackage typeOrPackage)
        {
            return Nodes.Any(i => i.UmlTypeOrPackage == typeOrPackage);
        }

        private DiagramConnector FindConnector(UmlRelationship relationship)
        {
            return Connectors.FirstOrDefault(i => i.UmlRelationship == relationship);
        }

        private bool ConnectorExists(UmlRelationship relationship)
        {
            return Connectors.Any(i => i.UmlRelationship == relationship);
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
