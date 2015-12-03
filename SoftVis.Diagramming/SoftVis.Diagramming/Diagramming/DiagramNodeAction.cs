namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A diagram action that desribes the addition or removal of a diagram node.
    /// </summary>
    internal sealed class DiagramNodeAction : DiagramShapeAction
    {
        public DiagramNode DiagramNode { get; }

        public DiagramNodeAction(DiagramNode diagramNode, ShapeActionType actionType)
            : base(actionType)
        {
            DiagramNode = diagramNode;
        }
    }
}
