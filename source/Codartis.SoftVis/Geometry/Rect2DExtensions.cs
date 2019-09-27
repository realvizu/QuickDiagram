using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Geometry
{
    public static class Rect2DExtensions
    {
        public static bool IsUndefined(this Rect2D rect)
        {
            return double.IsNaN(rect.Left) || double.IsNaN(rect.Top) || double.IsNaN(rect.Width) || double.IsNaN(rect.Height);
        }

        public static bool IsDefined(this Rect2D rect) => !rect.IsUndefined();

        public static Rect2D ToRect(this Route route) => Union(Rect2D.Undefined, route);

        public static Rect2D Union(this Rect2D rect1, Rect2D rect2)
        {
            return Rect2D.Union(rect1, rect2);
        }

        public static Rect2D Union(this Rect2D rect, Point2D point)
        {
            return Union(rect, new Rect2D(point, point));
        }

        public static Rect2D Union(this Rect2D rect, Route route)
        {
            var result = rect;

            foreach (var point in route)
                result = result.Union(point);

            return result;
        }

        public static Rect2D Union([NotNull] this IEnumerable<Rect2D> rects)
        {
            var definedRects = rects.Where(i => i.IsDefined()).ToList();

            return definedRects.Any()
                ? new Rect2D(
                    definedRects.Select(i => i.Left).Min(),
                    definedRects.Select(i => i.Top).Min(),
                    definedRects.Select(i => i.Right).Max(),
                    definedRects.Select(i => i.Bottom).Max()
                )
                : Rect2D.Undefined;
        }

        public static Rect2D Intersect(this Rect2D rect1, Rect2D rect2)
        {
            return Rect2D.Intersect(rect1, rect2);
        }
    }
}