using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Common;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.SimplifiedSugiyama
{
    internal static class LayoutVertexExtensions
    {
        internal static DoubleSpan GetHorizontalSpan(this IEnumerable<LayoutVertex> layoutVertices)
        {
            var layoutVertexList = layoutVertices as IList<LayoutVertex> ?? layoutVertices.ToList();

            return new DoubleSpan(
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
