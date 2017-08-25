using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Layout;

namespace Codartis.SoftVis.Service.Plugins
{
    /// <summary>
    /// Applies layout actions to a diagram store (move node, reroute connector).
    /// </summary>
    internal sealed class LayoutActionExecutorVisitor : ILayoutActionVisitor
    {
        private readonly IDiagramStore _diagramStore;

        public LayoutActionExecutorVisitor(IDiagramStore diagramStore)
        {
            _diagramStore = diagramStore;
        }

        public void Visit(IMoveDiagramNodeLayoutAction layoutAction)
        {
            _diagramStore.UpdateDiagramNodeCenter(layoutAction.DiagramNode, layoutAction.To);
        }

        public void Visit(IRerouteDiagramConnectorLayoutAction layoutAction)
        {
            _diagramStore.UpdateDiagramConnectorRoute(layoutAction.DiagramConnector, layoutAction.NewRoute);
        }
    }
}
