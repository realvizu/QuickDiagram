using System;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Layout;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    internal sealed class DiagramBackwardsMutatorLayoutActionVisitor : LayoutActionVisitorBase
    {
        private readonly Diagram _diagram;

        public DiagramBackwardsMutatorLayoutActionVisitor(Diagram diagram)
        {
            _diagram = diagram;
        }

        public override void Visit(IMoveDiagramNodeLayoutAction layoutAction)
        {
            var diagramNode = _diagram.Nodes.FirstOrDefault(i => i == layoutAction.DiagramNode);
            if (diagramNode == null)
                throw new InvalidOperationException($"Applying layout action, but DiagramNode {layoutAction.DiagramNode} not found.");

            diagramNode.Center = layoutAction.From;
        }

        public override void Visit(IRerouteDiagramConnectorLayoutAction layoutAction)
        {
            var diagramConnector = _diagram.Connectors.FirstOrDefault(i => i == layoutAction.DiagramConnector);
            if (diagramConnector == null)
                throw new InvalidOperationException($"Applying layout action, but DiagramConnector {layoutAction.DiagramConnector} not found.");

            diagramConnector.RoutePoints = layoutAction.OldRoute;
        }

    }
}
