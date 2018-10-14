namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// A diagram action that desribes the addition or removal of a diagram connector.
    /// </summary>
    internal abstract class DiagramConnectorAction : DiagramShapeAction
    {
        public IDiagramConnector DiagramConnector { get; }

        protected DiagramConnectorAction(IDiagramConnector diagramConnector, ShapeActionType actionType)
            : base(actionType)
        {
            DiagramConnector = diagramConnector;
        }

        public override string ToString() => $"DiagramConnectorAction({DiagramConnector}, {ActionType})";
    }
}
