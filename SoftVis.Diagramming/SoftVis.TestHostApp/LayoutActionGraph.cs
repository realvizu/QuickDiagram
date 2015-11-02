using Codartis.SoftVis.Diagramming.Layout.ActionTracking;
using QuickGraph;

namespace Codartis.SoftVis.TestHostApp
{
    /// <summary>
    /// Represents the actions of a layout logic run and the causality relationship between actions.
    /// </summary>
    internal class LayoutActionGraph : BidirectionalGraph<ILayoutAction, IEdge<ILayoutAction>>
    {
        public LayoutActionGraph()
            : base(false)
        {
        }
    }
}
