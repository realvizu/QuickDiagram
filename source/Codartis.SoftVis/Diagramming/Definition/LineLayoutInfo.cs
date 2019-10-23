using Codartis.SoftVis.Geometry;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition
{
    public struct LineLayoutInfo : ILayoutInfo
    {
        [NotNull] public string ShapeId { get; }
        public Route Route { get; }

        public LineLayoutInfo([NotNull] string shapeId, Route route)
        {
            ShapeId = shapeId;
            Route = route;
        }

        public Rect2D Rect => Route.ToRect();

        public LineLayoutInfo Translate(Point2D vector) => new LineLayoutInfo(ShapeId, Route.Translate(vector));
    }
}