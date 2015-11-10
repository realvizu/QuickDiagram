using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Layout.ActionGraph
{
    /// <summary>
    /// Represents the causality relationship between two layout actions.
    /// </summary>
    public class LayoutActionEdge : Edge<ILayoutAction>
    {
        public LayoutActionEdge(ILayoutAction source, ILayoutAction target) 
            : base(source, target)
        {
        }
    }
}
