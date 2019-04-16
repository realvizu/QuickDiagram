using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.Util;
using QuickGraph;

namespace Codartis.SoftVis.Graphs.Concurrent
{
    /// <summary>
    /// A simple wrapper for BidirectionalGraph that synchronizes concurrent access to the graph with a lock.
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertices.</typeparam>
    /// <typeparam name="TEdge">The type of the edges.</typeparam>
    public class ConcurrentBidirectionalGraph<TVertex, TEdge> : INotifyGraphChange<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        protected readonly BidirectionalGraph<TVertex, TEdge> Graph;

        public object SyncRoot { get; }

        public event VertexAction<TVertex> VertexAdded;
        public event VertexAction<TVertex> VertexRemoved;
        public event EdgeAction<TVertex, TEdge> EdgeAdded;
        public event EdgeAction<TVertex, TEdge> EdgeRemoved;
        public event EventHandler Cleared;

        public ConcurrentBidirectionalGraph(bool allowParallelEdges = true)
        {
            Graph = new BidirectionalGraph<TVertex, TEdge>(allowParallelEdges);
            SyncRoot = new object();
        }

        public bool AddVertex(TVertex vertex)
        {
            bool vertexWasAdded;

            lock (SyncRoot)
                vertexWasAdded = Graph.AddVertex(vertex);

            if (vertexWasAdded)
                VertexAdded?.Invoke(vertex);

            return vertexWasAdded;
        }

        public bool AddEdge(TEdge edge)
        {
            bool edgeWasAdded;

            lock (SyncRoot)
                edgeWasAdded = Graph.AddEdge(edge);

            if (edgeWasAdded)
                EdgeAdded?.Invoke(edge);

            return edgeWasAdded;
        }

        public RemoveVertexResult<TVertex, TEdge> RemoveVertex(TVertex vertex)
        {
            bool vertexWasRemoved;
            TEdge[] edgesOfRemovedVertex;

            lock (SyncRoot)
            {
                edgesOfRemovedVertex = Graph.GetAllEdges(vertex).ToArray();
                vertexWasRemoved = Graph.RemoveVertex(vertex);
            }

            if (!vertexWasRemoved)
                return RemoveVertexResult<TVertex, TEdge>.Empty;

            foreach (var edge in edgesOfRemovedVertex)
                EdgeRemoved?.Invoke(edge);

            VertexRemoved?.Invoke(vertex);

            return new RemoveVertexResult<TVertex, TEdge>(vertex, edgesOfRemovedVertex);
        }

        public RemoveVertexResult<TVertex, TEdge> RemoveVertex(VertexPredicate<TVertex> vertexPredicate)
        {
            TVertex vertex;
            lock (SyncRoot)
                vertex = Graph.Vertices.FirstOrDefault(i => vertexPredicate(i));

            if (vertex == null)
                return RemoveVertexResult<TVertex, TEdge>.Empty;

            return this.RemoveVertex(vertex);
        }

        public bool RemoveEdge(TEdge edge)
        {
            bool edgeWasRemoved;

            lock (SyncRoot)
                edgeWasRemoved = Graph.RemoveEdge(edge);

            if (edgeWasRemoved)
                EdgeRemoved?.Invoke(edge);

            return edgeWasRemoved;
        }

        public bool RemoveEdge(EdgePredicate<TVertex, TEdge> edgePredicate)
        {
            TEdge edge;

            lock (SyncRoot)
                edge = Graph.Edges.FirstOrDefault(i => edgePredicate(i));

            if (edge == null)
                return false;

            return this.RemoveEdge(edge);
        }

        public void Clear()
        {
            lock (SyncRoot)
                Graph.Clear();

            Cleared?.Invoke(this, EventArgs.Empty);
        }

        public IReadOnlyList<TVertex> Vertices
        {
            get
            {
                lock (SyncRoot)
                    return Graph.Vertices.ToArray();
            }
        }

        public IReadOnlyList<TEdge> Edges
        {
            get
            {
                lock (SyncRoot)
                    return Graph.Edges.ToArray();
            }
        }

        public int VertexCount
        {
            get
            {
                lock (SyncRoot)
                    return Graph.VertexCount;
            }
        }

        public int EdgeCount
        {
            get
            {
                lock (SyncRoot)
                    return Graph.EdgeCount;
            }
        }

        public GetOrAddResult<TVertex> GetOrAddVertex(VertexPredicate<TVertex> vertexPredicate, Func<TVertex> createVertexFunc)
        {
            TVertex vertex;

            lock (SyncRoot)
            {
                vertex = Graph.Vertices.FirstOrDefault(i => vertexPredicate(i));
                if (vertex != null)
                    return new GetOrAddResult<TVertex>(vertex, GetOrAddAction.Get);

                vertex = createVertexFunc();
                Graph.AddVertex(vertex);
            }

            VertexAdded?.Invoke(vertex);

            return new GetOrAddResult<TVertex>(vertex, GetOrAddAction.Add);
        }

        public GetOrAddResult<TEdge> GetOrAddEdge(EdgePredicate<TVertex, TEdge> edgePredicate, Func<TEdge> createEdgeFunc)
        {
            TEdge edge;

            lock (SyncRoot)
            {
                edge = Graph.Edges.FirstOrDefault(i => edgePredicate(i));
                if (edge != null)
                    return new GetOrAddResult<TEdge>(edge, GetOrAddAction.Get);

                edge = createEdgeFunc();
                Graph.AddEdge(edge);
            }

            EdgeAdded?.Invoke(edge);

            return new GetOrAddResult<TEdge>(edge, GetOrAddAction.Add);
        }

        public IReadOnlyList<TEdge> GetEdgesByVertex(TVertex vertex)
        {
            lock (SyncRoot)
                return Graph.GetAllEdges(vertex).ToArray();
        }

        public IReadOnlyList<TVertex> GetConnectedVertices(TVertex vertex, Func<TVertex, TEdge, bool> edgePredicate, bool recursive = false)
        {
            lock (SyncRoot)
            {
                if (!Graph.ContainsVertex(vertex))
                    return new TVertex[0];

                return GetConnectedVerticesRecursive(vertex, edgePredicate, recursive).ToArray();
            }
        }

        private IEnumerable<TVertex> GetConnectedVerticesRecursive(TVertex vertex, Func<TVertex, TEdge, bool> vertexAndEdgePredicate, bool recursive = false)
        {
            var connectedVertices = GetEdgesByVertex(vertex)
                .Where(edge => vertexAndEdgePredicate(vertex, edge))
                .Select(edge => edge.GetOtherEnd(vertex))
                .Distinct();

            foreach (var connectedVertex in connectedVertices)
            {
                yield return connectedVertex;

                // TODO: loop detection!
                if (recursive)
                    foreach (var nextConnectedVertex in GetConnectedVertices(connectedVertex, vertexAndEdgePredicate, recursive: true))
                        yield return nextConnectedVertex;
            }
        }
    }
}
