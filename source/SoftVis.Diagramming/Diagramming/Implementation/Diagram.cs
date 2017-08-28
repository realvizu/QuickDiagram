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

        public bool NodeExistsById(ModelNodeId modelNodeId) => _graph.ContainsVertexId(modelNodeId);
        public bool ConnectorExistsById(ModelRelationshipId modelRelationshipId) => _graph.ContainsEdgeId(modelRelationshipId);
        public bool PathExistsById(ModelNodeId sourceModelNodeId, ModelNodeId targetModelNodeId)
            => NodeExistsById(sourceModelNodeId)
            && NodeExistsById(targetModelNodeId)
            && _graph.PathExistsById(sourceModelNodeId, targetModelNodeId);

        public bool IsConnectorRedundantById(ModelRelationshipId modelRelationshipId) 
            => TryGetConnectorById(modelRelationshipId, out var connector) 
            && _graph.IsEdgeRedundant(connector);

        public IDiagramNode GetNodeById(ModelNodeId modelNodeId) => _graph.GetVertexById(modelNodeId);
        public bool TryGetNodeById(ModelNodeId modelNodeId, out IDiagramNode node)
            => _graph.TryGetVertexById(modelNodeId, out node);

        public IDiagramConnector GetConnectorById(ModelRelationshipId modelRelationshipId) => _graph.GetEdgeById(modelRelationshipId);
        public bool TryGetConnectorById(ModelRelationshipId modelRelationshipId, out IDiagramConnector connector)
            => _graph.TryGetEdgeById(modelRelationshipId, out connector);

        public IEnumerable<IDiagramConnector> GetConnectorsByNodeId(ModelNodeId id)
            => Connectors.Where(i => i.Source.Id == id || i.Target.Id == id);

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
