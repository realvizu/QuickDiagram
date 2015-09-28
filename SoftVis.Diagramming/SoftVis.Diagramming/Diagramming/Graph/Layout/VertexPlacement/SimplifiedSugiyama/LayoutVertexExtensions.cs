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
                layoutVertexList.Select(i => i.Center.X - i.Width/2).Min(),
                layoutVertexList.Select(i => i.Center.X + i.Width/2).Max()
                );
        }
    }
}
