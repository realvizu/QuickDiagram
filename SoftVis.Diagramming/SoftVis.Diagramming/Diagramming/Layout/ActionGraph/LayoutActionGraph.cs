using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Layout.ActionGraph
{
    /// <summary>
    /// A graph of layout actions connected by causality relationships.
    /// </summary>
    public class LayoutActionGraph : BidirectionalGraph<ILayoutAction, LayoutActionEdge>
    {
        public LayoutActionGraph() 
            : base(allowParallelEdges: false)
        {
        }
    }
}
