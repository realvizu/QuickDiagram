namespace Codartis.SoftVis.Geometry
{
    /// <summary>
    /// Describes a point relative to a Rect, given by a reference point and a translate vector.
    /// </summary>
    public class RectRelativeLocation
    {
        public RectReferencePoint ReferencePoint { get; }
        public Point2D Translate { get; }

        public RectRelativeLocation(RectReferencePoint referencePoint, Point2D translate)
        {
            ReferencePoint = referencePoint;
            Translate = translate;
        }

        public RectRelativeLocation WithTranslate(Point2D translate)
        {
            return new RectRelativeLocation(ReferencePoint, translate);
        }
    }
}
