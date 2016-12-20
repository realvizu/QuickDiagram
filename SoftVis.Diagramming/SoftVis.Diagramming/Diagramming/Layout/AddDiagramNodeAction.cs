namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// A diagram action that desribes the addition of a diagram node.
    /// </summary>
    internal sealed class AddDiagramNodeAction : DiagramNodeAction
    {
        public AddDiagramNodeAction(IDiagramNode diagramNode)
            : base(diagramNode, ShapeActionType.Add)
        {
        }

        public override void Accept(IDiagramActionVisitor visitor) => visitor.Visit(this);
    }
}
