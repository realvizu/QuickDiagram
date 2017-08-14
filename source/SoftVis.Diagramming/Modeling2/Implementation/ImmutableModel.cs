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
        private readonly ImmutableList<ImmutableModelNodeBase> _rootNodes;
        private readonly ImmutableModelGraph _graph;

        public ImmutableModel(ImmutableList<ImmutableModelNodeBase> rootNodes, ImmutableModelGraph graph)
        {
            _rootNodes = rootNodes;
            _graph = graph;
        }

        public ImmutableModel()
            : this(ImmutableList<ImmutableModelNodeBase>.Empty, new ImmutableModelGraph())
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

        public IEnumerable<IModelNode> GetRelatedNodes(ModelItemId modelNodeId, DirectedModelRelationshipType modelRelationshipType, bool recursive = false)
        {
            var modelNode = GetModelNode(modelNodeId);
            return _graph.GetConnectedVertices(modelNode,
                // TODO: is IsInstanceOfType working for subtypes?
                (_, relationship) => modelRelationshipType.Type.IsInstanceOfType(relationship) && 
                                     relationship.IsNodeInRelationship(modelNode, modelRelationshipType.Direction),
                recursive);
        }

        public ImmutableModel AddNode(ImmutableModelNodeBase node, ImmutableModelNodeBase parentNode = null)
        {
            if (parentNode == null)
                return new ImmutableModel(_rootNodes.Add(node), _graph.AddVertex(node));

            throw new NotImplementedException("Node hierarchy handling is not yet implemented.");
            //var newParentNode = parentNode.AddChildNode(node);
            //var newRootNodes = ReplaceNodeInTreeRecursive(_rootNodes, parentNode, newParentNode);
            //var newModelGraph = ReplaceVertexInModelGraph(parentNode, newParentNode);
            //return new ImmutableModel(newRootNodes, newModelGraph);
        }

        public ImmutableModel AddRelationship(ModelRelationshipBase relationship) => 
            new ImmutableModel(_rootNodes, _graph.AddEdge(relationship));

        public ImmutableModel RemoveRelationship(ModelRelationshipBase relationship) => 
            new ImmutableModel(_rootNodes, _graph.RemoveEdge(relationship));

        private ImmutableModelGraph ReplaceVertexInModelGraph(ImmutableModelNodeBase parentNode, ImmutableModelNodeBase newParentNode)
            => _graph.ReplaceVertex(parentNode, newParentNode);

    }
}
