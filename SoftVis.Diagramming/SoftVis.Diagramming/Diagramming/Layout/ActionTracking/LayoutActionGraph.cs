using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Layout.ActionTracking
{
    /// <summary>
    /// A graph of layout actions connected by causality relationships.
    /// </summary>
    internal class LayoutActionGraph : BidirectionalGraph<ILayoutAction, LayoutActionEdge>
    {
        public LayoutActionGraph() 
            : base(allowParallelEdges: false)
        {
        }
    }
}
