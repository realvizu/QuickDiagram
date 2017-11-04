using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// An immutable implementation of a diagram.
    /// </summary>
    public class Diagram : IDiagram
    {
        private readonly DiagramGraph _graph;

        public Diagram()
            : this(new DiagramGraph())
        {
        }

        protected Diagram(DiagramGraph graph)
        {
            _graph = graph;
        }

        public Rect2D ContentRect => Shapes.Where(i => i.IsRectDefined).Select(i => i.Rect).Union();

        public IEnumerable<IDiagramShape> Shapes => Nodes.OfType<IDiagramShape>().Concat(Connectors);
        public IEnumerable<IDiagramNode> Nodes => _graph.Vertices;
        public IEnumerable<IDiagramConnector> Connectors => _graph.Edges;

        public bool NodeExists(ModelNodeId modelNodeId) => _graph.ContainsVertex(modelNodeId);
        public bool ConnectorExists(ModelRelationshipId modelRelationshipId) => _graph.ContainsEdge(modelRelationshipId);
        public bool PathExists(ModelNodeId sourceModelNodeId, ModelNodeId targetModelNodeId)
            => NodeExists(sourceModelNodeId)
            && NodeExists(targetModelNodeId)
            && _graph.PathExists(sourceModelNodeId, targetModelNodeId);

        public bool IsConnectorRedundant(ModelRelationshipId modelRelationshipId)
            => TryGetConnector(modelRelationshipId, out var connector)
            && _graph.IsEdgeRedundant(connector);

        public IDiagramNode GetNode(ModelNodeId modelNodeId) => _graph.GetVertex(modelNodeId);
        public bool TryGetNode(ModelNodeId modelNodeId, out IDiagramNode node)
            => _graph.TryGetVertex(modelNodeId, out node);

        public IDiagramConnector GetConnector(ModelRelationshipId modelRelationshipId) => _graph.GetEdge(modelRelationshipId);
        public bool TryGetConnector(ModelRelationshipId modelRelationshipId, out IDiagramConnector connector)
            => _graph.TryGetEdge(modelRelationshipId, out connector);

        public IEnumerable<IDiagramConnector> GetConnectorsByNode(ModelNodeId id)
            => Connectors.Where(i => i.Source.Id == id || i.Target.Id == id);

        public IEnumerable<IDiagramNode> GetAdjacentNodes(ModelNodeId id, DirectedModelRelationshipType? directedModelRelationshipType = null)
        {
            IEnumerable<IDiagramNode> result;

            if (directedModelRelationshipType != null)
            {
                result = _graph.GetAdjacentVertices(id, directedModelRelationshipType.Value.Direction,
                    e => e.ModelRelationship.Stereotype == directedModelRelationshipType.Value.Stereotype);
            }
            else
            {
                result = _graph.GetAdjacentVertices(id, EdgeDirection.In)
                    .Union(_graph.GetAdjacentVertices(id, EdgeDirection.Out));
            }

            return result;
        }

        public IDiagram AddNode(IDiagramNode node, IContainerDiagramNode parentNode = null)
        {
            var updatedGraph = parentNode == null 
                ? _graph.AddVertex(node) 
                : _graph.UpdateVertex(parentNode.AddChildNode(node));

            return CreateInstance(updatedGraph);
        }

        public IDiagram RemoveNode(ModelNodeId nodeId)
        {
            // TODO: if it's a child node then remove from parent instead of removing from graph
            return CreateInstance(_graph.RemoveVertex(nodeId));
        }

        public IDiagram UpdateNode(IDiagramNode newNode) => CreateInstance(_graph.UpdateVertex(newNode));
        public IDiagram AddConnector(IDiagramConnector connector) => CreateInstance(_graph.AddEdge(connector));
        public IDiagram RemoveConnector(ModelRelationshipId connectorId) => CreateInstance(_graph.RemoveEdge(connectorId));
        public IDiagram UpdateConnector(IDiagramConnector newConnector) => CreateInstance(_graph.UpdateEdge(newConnector));
        public IDiagram Clear() => CreateInstance(new DiagramGraph());

        protected virtual IDiagram CreateInstance(DiagramGraph graph) => new Diagram(graph);
    }
}