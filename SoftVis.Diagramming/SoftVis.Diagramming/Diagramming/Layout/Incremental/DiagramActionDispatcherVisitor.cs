namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Dispatches diagram actions to operations of a diagram action consumer.
    /// </summary>
    internal class DiagramActionDispatcherVisitor : IDiagramActionVisitor
    {
        private readonly IDiagramActionConsumer _consumer;

        public DiagramActionDispatcherVisitor(IDiagramActionConsumer consumer)
        {
            _consumer = consumer;
        }

        public void Visit(DiagramNodeAction diagramNodeAction)
        {
            switch (diagramNodeAction.ActionType)
            {
                case ShapeActionType.Add:
                    _consumer.AddDiagramNode(diagramNodeAction.DiagramNode);
                    break;
                case ShapeActionType.Remove:
                    _consumer.RemoveDiagramNode(diagramNodeAction.DiagramNode);
                    break;
                case ShapeActionType.Resize:
                    _consumer.ResizeDiagramNode(diagramNodeAction.DiagramNode);
                    break;
            }
        }

        public void Visit(DiagramConnectorAction diagramConnectorAction)
        {
            switch (diagramConnectorAction.ActionType)
            {
                case ShapeActionType.Add:
                    _consumer.AddDiagramConnector(diagramConnectorAction.DiagramConnector);
                    break;
                case ShapeActionType.Remove:
                    _consumer.RemoveDiagramConnector(diagramConnectorAction.DiagramConnector);
                    break;
            }
        }

        public void Visit(DiagramBatchAction diagramBatchAction)
        {
        }
    }
}
