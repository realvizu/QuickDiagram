using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.ActionTracking
{
    /// <summary>
    /// An action of a layout logic run that moves a vertex.
    /// </summary>
    public interface IVertexMoveAction : IVertexAction
    {
        Point2D From { get; }
        Point2D To { get; }
        Point2D By { get; }
    }
}