using Codartis.SoftVis.Diagramming.Layout.Nodes.Layered.Sugiyama;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// A layout action that reroutes a LayoutPath.
    /// </summary>
    internal class ReroutePathLayoutAction : IRerouteDiagramConnectorLayoutAction
    {
        private LayoutPath Path { get; }
        public Route OldRoute { get; }
        public Route NewRoute { get; }

        public ReroutePathLayoutAction(LayoutPath path, Route oldRoute, Route newRoute)

        {
            Path = path;
            OldRoute = oldRoute;
            NewRoute = newRoute;
        }

        public IDiagramConnector DiagramConnector => Path.DiagramConnector;
        public IDiagramShape DiagramShape => DiagramConnector;

        public void AcceptVisitor(ILayoutActionVisitor visitor) => visitor.Visit(this);

        public override string ToString() => $"ReroutePathLayoutAction {Path}: {OldRoute}->{NewRoute}";
    }
}