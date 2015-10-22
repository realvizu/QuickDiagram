using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Layout.ActionTracking
{
    /// <summary>
    /// Represents the causality relationship of two actions in a layout logic run.
    /// </summary>
    public interface ILayoutActionEdge : IEdge<ILayoutAction>
    {
    }
}