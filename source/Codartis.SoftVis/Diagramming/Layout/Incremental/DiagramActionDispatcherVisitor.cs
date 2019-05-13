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

        public void Visit(AddDiagramNodeAction action) => _consumer.AddDiagramNode(action.DiagramNode);
        public void Visit(RemoveDiagramNodeAction action) => _consumer.RemoveDiagramNode(action.DiagramNode);
        public void Visit(ResizeDiagramNodeAction action) => _consumer.ResizeDiagramNode(action.DiagramNode, action.NewSize);
        public void Visit(AddDiagramConnectorAction action) => _consumer.AddDiagramConnector(action.DiagramConnector);
        public void Visit(RemoveDiagramConnectorAction action) => _consumer.RemoveDiagramConnector(action.DiagramConnector);
        public void Visit(ClearDiagramAction action) => _consumer.ClearDiagram();
    }
}
