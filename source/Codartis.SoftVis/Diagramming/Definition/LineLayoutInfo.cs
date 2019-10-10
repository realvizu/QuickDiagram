using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Definition
{
    public struct LineLayoutInfo : ILayoutInfo
    {
        public IDiagramConnector Connector { get; }
        public Route Route { get; }
        public Rect2D Rect { get; }

        public LineLayoutInfo(IDiagramConnector connector, Route route)
        {
            Connector = connector;
            Route = route;
            Rect = route.ToRect();
        }
    }
}
