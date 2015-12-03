using System;
using System.Linq;
using Codartis.SoftVis.Diagramming.Layout;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// Applies layout actions to a diagram (move node, reroute connector).
    /// </summary>
    internal sealed class LayoutActionExecutorVisitor : ILayoutActionVisitor
    {
        private readonly IArrangeableDiagram _diagram;

        public LayoutActionExecutorVisitor(IArrangeableDiagram diagram)
        {
            _diagram = diagram;
        }

        public void Visit(IMoveDiagramNodeLayoutAction layoutAction)
        {
            var diagramNode = _diagram.Nodes.FirstOrDefault(i => i == layoutAction.DiagramNode);
            if (diagramNode == null)
                throw new InvalidOperationException($"Applying layout action, but DiagramNode {layoutAction.DiagramNode} not found.");

            _diagram.MoveNode(diagramNode, layoutAction.To);
        }

        public void Visit(IRerouteDiagramConnectorLayoutAction layoutAction)
        {
            var diagramConnector = _diagram.Connectors.FirstOrDefault(i => i == layoutAction.DiagramConnector);
            if (diagramConnector == null)
                throw new InvalidOperationException($"Applying layout action, but DiagramConnector {layoutAction.DiagramConnector} not found.");

            _diagram.RerouteConnector(diagramConnector, layoutAction.NewRoute);
        }
    }
}
