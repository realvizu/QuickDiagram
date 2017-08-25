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

        public bool NodeExists(IDiagramNode node) => _graph.ContainsVertex(node);
        public bool NodeExistsById(ModelNodeId modelNodeId) => _graph.ContainsVertexId(modelNodeId);
        public bool ConnectorExists(IDiagramConnector connector) => _graph.ContainsEdge(connector);
        public bool ConnectorExistsById(ModelRelationshipId modelRelationshipId) => _graph.ContainsEdgeId(modelRelationshipId);
        public bool PathExists(IDiagramNode sourceNode, IDiagramNode targetNode) => _graph.PathExists(sourceNode, targetNode);
        public bool PathExistsById(ModelNodeId sourceModelNodeId, ModelNodeId targetModelNodeIdNode)
            => NodeExistsById(sourceModelNodeId)
            && NodeExistsById(targetModelNodeIdNode)
            && _graph.PathExists(GetNodeById(sourceModelNodeId), GetNodeById(targetModelNodeIdNode));

        public IDiagramNode GetNodeById(ModelNodeId modelNodeId) => _graph.GetVertexById(modelNodeId);
        public IDiagramConnector GetConnectorById(ModelRelationshipId modelRelationshipId) => _graph.GetEdgeById(modelRelationshipId);

        public IDiagram AddNode(IDiagramNode node) => CreateInstance(_graph.AddVertex(node));
        public IDiagram RemoveNode(IDiagramNode node) => CreateInstance(_graph.RemoveVertex(node));
        public IDiagram ReplaceNode(IDiagramNode oldNode, IDiagramNode newNode) => CreateInstance(_graph.ReplaceVertex(oldNode, newNode));
        public IDiagram AddConnector(IDiagramConnector connector) => CreateInstance(_graph.AddEdge(connector));
        public IDiagram RemoveConnector(IDiagramConnector connector) => CreateInstance(_graph.RemoveEdge(connector));
        public IDiagram ReplaceConnector(IDiagramConnector oldConnector, IDiagramConnector newConnector)
            => CreateInstance(_graph.ReplaceEdge(oldConnector, newConnector));
        public IDiagram Clear() => CreateInstance(new DiagramGraph());

        protected virtual IDiagram CreateInstance(DiagramGraph graph) => new Diagram(graph);
    }
}
