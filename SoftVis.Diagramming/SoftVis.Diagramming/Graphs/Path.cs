using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Common;
using QuickGraph;

namespace Codartis.SoftVis.Graphs
{
    /// <summary>
    /// A path is a series of directed edges where the target of an edge is the source of the next edge.
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertices.</typeparam>
    /// <typeparam name="TEdge">The type of the edges.</typeparam>
    internal class Path<TVertex, TEdge> : IEnumerable<TEdge>
        where TVertex: class
        where TEdge : IEdge<TVertex>
    {
        protected readonly List<TEdge> Edges;

        public Path(TEdge edge)
            : this(edge.ToEnumerable())
        {
        } 

        public Path(IEnumerable<TEdge> edges)
        {
            Edges = edges as List<TEdge> ?? edges.ToList();
            CheckInvariant();
        }

        public int Length => Edges.Count;
        public TVertex Source => Edges.FirstOrDefault()?.Source;
        public TVertex Target => Edges.LastOrDefault()?.Target;
        public TEdge this[int i] => Edges[i];

        public IEnumerator<TEdge> GetEnumerator() => Edges.AsEnumerable().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected void CheckInvariant()
        {
            for (var i = 0; i < Length - 1; i++)
                if (Edges[i].Target != Edges[i + 1].Source)
                    throw new Exception("The edges don't form a path.");
        }

    }
}
