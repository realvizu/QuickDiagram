namespace Codartis.SoftVis.Geometry
{
    /// <summary>
    /// Argument for an event that discribes the movement of a Rect from on place to another.
    /// </summary>
    public class RectMove
    {
        public Rect2D Rect { get; }
        public Point2D FromCenter { get; }
        public Point2D ToCenter { get; }

        public RectMove(Rect2D rect, Point2D fromCenter, Point2D toCenter)
        {
            Rect = rect;
            FromCenter = fromCenter;
            ToCenter = toCenter;
        }
    }
}
