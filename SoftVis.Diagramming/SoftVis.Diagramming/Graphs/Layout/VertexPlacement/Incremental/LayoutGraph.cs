using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using QuickGraph;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.Incremental
{
    internal sealed class LayoutGraph : BidirectionalGraph<LayoutVertex, LayoutEdge>
    {
        public IEnumerable<LayoutEdge> GetAllEdges(LayoutVertex layoutVertex)
        {
            return InEdges(layoutVertex).Union(OutEdges(layoutVertex));
        }

        public IEnumerable<LayoutVertex> GetSiblings(LayoutVertex childVertex, LayoutVertex parentVertex, EdgeDirection edgeDirection)
        {
            return this.GetEdges(parentVertex, edgeDirection)
                .Select(i => GetOtherEndOfEdge(i, edgeDirection))
                .Where(i => i != childVertex);
        }

        public void ExecuteOnTreeEdges(LayoutVertex rootVertex, EdgeDirection edgeDirection, Action<LayoutEdge> action)
        {
            foreach (var layoutEdge in this.GetEdges(rootVertex, edgeDirection))
            {
                action(layoutEdge);

                var nextVertex = GetOtherEndOfEdge(layoutEdge, edgeDirection);
                ExecuteOnTreeEdges(nextVertex, edgeDirection, action);
            }
        }

        private static LayoutVertex GetOtherEndOfEdge(LayoutEdge layoutEdge, EdgeDirection edgeDirection)
        {
            return edgeDirection == EdgeDirection.In
                ? layoutEdge.Source
                : layoutEdge.Target;
        }
    }
}
