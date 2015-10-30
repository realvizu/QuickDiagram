using System;
using QuickGraph;

namespace Codartis.SoftVis.Graphs
{
    /// <summary>
    /// Publishes notification events about graph changes.
    /// </summary>
    /// <typeparam name="TVertex"></typeparam>
    /// <typeparam name="TEdge"></typeparam>
    public interface INotifyGraphChange<TVertex, TEdge>
        where TEdge: IEdge<TVertex>
    {
        event VertexAction<TVertex> VertexAdded;
        event VertexAction<TVertex> VertexRemoved;
        event EdgeAction<TVertex, TEdge> EdgeAdded;
        event EdgeAction<TVertex, TEdge> EdgeRemoved;
        event EventHandler Cleared;
    }
}
