using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming.Graph.Layout
{
    internal interface IVertexPositioningAlgorithm<TVertex> : IAlgorithm
    {
        IDictionary<TVertex, DiagramPoint> VertexCenters { get; } 
    }
}
