namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// A diagram action that describes the removal of a diagram node.
    /// </summary>
    internal sealed class RemoveDiagramNodeAction : DiagramNodeAction
    {
        public RemoveDiagramNodeAction(IDiagramNode diagramNode)
            : base(diagramNode, ShapeActionType.Remove)
        {
        }

        public override void Accept(IDiagramActionVisitor visitor) => visitor.Visit(this);
    }
}
