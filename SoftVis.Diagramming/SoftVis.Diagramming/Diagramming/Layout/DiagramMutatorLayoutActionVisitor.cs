using System;
using System.Linq;

namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// Applies layout actions to a diagram (move node, reroute connector).
    /// </summary>
    public sealed class DiagramMutatorLayoutActionVisitor : LayoutActionVisitorBase
    {
        private readonly Diagram _diagram;

        public DiagramMutatorLayoutActionVisitor(Diagram diagram)
        {
            _diagram = diagram;
        }

        public override void Visit(IMoveDiagramNodeLayoutAction layoutAction)
        {
            var diagramNode = _diagram.Nodes.FirstOrDefault(i => i == layoutAction.DiagramNode);
            if (diagramNode == null)
                throw new InvalidOperationException($"Applying layout action, but DiagramNode {layoutAction.DiagramNode} not found.");

            diagramNode.Center = layoutAction.To;
        }

        public override void Visit(IRerouteDiagramConnectorLayoutAction layoutAction)
        {
            var diagramConnector = _diagram.Connectors.FirstOrDefault(i => i == layoutAction.DiagramConnector);
            if (diagramConnector == null)
                throw new InvalidOperationException($"Applying layout action, but DiagramConnector {layoutAction.DiagramConnector} not found.");

            diagramConnector.RoutePoints = layoutAction.NewRoute;
        }

    }
}
