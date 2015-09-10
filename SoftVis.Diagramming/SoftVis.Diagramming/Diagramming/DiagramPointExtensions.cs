using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Diagramming
{
    public static class DiagramPointExtensions
    {
        public static string ToOneString(this IEnumerable<DiagramPoint> points)
        {
            return points.Aggregate("", (s, p) => s += p.ToString() + " | ");
        }
    }
}
