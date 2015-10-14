using System;

namespace Codartis.SoftVis.Geometry
{
    /// <summary>
    /// Argument for an event that discribes the movement of a Rect from on place to another.
    /// </summary>
    public class RectMoveEventArgs : EventArgs
    {
        public IRect Rect { get; }
        public Point2D FromCenter { get; }
        public Point2D ToCenter { get; }

        public RectMoveEventArgs(IRect rect, Point2D fromCenter, Point2D toCenter)
        {
            Rect = rect;
            FromCenter = fromCenter;
            ToCenter = toCenter;
        }
    }
}
