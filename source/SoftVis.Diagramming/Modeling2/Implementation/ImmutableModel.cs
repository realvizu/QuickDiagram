using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Modeling2.Implementation
{
    /// <summary>
    /// An immutable implementation of a model.
    /// </summary>
    internal class ImmutableModel : IModel
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
        public IEnumerable<IModelRelationship> Relationships => _graph.Edges;

        public IEnumerable<IModelRelationship> GetRelationships(IModelNode node)
            => _graph.GetAllEdges(node);

        public IEnumerable<IModelNode> GetRelatedNodes(IModelNode node, Type relationshipType, bool recursive = false)
            => _graph.GetConnectedVertices(node,
                // TODO: is IsInstanceOfType working for subtypes?
                (_, relationship) => relationshipType.IsInstanceOfType(relationship),
                recursive);

        public ImmutableModel AddNode(ImmutableModelNode node, ImmutableModelNode parentNode = null)
        {
            if (parentNode == null)
                return new ImmutableModel(_rootNodes.Add(node), _graph);

            throw new NotImplementedException("Node hierarchy handling is not yet implemented.");
            //var newParentNode = parentNode.AddChildNode(node);
            //var newRootNodes = ReplaceNodeInTreeRecursive(_rootNodes, parentNode, newParentNode);
            //var newModelGraph = ReplaceVertexInModelGraph(parentNode, newParentNode);
            //return new ImmutableModel(newRootNodes, newModelGraph);
        }

        private ImmutableModelGraph ReplaceVertexInModelGraph(ImmutableModelNode parentNode, ImmutableModelNode newParentNode)
            => _graph.ReplaceVertex(parentNode, newParentNode);
    }
}
