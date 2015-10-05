using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.Incremental
{
    internal static class LayoutVertexExtensions
    {
        public static double GetInsertionPointX(this IEnumerable<LayoutVertex> siblings, LayoutVertex newVertex, 
            LayoutVertex parentVertex, double horizontalGap)
        {
            var result = parentVertex.Center.X;

            var orderedSiblings = siblings.OrderBy(i => i.Center.X).ToArray();
            if (orderedSiblings.Any())
            {
                result = orderedSiblings[0].Left - horizontalGap/2;

                var index = 0;
                while (index < orderedSiblings.Length && orderedSiblings[index].CompareTo(newVertex) < 0)
                {
                    result = orderedSiblings[index].Right + horizontalGap/2;
                    index++;
                }
            }

            return result;
        }
    }
}
