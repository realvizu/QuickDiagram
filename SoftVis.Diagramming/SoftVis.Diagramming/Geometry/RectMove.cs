namespace Codartis.SoftVis.Geometry
{
    /// <summary>
    /// Describes a vertex movement.
    /// </summary>
    public struct RectMove
    {
        public IRect Node { get; }
        public Point2D FromCenter { get; }
        public Point2D ToCenter { get; }

        public RectMove(IRect node, Point2D fromCenter, Point2D toCenter)
        {
            Node = node;
            FromCenter = fromCenter;
            ToCenter = toCenter;
        }
    }
}