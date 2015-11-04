using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.ActionTracking
{
    /// <summary>
    /// A layout action that moves a LayoutVertexBase.
    /// </summary>
    internal interface IMoveVertexAction
    {
        LayoutVertexBase Vertex { get; }
        Point2D From { get; }
        Point2D To { get; }
        Point2D By { get; }
    }
}
