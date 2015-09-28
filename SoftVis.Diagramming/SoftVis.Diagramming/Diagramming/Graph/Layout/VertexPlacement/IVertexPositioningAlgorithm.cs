using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.VertexPlacement
{
    internal interface IVertexPositioningAlgorithm<TVertex> : IAlgorithm
    {
        IDictionary<TVertex, DiagramPoint> VertexCenters { get; } 
    }
}
