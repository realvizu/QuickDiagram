using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Diagramming
{
    public static class DiagramRectExtensions
    {
        public static DiagramRect Union(this DiagramRect diagramRect1, DiagramRect diagramRect2)
        {
            return DiagramRect.Union(diagramRect1, diagramRect2);
        }

        public static DiagramRect Union(this IEnumerable<DiagramRect> diagramRects)
        {
            var enumerable = diagramRects as IList<DiagramRect> ?? diagramRects.ToList();

            return enumerable.Any()
                ? new DiagramRect(
                    enumerable.Select(i => i.Left).Min(),
                    enumerable.Select(i => i.Top).Min(),
                    enumerable.Select(i => i.Right).Max(),
                    enumerable.Select(i => i.Bottom).Max()
                    )
                : new DiagramRect(0, 0, 0, 0);
        }
    }
}
