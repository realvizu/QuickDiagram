namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// Applies layout actions to a diagram store (move node, reroute connector).
    /// </summary>
    internal sealed class LayoutActionExecutorVisitor : ILayoutActionVisitor
    {
        private readonly IDiagramService _diagramService;

        public LayoutActionExecutorVisitor(IDiagramService diagramService)
        {
            _diagramService = diagramService;
        }

        public void Visit(IMoveDiagramNodeLayoutAction layoutAction)
        {
            _diagramService.UpdateDiagramNodeCenter(layoutAction.DiagramNode.Id, layoutAction.To);
        }

        public void Visit(IRerouteDiagramConnectorLayoutAction layoutAction)
        {
            _diagramService.UpdateConnectorRoute(layoutAction.DiagramConnector.Id, layoutAction.NewRoute);
        }
    }
}
