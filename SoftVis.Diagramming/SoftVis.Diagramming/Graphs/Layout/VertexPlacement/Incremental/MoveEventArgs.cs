using System;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.Incremental
{
    internal class MoveEventArgs : EventArgs
    {
        public Point2D From { get; }
        public Point2D To { get; }

        public MoveEventArgs(Point2D @from, Point2D to)
        {
            From = @from;
            To = to;
        }
    }
}
