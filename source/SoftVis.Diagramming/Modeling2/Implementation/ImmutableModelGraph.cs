using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Graphs;
using QuickGraph;

namespace Codartis.SoftVis.Modeling2.Implementation
{
    /// <summary>
    /// An immutable graph of model nodes and relationships. Mutators return a new graph.
    /// </summary>
    public class ImmutableModelGraph : IBidirectionalGraph<IModelNode, ModelRelationship>
    {
        private readonly ImmutableBidirectionalGraph<IModelNode, ModelRelationship> _graph;

        private ImmutableModelGraph(ImmutableBidirectionalGraph<IModelNode, ModelRelationship> graph)
        {
            _graph = graph;
        }

        public ImmutableModelGraph()
            : this(new ImmutableBidirectionalGraph<IModelNode, ModelRelationship>(allowParallelEdges: true))
        {
        }

        public IEnumerable<IModelNode> GetConnectedVertices(IModelNode vertex, Func<IModelNode, ModelRelationship, bool> edgePredicate, bool recursive = false)
        {
            return _graph.ContainsVertex(vertex)
                ? GetConnectedVerticesRecursive(vertex, edgePredicate, recursive)
                : Enumerable.Empty<IModelNode>();
        }

        public ImmutableModelGraph ReplaceVertex(IModelNode oldVertex, IModelNode newVertex)
            => new ImmutableModelGraph(_graph.ReplaceVertex(oldVertex, newVertex));

        public bool IsDirected => _graph.IsDirected;
        public bool AllowParallelEdges => _graph.AllowParallelEdges;
        public bool ContainsVertex(IModelNode vertex) => _graph.ContainsVertex(vertex);
        public bool IsOutEdgesEmpty(IModelNode v) => _graph.IsOutEdgesEmpty(v);
        public int OutDegree(IModelNode v) => _graph.OutDegree(v);
        public IEnumerable<ModelRelationship> OutEdges(IModelNode v) => _graph.OutEdges(v);
        public bool TryGetOutEdges(IModelNode v, out IEnumerable<ModelRelationship> edges) => _graph.TryGetOutEdges(v, out edges);
        public ModelRelationship OutEdge(IModelNode v, int index) => _graph.OutEdge(v, index);
        public bool ContainsEdge(IModelNode source, IModelNode target) => _graph.ContainsEdge(source, target);
        public bool TryGetEdges(IModelNode source, IModelNode target, out IEnumerable<ModelRelationship> edges) => _graph.TryGetEdges(source, target, out edges);
        public bool TryGetEdge(IModelNode source, IModelNode target, out ModelRelationship edge) => _graph.TryGetEdge(source, target, out edge);
        public bool IsVerticesEmpty => _graph.IsVerticesEmpty;
        public int VertexCount => _graph.VertexCount;
        public IEnumerable<IModelNode> Vertices => _graph.Vertices;
        public bool ContainsEdge(ModelRelationship edge) => _graph.ContainsEdge(edge);
        public bool IsEdgesEmpty => _graph.IsEdgesEmpty;
        public int EdgeCount => _graph.EdgeCount;
        public IEnumerable<ModelRelationship> Edges => _graph.Edges;
        public bool IsInEdgesEmpty(IModelNode v) => _graph.IsInEdgesEmpty(v);
        public int InDegree(IModelNode v) => _graph.InDegree(v);
        public IEnumerable<ModelRelationship> InEdges(IModelNode v) => _graph.InEdges(v);
        public bool TryGetInEdges(IModelNode v, out IEnumerable<ModelRelationship> edges) => _graph.TryGetInEdges(v, out edges);
        public ModelRelationship InEdge(IModelNode v, int index) => _graph.InEdge(v, index);
        public int Degree(IModelNode v) => _graph.Degree(v);

        private IEnumerable<IModelNode> GetConnectedVerticesRecursive(IModelNode vertex, Func<IModelNode, ModelRelationship, bool> edgePredicate, bool recursive = false)
        {
            var connectedVertices = this.GetAllEdges(vertex)
                .Where(edge => edgePredicate(vertex, edge))
                .Select(edge => edge.GetOtherEnd(vertex))
                .Distinct();

            foreach (var connectedVertex in connectedVertices)
            {
                yield return connectedVertex;

                // TODO: loop detection!
                if (recursive)
                    foreach (var nextConnectedVertex in GetConnectedVertices(connectedVertex, edgePredicate, recursive: true))
                        yield return nextConnectedVertex;
            }
        }

    }
}
