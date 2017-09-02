using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Layout;

namespace Codartis.SoftVis.Service.Plugins
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
            _diagramService.UpdateDiagramNodeCenter(layoutAction.DiagramNode, layoutAction.To);
        }

        public void Visit(IRerouteDiagramConnectorLayoutAction layoutAction)
        {
            _diagramService.UpdateDiagramConnectorRoute(layoutAction.DiagramConnector, layoutAction.NewRoute);
        }
    }
}
