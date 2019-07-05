using System;
using System.Collections.Immutable;
using Codartis.SoftVis.Graphs.Immutable;

namespace Codartis.SoftVis.Modeling.Implementation
{
    /// <summary>
    /// An immutable graph that represents model nodes and relationships. Mutators return a new graph.
    /// </summary>
    // TODO: should be able to remove onFaith: true
    [Immutable(onFaith: true)]
    public sealed class ModelGraph
        : UpdatableImmutableBidirectionalGraph<IModelNode, ModelNodeId, IModelRelationship, ModelRelationshipId, ModelGraph>
    {
        public ModelGraph()
            : base(allowParallelEdges: true)
        {
        }

        private ModelGraph(
            ImmutableDictionary<ModelNodeId, IModelNode> vertices,
            ImmutableDictionary<ModelRelationshipId, IModelRelationship> edges,
            ImmutableBidirectionalGraph<ModelNodeId, VertexIdEdge<ModelNodeId, ModelRelationshipId>> graph)
            : base(vertices, edges, graph)
        {
        }

        protected override ModelGraph CreateInstance(
            ImmutableDictionary<ModelNodeId, IModelNode> vertices,
            ImmutableDictionary<ModelRelationshipId, IModelRelationship> edges,
            ImmutableBidirectionalGraph<ModelNodeId, VertexIdEdge<ModelNodeId, ModelRelationshipId>> graph)
        {
            return new ModelGraph(vertices, edges, graph);
        }
    }
}