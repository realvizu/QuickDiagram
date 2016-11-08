using Codartis.SoftVis.Diagramming.Layout;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// Applies layout actions to a diagram (move node, reroute connector).
    /// </summary>
    internal sealed class LayoutActionExecutorVisitor : ILayoutActionVisitor
    {
        private readonly IArrangedDiagram _diagram;

        public LayoutActionExecutorVisitor(IArrangedDiagram diagram)
        {
            _diagram = diagram;
        }

        public void Visit(IMoveDiagramNodeLayoutAction layoutAction)
        {
            _diagram.MoveNodeCenter(layoutAction.DiagramNode, layoutAction.To);
        }

        public void Visit(IRerouteDiagramConnectorLayoutAction layoutAction)
        {

            _diagram.RerouteConnector(layoutAction.DiagramConnector, layoutAction.NewRoute);
        }
    }
}
