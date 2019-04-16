namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// A diagram action that desribes the removal of a diagram connector.
    /// </summary>
    internal sealed class RemoveDiagramConnectorAction : DiagramConnectorAction
    {
        public RemoveDiagramConnectorAction(IDiagramConnector diagramConnector)
            : base(diagramConnector, ShapeActionType.Remove)
        {
        }

        public override void Accept(IDiagramActionVisitor visitor) => visitor.Visit(this);
    }
}
