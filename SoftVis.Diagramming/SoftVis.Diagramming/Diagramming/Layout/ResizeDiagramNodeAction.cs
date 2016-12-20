using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// A diagram action that desribes the resizing of a diagram node.
    /// </summary>
    internal sealed class ResizeDiagramNodeAction : DiagramNodeAction
    {
        public Size2D NewSize { get; }

        public ResizeDiagramNodeAction(IDiagramNode diagramNode, Size2D newSize)
            : base(diagramNode, ShapeActionType.Resize)
        {
            NewSize = newSize;
        }

        public override void Accept(IDiagramActionVisitor visitor) => visitor.Visit(this);

        public override string ToString() => $"DiagramNodeAction({DiagramNode}, {ActionType}) [NewSize={NewSize}]";
    }
}
