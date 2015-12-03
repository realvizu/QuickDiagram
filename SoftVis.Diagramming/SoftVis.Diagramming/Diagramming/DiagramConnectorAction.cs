namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A diagram action that desribes the addition or removal of a diagram connector.
    /// </summary>
    internal sealed class DiagramConnectorAction : DiagramShapeAction
    {
        public DiagramConnector DiagramConnector { get; }

        public DiagramConnectorAction(DiagramConnector diagramConnector, ShapeActionType actionType)
            : base(actionType)
        {
            DiagramConnector = diagramConnector;
        }
    }
}
