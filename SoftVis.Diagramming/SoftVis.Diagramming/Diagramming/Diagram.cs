using Codartis.SoftVis.Diagramming.Layout;
using Codartis.SoftVis.Modeling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
        public event EventHandler<DiagramShape> ShapeModified;
        public event EventHandler<DiagramShape> ShapeRemoved;
        public event EventHandler Cleared;

        public IEnumerable<DiagramNode> Nodes => _graph.Vertices;
        public IEnumerable<DiagramConnector> Connectors => _graph.Edges;

        public void Clear()
        {
            _graph = new DiagramGraph();
            SignalClearedEvent();
        }

        public void ShowModelElement(UmlModelElement modelElement)
        {
            // TODO: replace type checks with polimorphism?
            if (modelElement is UmlTypeOrPackage)
            {
                ShowUmlTypeOrPackage((UmlTypeOrPackage)modelElement);
            }
            else if (modelElement is UmlRelationship)
            {
                ShowUmlRelationship((UmlRelationship)modelElement);
            }
        }

        protected virtual DiagramNode CreateDiagramNode(UmlTypeOrPackage umlTypeOrPackage)
        {
            return ModelToDiagramNodeTranslator.Translate(umlTypeOrPackage);
        }

        protected virtual DiagramConnector CreateDiagramConnector(UmlRelationship umlRelationship)
        {
            return ModelToDiagramConnectorTranslator.Translate(_graph, umlRelationship);
        }

        private void ShowUmlTypeOrPackage(UmlTypeOrPackage umlTypeOrPackage)
        {
            if (_graph.Vertices.Any(i => i.ModelElement == umlTypeOrPackage))
                return;

            var node = CreateDiagramNode(umlTypeOrPackage);
            _graph.AddVertex(node);
            SignalShapeAddedEvent(node);

            foreach (var relationship in umlTypeOrPackage.OutgoingRelationships)
            {
                if (_graph.Vertices.Any(i => i.ModelElement == relationship.SourceElement) &&
                    _graph.Vertices.Any(i => i.ModelElement == relationship.TargetElement))
                {
                    ShowModelElement(relationship);
                }
            }
        }

        private void ShowUmlRelationship(UmlRelationship umlRelationship)
        {
            if (_graph.Edges.Any(i => i.ModelElement == umlRelationship))
                return;

            var connector = CreateDiagramConnector(umlRelationship);
            _graph.AddEdge(connector);
            SignalShapeAddedEvent(connector);
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

        public void HideModelElement(UmlModelElement modelElement)
        {
            var node = _graph.FindNode(modelElement);
            _graph.RemoveVertex(node);
            SignalShapeRemovedEvent(node);
        }

        public void Layout()
        {
            var algorithm = new SimpleTreeLayoutAlgorithm(_graph);
            var newVertexPositions = algorithm.ComputeNewVertexPositions();

            _graph.PositionNodes(newVertexPositions);

            foreach (var vertex in _graph.Vertices)
                SignalShapeModifiedEvent(vertex);

            foreach (var edge in _graph.Edges)
                SignalShapeModifiedEvent(edge);
        }
    }
}
