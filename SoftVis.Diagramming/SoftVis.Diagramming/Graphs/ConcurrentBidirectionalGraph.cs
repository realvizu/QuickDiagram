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
    public class ConcurrentBidirectionalGraph<TVertex, TEdge> : BidirectionalGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        private readonly object _lockObject = new object();

        public ConcurrentBidirectionalGraph(bool allowParallelEdges = true) 
            : base(allowParallelEdges)
        {
        }

        public override bool AddVertex(TVertex v)
        {
            lock (_lockObject)
                return base.AddVertex(v);
        }

        public override bool AddEdge(TEdge e)
        {
            lock (_lockObject)
                return base.AddEdge(e);
        }

        public override int AddVertexRange(IEnumerable<TVertex> vertices)
        {
            lock (_lockObject)
                return base.AddVertexRange(vertices);
        }

        public override bool AddVerticesAndEdge(TEdge e)
        {
            lock (_lockObject)
                return base.AddVerticesAndEdge(e);
        }

        public override bool RemoveVertex(TVertex v)
        {
            lock (_lockObject)
                return base.RemoveVertex(v);
        }

        public override bool RemoveEdge(TEdge e)
        {
            lock (_lockObject)
                return base.RemoveEdge(e);
        }

        public override IEnumerable<TVertex> Vertices
        {
            get
            {
                lock (_lockObject)
                    return base.Vertices.ToArray();
            }
        }

        public override IEnumerable<TEdge> Edges
        {
            get
            {
                lock (_lockObject)
                    return base.Edges.ToArray();
            }
        }
    }
}
