using System.Collections.Generic;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs.Algorithms;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement
{
    internal interface IVertexPositioningAlgorithm<TVertex> : IAlgorithm
    {
        IDictionary<TVertex, Point2D> VertexCenters { get; } 
    }
}
