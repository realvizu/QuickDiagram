using System;
using System.Collections.Generic;
using Codartis.SoftVis.Graphs.Immutable;

namespace Codartis.SoftVis.Modeling.Implementation
{
    using IModelGraph = IImmutableBidirectionalGraph<IModelNode, ModelNodeId, IModelRelationship, ModelRelationshipId>;
    using ModelGraph = ImmutableBidirectionalGraph<IModelNode, ModelNodeId, IModelRelationship, ModelRelationshipId>;

    /// <summary>
    /// Implements an immutable model.
    /// </summary>
    public class Model : IModel
    {
        protected readonly IModelGraph Graph;

        public Model()
            : this(ModelGraph.Empty(allowParallelEdges: false))
        {
        }

        protected Model(IModelGraph graph)
        {
            Graph = graph;
        }

        public IEnumerable<IModelNode> Nodes => Graph.Vertices;
        public IEnumerable<IModelRelationship> Relationships => Graph.Edges;

        public IModelNode GetNode(ModelNodeId nodeId) => Graph.GetVertex(nodeId);
        public bool TryGetNode(ModelNodeId nodeId, out IModelNode node) => Graph.TryGetVertex(nodeId, out node);

        public IModelRelationship GetRelationship(ModelRelationshipId relationshipId) => Graph.GetEdge(relationshipId);
        public bool TryGetRelationship(ModelRelationshipId relationshipId, out IModelRelationship relationship) 
            => Graph.TryGetEdge(relationshipId, out relationship);

        // TODO: implement node hierarchy
        public IEnumerable<IModelNode> GetChildNodes(ModelNodeId nodeId) => throw new NotImplementedException();

        public IEnumerable<IModelNode> GetRelatedNodes(ModelNodeId nodeId,
            DirectedModelRelationshipType directedModelRelationshipType, bool recursive = false)
        {
            return Graph.GetAdjacentVertices(nodeId, directedModelRelationshipType.Direction,
                i => i.Stereotype == directedModelRelationshipType.Stereotype,
                recursive);
        }

        public IEnumerable<IModelRelationship> GetRelationships(ModelNodeId nodeId) => Graph.GetAllEdges(nodeId);

        public IModel AddNode(IModelNode node) => CreateInstance(Graph.AddVertex(node));
        public IModel RemoveNode(ModelNodeId nodeId) => CreateInstance(Graph.RemoveVertex(nodeId));
        public IModel ReplaceNode(IModelNode newNode) => CreateInstance(Graph.UpdateVertex(newNode));
        public IModel AddRelationship(IModelRelationship relationship) => CreateInstance(Graph.AddEdge(relationship));
        public IModel RemoveRelationship(ModelRelationshipId relationshipId) => CreateInstance(Graph.RemoveEdge(relationshipId));
        public IModel Clear() => CreateInstance(ModelGraph.Empty(allowParallelEdges: false));

        protected virtual IModel CreateInstance(IModelGraph graph) => new Model(graph);
    }
}
