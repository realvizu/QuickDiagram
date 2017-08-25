using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Codartis.SoftVis.Graphs;
using Codartis.SoftVis.Graphs.Immutable;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;
using QuickGraph;
using QuickGraph.Algorithms;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// An immutable graph of digaram nodes and connectors. Mutators return a new graph.
    /// </summary>
    public class DiagramGraph
        : ReplaceableImmutableBidirectionalGraph<IDiagramNode, ModelNodeId, IDiagramConnector, ModelRelationshipId, DiagramGraph>

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
