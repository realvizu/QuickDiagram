using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Geometry
{
    internal static class IRectExtensions
    {
        public static Rect2D GetRect(this IEnumerable<IRect> vertices)
        {
            return vertices.Select(i => i.Rect).Union();
        }
    }
}
