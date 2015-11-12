using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Graphs;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic
{
    /// <summary>
    /// Abstract base class for graphs that are used for layout calculation. 
    /// </summary>
    /// <remarks>
    /// Responsibilities:
    /// <para>Understands parent/child/sibling relationships.</para>
    /// </remarks>
    internal abstract class LayoutGraphBase<TVertex, TEdge> : BidirectionalGraph<TVertex, TEdge>,
        IReadOnlyLayoutGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        protected LayoutGraphBase()
            : base(allowParallelEdges: false)
        {
        }

        public IEnumerable<TVertex> GetParents(TVertex vertex)
        {
            return this.GetOutNeighbours(vertex);
        }

        public IEnumerable<TVertex> GetChildren(TVertex vertex)
        {
            return this.GetInNeighbours(vertex);
        }

        public IEnumerable<TVertex> GetSiblings(TVertex vertex)
        {
            var parentVertices = GetParents(vertex).ToList();
            return Vertices.Where(i => !vertex.Equals(i) && parentVertices.Intersect(GetParents(i)).Any());
        }

        public void ExecuteOnDescendantVertices(TVertex rootVertex, Action<TVertex> actionOnVertex)
        {
            actionOnVertex(rootVertex);

            foreach (var child in GetChildren(rootVertex))
                ExecuteOnDescendantVertices(child, actionOnVertex);
        }

        /// <summary>
        /// Returns a value that is one higher than the rank of all parent vertices, or zero if there's no parent vertex.
        /// </summary>
        /// <param name="vertex">A vertex.</param>
        /// <param name="rankFunc">A function that returns the rank of a vertex.</param>
        /// <returns>The rank of the given vertex.</returns>
        public int GetRank(TVertex vertex, Func<TVertex, int> rankFunc)
        {
            return GetParents(vertex).Select(rankFunc).DefaultIfEmpty(-1).Max() + 1;
        }
    }
}
