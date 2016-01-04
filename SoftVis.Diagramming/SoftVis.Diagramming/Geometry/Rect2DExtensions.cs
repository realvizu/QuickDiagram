using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Geometry
{
    public static class Rect2DExtensions
    {
        public static Rect2D Union(this Rect2D rect1, Rect2D rect2)
        {
            return Rect2D.Union(rect1, rect2);
        }

        public static Rect2D Union(this Rect2D rect, Point2D point)
        {
            return Union(rect, new Rect2D(point, point));
        }

        public static Rect2D Union(this IEnumerable<Rect2D> rects)
        {
            var enumerable = rects as IList<Rect2D> ?? rects.ToList();

            return enumerable.Any()
                ? new Rect2D(
                    enumerable.Select(i => i.Left).Min(),
                    enumerable.Select(i => i.Top).Min(),
                    enumerable.Select(i => i.Right).Max(),
                    enumerable.Select(i => i.Bottom).Max()
                    )
                : Rect2D.Empty;
        }

        public static Rect2D Intersect(this Rect2D rect1, Rect2D rect2)
        {
            return Rect2D.Intersect(rect1, rect2);
        }
    }
}
