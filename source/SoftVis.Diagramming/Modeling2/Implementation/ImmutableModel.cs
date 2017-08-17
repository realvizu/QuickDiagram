using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Modeling2.Implementation
{
    /// <summary>
    /// An immutable implementation of a model.
    /// </summary>
    public class ImmutableModel : IModel
    {
        private readonly ImmutableModelGraph _graph;

        public ImmutableModel() 
            : this(new ImmutableModelGraph())
        {
        }

        private ImmutableModel(ImmutableModelGraph graph)
        {
            _graph = graph;
        }

        public IEnumerable<IModelNode> Nodes => _graph.Vertices;
        public IEnumerable<IModelRelationship> Relationships => _graph.Edges;

        // TODO: implement node hierarchy
        public IEnumerable<IModelNode> RootNodes => Nodes;

        // TODO: implement node hierarchy
        public IEnumerable<IModelNode> GetChildNodes(ModelItemId parentNodeId) => throw new NotImplementedException();

        public IModelNode GetModelNode(ModelItemId id)
        {
            var result = Nodes.SingleOrDefault(i => i.Id == id);
            if (result == null)
                throw new InvalidOperationException($"Node with id {id} not found in the model.");
            return result;
        }

        public IEnumerable<IModelRelationship> GetRelationships(ModelItemId modelNodeId)
            => _graph.GetAllEdges(GetModelNode(modelNodeId));

        public IEnumerable<IModelNode> GetRelatedNodes(ModelItemId modelNodeId, DirectedModelRelationshipType directedModelRelationshipType, bool recursive = false)
        {
            var modelNode = GetModelNode(modelNodeId);
            return _graph.GetConnectedVertices(modelNode,
                (_, relationship) => relationship.IsNodeInRelationship(modelNode, directedModelRelationshipType),
                recursive);
        }

        // TODO: implement node hierarchy
        public ImmutableModel AddNode(ModelNodeBase node, ModelNodeBase parentNode = null) => 
            new ImmutableModel(_graph.AddVertex(node));

        public ImmutableModel RemoveNode(ModelNodeBase node) =>
            new ImmutableModel(_graph.RemoveVertex(node));

        public ImmutableModel AddRelationship(ModelRelationshipBase relationship) => 
            new ImmutableModel(_graph.AddEdge(relationship));

        public ImmutableModel RemoveRelationship(ModelRelationshipBase relationship) => 
            new ImmutableModel(_graph.RemoveEdge(relationship));

        public ImmutableModel UpdateNode(ModelNodeBase oldNode, ModelNodeBase newNode)
        {
            if (oldNode.Id != newNode.Id)
                throw new InvalidOperationException($"Cannot update node with id {oldNode.Id} to node with id {newNode.Id}");

            return new ImmutableModel(_graph.ReplaceVertex(oldNode, newNode));
        }
    }
}
