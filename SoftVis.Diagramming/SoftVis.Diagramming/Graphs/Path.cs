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
        private readonly TEdge[] _edges;

        public Path(TEdge edge)
            : this(edge.ToEnumerable())
        {
        } 

        public Path(IEnumerable<TEdge> edges)
        {
            _edges = edges as TEdge[] ?? edges.ToArray();

            CheckInvariant();
        }

        public int Length => _edges.Length;
        public TVertex Source => _edges.FirstOrDefault()?.Source;
        public TVertex Target => _edges.LastOrDefault()?.Target;
        public TEdge this[int i] => _edges[i];

        public IEnumerator<TEdge> GetEnumerator()
        {
            return _edges.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void CheckInvariant()
        {
            for (var i = 0; i < _edges.Length - 1; i++)
                if (_edges[i].Target != _edges[i + 1].Source)
                    throw new Exception("The edges don't form a path.");
        }

    }
}
