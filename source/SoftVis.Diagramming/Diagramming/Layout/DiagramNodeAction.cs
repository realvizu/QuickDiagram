namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// A diagram action that desribes the addition or removal of a diagram node.
    /// </summary>
    internal abstract class DiagramNodeAction : DiagramShapeAction
    {
        public IDiagramNode DiagramNode { get; }

        protected DiagramNodeAction(IDiagramNode diagramNode, ShapeActionType actionType)
            : base(actionType)
        {
            DiagramNode = diagramNode;
        }

        public override string ToString() => $"DiagramNodeAction({DiagramNode}, {ActionType})";
    }
}
