using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.UI.Geometry
{
    /// <summary>
    /// Describes a point relative to a Rect, given by the alignment and a translate vector.
    /// </summary>
    public class RectRelativeLocation
    {
        public RectAlignment Alignment { get; }
        public Point2D Translate { get; }

        public RectRelativeLocation(RectAlignment alignment, Point2D translate)
        {
            Alignment = alignment;
            Translate = translate;
        }

        public RectRelativeLocation WithTranslate(Point2D translate)
        {
            return new RectRelativeLocation(Alignment, translate);
        }
    }
}
