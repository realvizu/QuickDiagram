using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Layout.ActionTracking.Implementation
{
    /// <summary>
    /// Represents the causality relationship of two actions in a layout logic run.
    /// </summary>
    internal class LayoutActionEdge : Edge<ILayoutAction>,  ILayoutActionEdge
    {
        public LayoutActionEdge(ILayoutAction source, ILayoutAction target) 
            : base(source, target)
        {
        }
    }
}
