using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.VertexPlacement.SimplifiedSugiyama
{
    internal static class LayoutVertexExtensions
    {
        internal static Span GetHorizontalSpan(this IEnumerable<LayoutVertex> layoutVertices)
        {
            var layoutVertexList = layoutVertices as IList<LayoutVertex> ?? layoutVertices.ToList();

            return new Span(
                layoutVertexList.Select(i => i.Center.X - i.Width / 2).Min(),
                layoutVertexList.Select(i => i.Center.X + i.Width / 2).Max()
                );
        }

        internal static double GetWidth(this IEnumerable<LayoutVertex> layoutVertices, double horizontalGap)
        {
            var layoutVertexList = layoutVertices as IList<LayoutVertex> ?? layoutVertices.ToList();

            return layoutVertexList.Any()
                ? layoutVertexList.Sum(i => i.Width) + (layoutVertexList.Count - 1) * horizontalGap
                : 0;
        }
    }
}
