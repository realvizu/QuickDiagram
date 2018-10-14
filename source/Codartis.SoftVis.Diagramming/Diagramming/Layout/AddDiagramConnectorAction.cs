namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// A diagram action that desribes the addition of a diagram connector.
    /// </summary>
    internal sealed class AddDiagramConnectorAction : DiagramConnectorAction
    {
        public AddDiagramConnectorAction(IDiagramConnector diagramConnector)
            : base(diagramConnector, ShapeActionType.Add)
        {
        }

        public override void Accept(IDiagramActionVisitor visitor) => visitor.Visit(this);
    }
}
