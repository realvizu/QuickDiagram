using System.Collections.Generic;
using System.Linq;
using QuickGraph;
using QuickGraph.Algorithms;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// The graph formed by the nodes and connectors of a diagram.
    /// </summary>
    internal sealed class DiagramGraph : BidirectionalGraph<DiagramNode, DiagramConnector>
    {
        public DiagramGraph() 
            : base(allowParallelEdges: false)
        {
        }

        public IEnumerable<DiagramPath> GetShortestPaths(DiagramNode source, DiagramNode target, int pathCount)
        {
            return this.RankedShortestPathHoffmanPavley(i => 1, source, target, pathCount).Select(i => new DiagramPath(i));
        }
    }
}
