using System.Collections.Generic;
using QuickGraph;

namespace Codartis.SoftVis.Graphs.Layered
{
    /// <summary>
    /// A read-only view of a graph that contains no directed cycles and its vertices are arranged into layers. 
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertices.</typeparam>
    /// <typeparam name="TEdge">The type of the edges.</typeparam>
    internal interface IReadOnlyLayeredGraph<TVertex, TEdge> : IBidirectionalGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        IEnumerable<TVertex> GetParents(TVertex vertex);
        IEnumerable<TVertex> GetChildren(TVertex vertex);
        IEnumerable<TVertex> GetSiblings(TVertex vertex);
        IEnumerable<TVertex> GetDescendants(TVertex vertex);
        IEnumerable<TVertex> GetVertexAndDescendants(TVertex vertex);

        bool HasChildren(TVertex vertex);

        int GetRank(TVertex vertex);
        int GetLayerIndex(TVertex vertex);
    }
}
