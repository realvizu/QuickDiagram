using System.Collections.Generic;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Graph.Layout
{
    internal interface IEdgeRoutingSupportAlgorithm<TVertex, TEdge> : IAlgorithm
        where TEdge: IEdge<TVertex>
    {
        IDictionary<TEdge, DiagramPoint[]> InterimRoutePointsOfEdges { get; }
    }
}
