using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Diagramming
{
    public static class DiagramRectExtensions
    {
        public static DiagramRect Union(this IEnumerable<DiagramRect> diagramRects)
        {
            return new DiagramRect(
                diagramRects.Select(i => i.Left).Min(),
                diagramRects.Select(i => i.Top).Min(),
                diagramRects.Select(i => i.Right).Max(),
                diagramRects.Select(i => i.Bottom).Max()
                );
        }
    }
}
