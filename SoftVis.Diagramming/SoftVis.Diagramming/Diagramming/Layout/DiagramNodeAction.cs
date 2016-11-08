namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// A diagram action that desribes the addition or removal of a diagram node.
    /// </summary>
    internal sealed class DiagramNodeAction : DiagramShapeAction
    {
        public IDiagramNode DiagramNode { get; }

        public DiagramNodeAction(IDiagramNode diagramNode, ShapeActionType actionType)
            : base(actionType)
        {
            DiagramNode = diagramNode;
        }

        public override string ToString() => $"DiagramNodeAction({DiagramNode}, {ActionType})";
    }
}
