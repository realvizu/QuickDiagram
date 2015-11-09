using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.ActionTracking
{
    internal interface  IRelativeVertexLayoutAction : IRelativeLayoutAction
    {
        LayoutVertexBase Vertex { get; }
    }
}
