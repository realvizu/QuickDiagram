using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// A layout action that moves a diagram node.
    /// </summary>
    public interface IMoveDiagramNodeLayoutAction : IDiagramNodeLayoutAction
    {
        Point2D From { get; }
        Point2D To { get; }
        Point2D By { get; }
    }
}