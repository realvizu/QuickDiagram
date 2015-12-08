using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.Incremental.Absolute
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

        public DiagramConnector DiagramConnector => Path.DiagramConnector;
        public DiagramShape DiagramShape => DiagramConnector;

        public void AcceptVisitor(ILayoutActionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}