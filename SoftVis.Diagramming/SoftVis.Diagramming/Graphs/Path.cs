using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Common;
using MoreLinq;
using QuickGraph;

namespace Codartis.SoftVis.Graphs
{
    /// <summary>
    /// A path is a series of consecutive directed edges.
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertices.</typeparam>
    /// <typeparam name="TEdge">The type of the edges.</typeparam>
    /// <remarks>
    /// Invariants:
    /// <para>The target of an edge is the source of the next edge.</para>
    /// </remarks>
    internal class Path<TVertex, TEdge> : IEnumerable<TEdge>
        where TVertex : class
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
        public IEnumerable<TVertex> Vertices => this.Select(i => i.Source).Concat(this.Last().Target);
        public TEdge this[int i] => Edges[i];

        public IEnumerator<TEdge> GetEnumerator() => Edges.AsEnumerable().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected void CheckInvariant()
        {
            for (var i = 0; i < Length - 1; i++)
            {
                var prevTarget = Edges[i].Target;
                var nextSource = Edges[i + 1].Source;
                if (!prevTarget.Equals(nextSource))
                    throw new PathException($"The edges don't form a path at index {i}: target={prevTarget} but next source={nextSource}");
            }
        }

        public override string ToString()
        {
            return string.Join("->", Vertices);
        }
    }
}
