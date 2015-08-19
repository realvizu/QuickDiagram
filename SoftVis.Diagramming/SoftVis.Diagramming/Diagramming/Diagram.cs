using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Codartis.SoftVis.Diagramming.Layout;
using Codartis.SoftVis.Modeling;
using System;

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
        private DiagramGraph _graph = new DiagramGraph();

        public event EventHandler<DiagramShape> ShapeAdded;
        public event EventHandler<DiagramShape> ShapeRemoved;

        public IEnumerable<DiagramNode> Nodes
        {
            get { return _graph.Vertices.OfType<DiagramNode>(); }
        }

        public IEnumerable<DiagramConnector> Connectors
        {
            get { return _graph.Edges.OfType<DiagramConnector>(); }
        }

        public DiagramRect GetEnclosingRect()
        {
            return Nodes.Select(i => i.Rect).Union();
        }

        public void ShowModelElement(UmlModelElement modelElement)
        {
            DiagramShape shape = null;

            if (modelElement is UmlTypeOrPackage)
            {
                var node = ModelToNodeTranslator.Translate(modelElement as UmlTypeOrPackage);
                _graph.AddVertex(node);
                shape = node;
            }
            else if (modelElement is UmlRelationship)
            {
                var connector = ModelToConnectorTranslator.Translate(_graph, modelElement as UmlRelationship);
                _graph.AddEdge(connector);
                shape = connector;
            }

            Layout();

            if (shape != null && ShapeAdded != null)
                ShapeAdded(this, shape);
        }

        public void HideModelElement(UmlModelElement modelElement)
        {
            var node = _graph.FindNode(modelElement);
            _graph.RemoveVertex(node);

            Layout();

            if (node != null && ShapeRemoved != null)
                ShapeRemoved(this, node);
        }

        public void Layout()
        {
            var algorithm = new SimpleTreeLayoutAlgorithm(_graph);
            var newVertexPositions = algorithm.ComputeNewVertexPositions();

            _graph.PositionNodes(newVertexPositions);
        }
    }
}
