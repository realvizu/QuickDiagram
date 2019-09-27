using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Definition
{
    public struct ConnectorLayoutInfo : ILayoutInfo
    {
        public IDiagramConnector Connector { get; }
        public Route Route { get; }
        public Rect2D Rect { get; }

        public ConnectorLayoutInfo(IDiagramConnector connector, Route route)
        {
            Connector = connector;
            Route = route;
            Rect = route.ToRect();
        }
    }
}
