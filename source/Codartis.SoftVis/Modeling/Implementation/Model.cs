using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Graphs.Immutable;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Modeling.Implementation
{
    using IModelGraph = IImmutableBidirectionalGraph<IModelNode, ModelNodeId, IModelRelationship, ModelRelationshipId>;
    using ModelGraph = ImmutableBidirectionalGraph<IModelNode, ModelNodeId, IModelRelationship, ModelRelationshipId>;

    /// <summary>
    /// Implements an immutable model.
    /// </summary>
    public sealed class Model : IModel
    {
        [NotNull] public static readonly IModel Empty = new Model(ModelGraph.Empty(allowParallelEdges: false));

        private readonly IModelGraph _graph;

        private Model(IModelGraph graph)
        {
            _graph = graph;
        }

        public IEnumerable<IModelNode> Nodes => _graph.Vertices;
        public IEnumerable<IModelRelationship> Relationships => _graph.Edges;

        public IModelNode GetNode(ModelNodeId nodeId) => _graph.GetVertex(nodeId);
        public Maybe<IModelNode> TryGetNode(ModelNodeId nodeId) => _graph.TryGetVertex(nodeId);

        public Maybe<IModelNode> TryGetParentNode(ModelNodeId modelNodeId)
        {
            var parentNodes = GetRelatedNodes(modelNodeId, CommonDirectedModelRelationshipTypes.Container).ToList();

            if (parentNodes.Count > 1)
                throw new Exception($"There are {parentNodes.Count} parent nodes for node {modelNodeId}.");

            return Maybe.Create(parentNodes.SingleOrDefault());
        }

        public IModelRelationship GetRelationship(ModelRelationshipId relationshipId) => _graph.GetEdge(relationshipId);
        public Maybe<IModelRelationship> TryGetRelationship(ModelRelationshipId relationshipId) => _graph.TryGetEdge(relationshipId);

        public IEnumerable<IModelNode> GetRelatedNodes(
            ModelNodeId nodeId,
            DirectedModelRelationshipType directedModelRelationshipType,
            bool recursive = false)
        {
            return _graph.GetAdjacentVertices(
                nodeId,
                directedModelRelationshipType.Direction,
                i => i.Stereotype == directedModelRelationshipType.Stereotype,
                recursive);
        }

        public IEnumerable<IModelRelationship> GetRelationships(ModelNodeId nodeId) => _graph.GetAllEdges(nodeId);

        public IModel AddNode(IModelNode node) => CreateInstance(_graph.AddVertex(node));
        public IModel RemoveNode(ModelNodeId nodeId) => CreateInstance(_graph.RemoveVertex(nodeId));
        public IModel ReplaceNode(IModelNode newNode) => CreateInstance(_graph.UpdateVertex(newNode));
        public IModel AddRelationship(IModelRelationship relationship) => CreateInstance(_graph.AddEdge(relationship));
        public IModel RemoveRelationship(ModelRelationshipId relationshipId) => CreateInstance(_graph.RemoveEdge(relationshipId));
        public IModel Clear() => Empty;

        private static IModel CreateInstance(IModelGraph graph) => new Model(graph);
    }
}