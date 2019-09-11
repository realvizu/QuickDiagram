using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama.Relative.Logic
{
    /// <summary>
    /// Calculates the arrangement of layout vertices relative to each other.
    /// Stateless.
    /// </summary>
    /// <remarks>
    /// Arranges vertices into layers so that:
    /// <para>all edges point "upward" (to a layer with lower index)</para>
    /// <para>all edges span exactly 2 layers (by using dummy vertices as necessary)</para>
    /// <para>vertices in all layers ar ordered so that primary edges never cross.</para>
    /// </remarks>
    internal static class RelativeLayoutCalculator
    {
        public static IReadOnlyRelativeLayout Calculate(IEnumerable<DiagramNodeLayoutVertex> vertices, IEnumerable<LayoutPath> edges)
        {
            var relativeLayout = new RelativeLayout();

            foreach (var vertex in vertices)
                relativeLayout.AddVertex(vertex);

            foreach (var edge in edges)
                relativeLayout.AddEdge(edge);

            return relativeLayout;
        }
    }
}