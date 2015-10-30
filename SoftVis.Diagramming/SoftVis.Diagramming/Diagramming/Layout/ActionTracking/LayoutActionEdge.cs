using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Layout.ActionTracking
{
    /// <summary>
    /// Represents the causality relationship between two layout actions.
    /// </summary>
    internal class LayoutActionEdge : Edge<ILayoutAction>
    {
        public LayoutActionEdge(ILayoutAction source, ILayoutAction target) 
            : base(source, target)
        {
        }
    }
}
