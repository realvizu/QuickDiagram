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
        protected readonly ModelGraph Graph;

        public Model()
            : this(new ModelGraph())
        {
        }

        protected Model(ModelGraph graph)
        {
            Graph = graph;
        }

        public IEnumerable<IModelNode> Nodes => Graph.Vertices;
        public IEnumerable<IModelRelationship> Relationships => Graph.Edges;

        // TODO: implement node hierarchy
        public IEnumerable<IModelNode> RootNodes => Nodes;

        public IModelNode GetNodeById(ModelNodeId nodeId) => Graph.GetVertexById(nodeId);
        public bool TryGetNodeById(ModelNodeId nodeId, out IModelNode node) => Graph.TryGetVertexById(nodeId, out node);

        // TODO: implement node hierarchy
        public IEnumerable<IModelNode> GetChildNodes(IModelNode node) => throw new NotImplementedException();

        public IEnumerable<IModelNode> GetRelatedNodes(IModelNode node,
            DirectedModelRelationshipType directedModelRelationshipType, bool recursive = false)
        {
            return Graph.GetConnectedVerticesById(node,
                (otherNode, relationship) => IsNodeRelatedById(relationship, otherNode, directedModelRelationshipType),
                recursive);
        }

        public IEnumerable<IModelRelationship> GetRelationships(IModelNode node) => Graph.GetAllEdges(node);

        public IModel AddNode(IModelNode node) => CreateInstance(Graph.AddVertex(node));
        public IModel RemoveNode(IModelNode node) => CreateInstance(Graph.RemoveVertex(node));
        public IModel ReplaceNode(IModelNode oldNode, IModelNode newNode) => CreateInstance(Graph.ReplaceVertex(oldNode, newNode));
        public IModel AddRelationship(IModelRelationship relationship) => CreateInstance(Graph.AddEdge(relationship));
        public IModel RemoveRelationship(IModelRelationship relationship) => CreateInstance(Graph.RemoveEdge(relationship));
        public IModel Clear() => CreateInstance(new ModelGraph());

        protected virtual IModel CreateInstance(ModelGraph graph) => new Model(graph);

        private static bool IsNodeRelatedById(IModelRelationship relationship, IModelNode modelNode,
        DirectedModelRelationshipType directedModelRelationshipType)
        {
            return relationship.Stereotype == directedModelRelationshipType.Stereotype
                   && ContainsNodeOnGivenEndById(relationship, modelNode, directedModelRelationshipType.Direction);
        }

        private static bool ContainsNodeOnGivenEndById(IModelRelationship relationship, IModelNode modelNode,
            EdgeDirection direction)
        {
            switch (direction)
            {
                case EdgeDirection.Out: return modelNode.Id.Equals(relationship.Source.Id);
                case EdgeDirection.In: return modelNode.Id.Equals(relationship.Target.Id);
                default: throw new ArgumentException($"Unexpected EdgeDirection: {direction}");
            }
        }

    }
}
