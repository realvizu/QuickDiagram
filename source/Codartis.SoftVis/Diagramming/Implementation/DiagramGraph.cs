using System;
using System.Collections.Immutable;
using Codartis.SoftVis.Graphs.Immutable;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// An immutable graph of diagram nodes and connectors. Mutators return a new graph.
    /// </summary>
    // TODO: should be able to remove onFaith: true
    [Immutable(onFaith: true)]
    public class DiagramGraph
        : UpdatableImmutableBidirectionalGraph<IDiagramNode, ModelNodeId, IDiagramConnector, ModelRelationshipId, DiagramGraph>

    {
        public DiagramGraph()
            : base(allowParallelEdges: true)
        {
        }

        protected DiagramGraph(
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
