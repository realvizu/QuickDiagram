using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Modeling.Implementation
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

        protected ImmutableModel(ImmutableModelGraph graph)
        {
            _graph = graph;
        }

        public IEnumerable<IModelNode> Nodes => _graph.Vertices;
        public IEnumerable<IModelRelationship> Relationships => _graph.Edges;

        // TODO: implement node hierarchy
        public IEnumerable<IModelNode> RootNodes => Nodes;

        // TODO: implement node hierarchy
        public IEnumerable<IModelNode> GetChildNodes(ModelItemId parentNodeId) => throw new NotImplementedException();

        public IModelNode GetModelNode(ModelItemId id) => Nodes.FirstOrDefault(i => i.Id == id);

        public IEnumerable<IModelRelationship> GetRelationships(ModelItemId modelNodeId)
        {
            var modelNode = GetModelNode(modelNodeId);
            if (modelNode == null)
                return Enumerable.Empty<IModelRelationship>();

            return _graph.GetAllEdges(modelNode);
        }

        public IEnumerable<IModelNode> GetRelatedNodes(ModelItemId modelNodeId, DirectedModelRelationshipType directedModelRelationshipType, bool recursive = false)
        {
            var modelNode = GetModelNode(modelNodeId);
            if (modelNode == null)
                return Enumerable.Empty<IModelNode>();

            return _graph.GetConnectedVertices(modelNode,
                (node, relationship) => relationship.IsNodeInRelationship(node, directedModelRelationshipType),
                recursive);
        }

        // TODO: implement node hierarchy
        public ImmutableModel AddNode(ModelNodeBase node, ModelNodeBase parentNode = null) => 
            CreateClone(_graph.AddVertex(node));

        public ImmutableModel RemoveNode(ModelNodeBase node) =>
            CreateClone(_graph.RemoveVertex(node));

        public ImmutableModel AddRelationship(ModelRelationshipBase relationship) =>
            CreateClone(_graph.AddEdge(relationship));

        public ImmutableModel RemoveRelationship(ModelRelationshipBase relationship) =>
            CreateClone(_graph.RemoveEdge(relationship));

        public ImmutableModel UpdateNode(ModelNodeBase oldNode, ModelNodeBase newNode)
        {
            if (oldNode.Id != newNode.Id)
                throw new InvalidOperationException($"Cannot update node with id {oldNode.Id} to node with id {newNode.Id}");

            return CreateClone(_graph.ReplaceVertex(oldNode, newNode));
        }

        protected virtual ImmutableModel CreateClone(ImmutableModelGraph graph) => 
            new ImmutableModel(graph);
    }
}
