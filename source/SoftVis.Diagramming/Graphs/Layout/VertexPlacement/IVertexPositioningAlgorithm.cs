using System.Collections.Generic;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement
{
    internal interface IVertexPositioningAlgorithm<TVertex> : IAlgorithm
    {
        IDictionary<TVertex, Point2D> VertexCenters { get; } 
    }
}
