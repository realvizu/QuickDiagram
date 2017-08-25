using System;
using System.Collections.Generic;
using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Modeling.Implementation
{
    /// <summary>
    /// Implements an immutable model.
    /// </summary>
    public class Model : IModel
    {
        private readonly ModelGraph _graph;

        public Model()
            : this(new ModelGraph())
        {
        }

        protected Model(ModelGraph graph)
        {
            _graph = graph;
        }

        public IEnumerable<IModelNode> Nodes => _graph.Vertices;
        public IEnumerable<IModelRelationship> Relationships => _graph.Edges;

        // TODO: implement node hierarchy
        public IEnumerable<IModelNode> RootNodes => Nodes;

        public bool NodeExists(IModelNode node) => _graph.ContainsVertex(node);
        public bool RelationshipExists(IModelRelationship relationship) => _graph.ContainsEdge(relationship);
        public bool PathExists(IModelNode sourceNode, IModelNode targetNode) => _graph.PathExists(sourceNode, targetNode);

        public IModelNode GetNodeById(ModelNodeId nodeId) => _graph.GetVertexById(nodeId);

        // TODO: implement node hierarchy
        public IEnumerable<IModelNode> GetChildNodes(IModelNode node) => throw new NotImplementedException();

        public IEnumerable<IModelNode> GetRelatedNodes(IModelNode node,
            DirectedModelRelationshipType directedModelRelationshipType, bool recursive = false)
        {
            return _graph.GetConnectedVertices(node,
                (otherNode, relationship) => relationship.IsNodeRelated(otherNode, directedModelRelationshipType),
                recursive);
        }

        public IEnumerable<IModelRelationship> GetRelationships(IModelNode node) => _graph.GetAllEdges(node);

        public IModel AddNode(IModelNode node) => CreateInstance(_graph.AddVertex(node));
        public IModel RemoveNode(IModelNode node) => CreateInstance(_graph.RemoveVertex(node));
        public IModel ReplaceNode(IModelNode oldNode, IModelNode newNode) => CreateInstance(_graph.ReplaceVertex(oldNode, newNode));
        public IModel AddRelationship(IModelRelationship relationship) => CreateInstance(_graph.AddEdge(relationship));
        public IModel RemoveRelationship(IModelRelationship relationship) => CreateInstance(_graph.RemoveEdge(relationship));
        public IModel Clear() => CreateInstance(new ModelGraph());

        protected virtual IModel CreateInstance(ModelGraph graph) => new Model(graph);
    }
}
