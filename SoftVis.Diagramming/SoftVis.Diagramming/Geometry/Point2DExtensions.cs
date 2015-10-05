using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Geometry
{
    public static class Point2DExtensions
    {
        public static string ToOneString(this IEnumerable<Point2D> points)
        {
            return points.Aggregate("", (s, p) => s += p.ToString() + " | ");
        }
    }
}
