using System;
using System.Collections.Generic;
using System.Linq;
using QuickGraph;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.Incremental
{
    /// <summary>
    /// A graph used for calculating layout (vertex positions and edge routes).
    /// </summary>
    /// <remarks>
    /// Differences to the original graph:
    /// <para>Dummy vertices can be added to break long edges and ensure that neighbours are always on adjacent layers.</para>
    /// <para>Edges can be reversed to ensure an acyclic graph.</para>
    /// <para>When rearranging vertices the "floating" ones are removed from this graph so they don't cause overlaps.</para>
    /// </remarks>
    internal sealed class LayoutGraph : BidirectionalGraph<LayoutVertex, LayoutEdge>
    {
        public IEnumerable<LayoutEdge> GetAllEdges(LayoutVertex layoutVertex)
        {
            return InEdges(layoutVertex).Union(OutEdges(layoutVertex));
        }

        public IEnumerable<LayoutVertex> GetNonFloatingNeighbours(LayoutVertex layoutVertex, EdgeDirection edgeDirection)
        {
            return this.GetNeighbours(layoutVertex, edgeDirection).Where(i => !i.IsFloating);
        }

        public void ExecuteOnTree(LayoutVertex rootVertex, EdgeDirection edgeDirection, Action<LayoutVertex> actionOnVertex)
        {
            actionOnVertex(rootVertex);
            foreach (var layoutEdge in this.GetEdges(rootVertex, edgeDirection))
            {
                var nextVertex = GetOtherEndOfEdge(layoutEdge, edgeDirection);
                ExecuteOnTree(nextVertex, edgeDirection, actionOnVertex);
            }
        }

        public void ExecuteOnTree(LayoutVertex rootVertex, LayoutVertex parentVertex, EdgeDirection edgeDirection, 
            Action<LayoutVertex, LayoutVertex> actionOnVertexAndParent)
        {
            actionOnVertexAndParent(rootVertex, parentVertex);
            foreach (var layoutEdge in this.GetEdges(rootVertex, edgeDirection))
            {
                var nextVertex = GetOtherEndOfEdge(layoutEdge, edgeDirection);
                ExecuteOnTree(nextVertex, rootVertex, edgeDirection, actionOnVertexAndParent);
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
