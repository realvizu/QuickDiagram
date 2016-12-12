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
        public ConcurrentBidirectionalGraph(bool allowParallelEdges = true) 
            : base(allowParallelEdges)
        {
        }

        public override bool AddVertex(TVertex v)
        {
            lock (this)
                return base.AddVertex(v);
        }

        public override bool AddEdge(TEdge e)
        {
            lock (this)
                return base.AddEdge(e);
        }

        public override int AddVertexRange(IEnumerable<TVertex> vertices)
        {
            lock (this)
                return base.AddVertexRange(vertices);
        }

        public override bool AddVerticesAndEdge(TEdge e)
        {
            lock (this)
                return base.AddVerticesAndEdge(e);
        }

        public override bool RemoveVertex(TVertex v)
        {
            lock (this)
                return base.RemoveVertex(v);
        }

        public override bool RemoveEdge(TEdge e)
        {
            lock (this)
                return base.RemoveEdge(e);
        }

        public override IEnumerable<TVertex> Vertices
        {
            get
            {
                lock (this)
                    return base.Vertices.ToArray();
            }
        }

        public override IEnumerable<TEdge> Edges
        {
            get
            {
                lock (this)
                    return base.Edges.ToArray();
            }
        }
    }
}
