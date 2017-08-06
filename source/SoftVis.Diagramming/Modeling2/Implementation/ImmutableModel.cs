using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Modeling2.Implementation
{
    /// <summary>
    /// An immutable implementation of a model.
    /// </summary>
    public class ImmutableModel : IModel
    {
        private readonly ImmutableList<ImmutableModelNode> _rootNodes;
        private readonly ImmutableModelGraph _graph;

        public ImmutableModel(ImmutableList<ImmutableModelNode> rootNodes, ImmutableModelGraph graph)
        {
            _rootNodes = rootNodes;
            _graph = graph;
        }

        public ImmutableModel()
            : this(ImmutableList<ImmutableModelNode>.Empty, new ImmutableModelGraph())
        {
        }

        public IEnumerable<IModelNode> RootNodes => _rootNodes;
        public IEnumerable<IModelNode> Nodes => _graph.Vertices;
        public IEnumerable<IModelRelationship> Relationships => _graph.Edges;

        public IModelNode GetModelNode(ModelItemId id)
        {
            var result = Nodes.SingleOrDefault(i => i.Id == id);
            if (result == null)
                throw new InvalidOperationException($"Node with id {id} not found in the model.");
            return result;
        }

        public IEnumerable<IModelRelationship> GetRelationships(ModelItemId modelNodeId)
            => _graph.GetAllEdges(GetModelNode(modelNodeId));

        public IEnumerable<IModelNode> GetRelatedNodes(ModelItemId modelNodeId, Type relationshipType, bool recursive = false)
            => _graph.GetConnectedVertices(GetModelNode(modelNodeId),
                // TODO: is IsInstanceOfType working for subtypes?
                (_, relationship) => relationshipType.IsInstanceOfType(relationship),
                recursive);

        public ImmutableModel AddNode(ImmutableModelNode node, ImmutableModelNode parentNode = null)
        {
            if (parentNode == null)
                return new ImmutableModel(_rootNodes.Add(node), _graph.AddVertex(node));

            throw new NotImplementedException("Node hierarchy handling is not yet implemented.");
            //var newParentNode = parentNode.AddChildNode(node);
            //var newRootNodes = ReplaceNodeInTreeRecursive(_rootNodes, parentNode, newParentNode);
            //var newModelGraph = ReplaceVertexInModelGraph(parentNode, newParentNode);
            //return new ImmutableModel(newRootNodes, newModelGraph);
        }

        public ImmutableModel AddRelationship(ModelRelationship relationship)
        {
            return new ImmutableModel(_rootNodes, _graph.AddEdge(relationship));
        }

        private ImmutableModelGraph ReplaceVertexInModelGraph(ImmutableModelNode parentNode, ImmutableModelNode newParentNode)
            => _graph.ReplaceVertex(parentNode, newParentNode);
    }
}
