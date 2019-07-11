using System.Collections.Immutable;
using Codartis.SoftVis.Graphs.Immutable;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// An immutable graph of diagram nodes and connectors. Mutators return a new graph.
    /// </summary>
    public sealed class DiagramGraph
        : UpdatableImmutableBidirectionalGraph<IDiagramNode, ModelNodeId, IDiagramConnector, ModelRelationshipId, DiagramGraph>
    {
        public static DiagramGraph Empty => new DiagramGraph();

        public DiagramGraph()
            : base(allowParallelEdges: true)
        {
        }

        private DiagramGraph(
            ImmutableDictionary<ModelNodeId, IDiagramNode> vertices,
            ImmutableDictionary<ModelRelationshipId, IDiagramConnector> edges,
            ImmutableBidirectionalGraph<ModelNodeId, VertexIdEdge<ModelNodeId, ModelRelationshipId>> graph)
            : base(vertices, edges, graph)
        {
        }

        protected override DiagramGraph CreateInstance(
            ImmutableDictionary<ModelNodeId, IDiagramNode> vertices,
            ImmutableDictionary<ModelRelationshipId, IDiagramConnector> edges,
            ImmutableBidirectionalGraph<ModelNodeId, VertexIdEdge<ModelNodeId, ModelRelationshipId>> graph)
        {
            return new DiagramGraph(vertices, edges, graph);
        }
    }
}