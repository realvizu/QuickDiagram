using System;
using System.Collections.Generic;
using System.Linq;
using QuickGraph;

namespace Codartis.SoftVis.Graphs
{
    /// <summary>
    /// A simple wrapper for BidirectionalGraph that synchronizes concurrent access to the graph with a lock.
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertices.</typeparam>
    /// <typeparam name="TEdge">The type of the edges.</typeparam>
    public class ConcurrentBidirectionalGraph<TVertex, TEdge> : INotifyGraphChange<TVertex, TEdge>, IDisposable
        where TEdge : IEdge<TVertex>
    {
        private readonly object _lockObject = new object();
        protected readonly BidirectionalGraph<TVertex, TEdge> Graph;

        public event VertexAction<TVertex> VertexAdded;
        public event VertexAction<TVertex> VertexRemoved;
        public event EdgeAction<TVertex, TEdge> EdgeAdded;
        public event EdgeAction<TVertex, TEdge> EdgeRemoved;
        public event EventHandler Cleared;

        public ConcurrentBidirectionalGraph(bool allowParallelEdges = true)
        {
            Graph = new BidirectionalGraph<TVertex, TEdge>(allowParallelEdges);
            SubscribeToGraphEvents();
        }

        public void Dispose()
        {
            UnsubscribeFromGraphEvents();
        }

        public bool AddVertex(TVertex v)
        {
            lock (_lockObject)
                return Graph.AddVertex(v);
        }

        public bool AddEdge(TEdge e)
        {
            lock (_lockObject)
                return Graph.AddEdge(e);
        }

        public int AddVertexRange(IEnumerable<TVertex> vertices)
        {
            lock (_lockObject)
                return Graph.AddVertexRange(vertices);
        }

        public bool AddVerticesAndEdge(TEdge e)
        {
            lock (_lockObject)
                return Graph.AddVerticesAndEdge(e);
        }

        public bool RemoveVertex(TVertex v)
        {
            lock (_lockObject)
                return Graph.RemoveVertex(v);
        }

        public bool RemoveEdge(TEdge e)
        {
            lock (_lockObject)
                return Graph.RemoveEdge(e);
        }

        public void Clear()
        {
            lock (_lockObject)
                Graph.Clear();
        }

        public TVertex[] Vertices
        {
            get
            {
                lock (_lockObject)
                    return Graph.Vertices.ToArray();
            }
        }

        public TEdge[] Edges
        {
            get
            {
                lock (_lockObject)
                    return Graph.Edges.ToArray();
            }
        }

        public int VertexCount
        {
            get
            {
                lock (_lockObject)
                    return Graph.VertexCount;
            }
        }

        public int EdgeCount
        {
            get
            {
                lock (_lockObject)
                    return Graph.EdgeCount;
            }
        }

        public TVertex GetOrAddVertex(VertexPredicate<TVertex> vertexPredicate, Func<TVertex> createVertexFunc)
        {
            lock (_lockObject)
            {
                var vertex = Graph.Vertices.FirstOrDefault(i => vertexPredicate(i));
                if (vertex != null)
                    return vertex;

                vertex = createVertexFunc();
                AddVertex(vertex);
                return vertex;
            }
        }

        public TEdge GetOrAddEdge(EdgePredicate<TVertex, TEdge> edgePredicate, Func<TEdge> createEdgeFunc)
        {
            lock (_lockObject)
            {
                var edge = Graph.Edges.FirstOrDefault(i => edgePredicate(i));
                if (edge != null)
                    return edge;

                edge = createEdgeFunc();
                AddEdge(edge);
                return edge;
            }
        }

        public TEdge[] GetEdgesByVertex(TVertex vertex)
        {
            lock (_lockObject)
                return Graph.GetAllEdges(vertex).ToArray();
        }

        public TVertex[] GetConnectedVertices(TVertex vertex, Func<TVertex, TEdge, bool> edgePredicate, bool recursive = false)
        {
            lock (_lockObject)
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

        private void SubscribeToGraphEvents()
        {
            Graph.VertexAdded += PropagateVertexAdded;
            Graph.VertexRemoved += PropagateVertexRemoved;
            Graph.EdgeAdded += PropagateEdgeAdded;
            Graph.EdgeRemoved += PropagateEdgeRemoved;
            Graph.Cleared += PropagateCleared;
        }

        private void UnsubscribeFromGraphEvents()
        {
            Graph.VertexAdded -= PropagateVertexAdded;
            Graph.VertexRemoved -= PropagateVertexRemoved;
            Graph.EdgeAdded -= PropagateEdgeAdded;
            Graph.EdgeRemoved -= PropagateEdgeRemoved;
            Graph.Cleared -= PropagateCleared;
        }

        private void PropagateVertexAdded(TVertex vertex) => VertexAdded?.Invoke(vertex);
        private void PropagateVertexRemoved(TVertex vertex) => VertexRemoved?.Invoke(vertex);
        private void PropagateEdgeAdded(TEdge edge) => EdgeAdded?.Invoke(edge);
        private void PropagateEdgeRemoved(TEdge edge) => EdgeRemoved?.Invoke(edge);
        private void PropagateCleared(object s, EventArgs e) => Cleared?.Invoke(s, e);
    }
}
