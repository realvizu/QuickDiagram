using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Geometry;
using QuickGraph;

namespace Codartis.SoftVis.Graphs.Layout.EdgeRouting
{
    internal class EdgeRoutingAlgorithm<TVertex, TEdge> : IEdgeRoutingAlgorithm<TVertex, TEdge>
        where TVertex: IRect
        where TEdge: IEdge<TVertex>
    {
        private readonly IEnumerable<TEdge> _originalEdges;
        private readonly EdgeRoutingType _edgeRoutingType;
        private readonly IDictionary<TEdge, Point2D[]> _interimRoutePointsOfEdges;

        public IDictionary<TEdge, Point2D[]> EdgeRoutes { get; private set; }

        public EdgeRoutingAlgorithm(IEnumerable<TEdge> originalEdges, EdgeRoutingType edgeRoutingType, 
            IDictionary<TEdge, Point2D[]> interimRoutePointsOfEdges)
        {
            _originalEdges = originalEdges;
            _interimRoutePointsOfEdges = interimRoutePointsOfEdges;
            _edgeRoutingType= edgeRoutingType;
        }

        public void Compute()
        {
            switch (_edgeRoutingType)
            {
                case EdgeRoutingType.Straight:
                    EdgeRoutes = CalculateStraightEdgeRouting();
                    break;
                case EdgeRoutingType.Orthogonal:
                    EdgeRoutes = CalculateOrthogonalEdgeRouting();
                    break;
                default:
                    throw new Exception($"Unexpected EdgeRoutingType: {_edgeRoutingType}");
            }
        }

        private Dictionary<TEdge, Point2D[]> CalculateStraightEdgeRouting()
        {
            var edgeRoutes = new Dictionary<TEdge, Point2D[]>();

            foreach (var edge in _originalEdges)
            {
                var source = edge.Source;
                var target = edge.Target;

                Point2D[] interimRoutePoints;
                _interimRoutePointsOfEdges.TryGetValue(edge, out interimRoutePoints);

                var secondPoint = interimRoutePoints?.FirstOrDefault() ?? target.Center;
                var firstPoint = source.Rect.GetAttachPointToward(secondPoint);

                var penultimatePoint = interimRoutePoints?.LastOrDefault() ?? source.Center;
                var lastPoint = target.Rect.GetAttachPointToward(penultimatePoint);

                edgeRoutes.Add(edge, Point2D.CreateRoute(firstPoint, interimRoutePoints, lastPoint));
            }

            return edgeRoutes;
        }

        private Dictionary<TEdge, Point2D[]> CalculateOrthogonalEdgeRouting()
        {
            var edgeRoutes = new Dictionary<TEdge, Point2D[]>();

            //var layerDistance = _layoutParameters.LayerDistance;

            //foreach (var edge in _originalEdges)
            //{
            //    var sourceVertex = _sugiGraph.GetSugiVertexByOriginal(edge.Source);
            //    var targetVertex = _sugiGraph.GetSugiVertexByOriginal(edge.Target);

            //    var sourceLayer = _layers[sourceVertex];
            //    var targetLayer = _layers[targetVertex];

            //    var isUpsideDown = sourceVertex.LayerIndex > targetVertex.LayerIndex;

            //    var sourceVertical = isUpsideDown
            //        ? sourceLayer.Position - layerDistance / 2.0
            //        : sourceLayer.Position + sourceLayer.Height + layerDistance / 2.0;

            //    var secondPoint = new Point2D(sourceVertex.HorizontalPosition, sourceVertical);
            //    var firstPoint = NewRect(edge.Source).GetAttachPointToward(secondPoint);

            //    var targetVertical = isUpsideDown
            //        ? targetLayer.Position + targetLayer.Height + layerDistance / 2.0
            //        : targetLayer.Position - layerDistance / 2.0;

            //    var penultimatePoint = new Point2D(targetVertex.HorizontalPosition, targetVertical);
            //    var lastPoint = NewRect(edge.Target).GetAttachPointToward(penultimatePoint);

            //    var dummyVertexPoints = _edgeToDummyVerticesMap[edge]?.Select(i => new Point2D(i.HorizontalPosition, i.VerticalPosition)).ToList();

            //    Point2D? thirdPoint = null;
            //    Point2D? beforePenultimatePoint = null;
            //    if (dummyVertexPoints != null)
            //    {
            //        thirdPoint = new Point2D(dummyVertexPoints.First().X, secondPoint.Y);
            //        beforePenultimatePoint = new Point2D(dummyVertexPoints.Last().X, penultimatePoint.Y);
            //    }

            //    var route = Point2D.CreateRoute(firstPoint, secondPoint, thirdPoint, dummyVertexPoints,
            //        beforePenultimatePoint, penultimatePoint, lastPoint);
            //    route = RemoveConsecutiveSamePoints(route);

            //    edgeRoutes.Add(edge, route);
            //}

            return edgeRoutes;
        }

        private static Point2D[] RemoveConsecutiveSamePoints(Point2D[] route)
        {
            var resultPoints = new List<Point2D>();

            var previousPoint = Point2D.Extreme;
            foreach (var point in route)
            {
                if (point != previousPoint)
                    resultPoints.Add(point);
                
                previousPoint = point;
            }

            return resultPoints.ToArray();
        }
    }
}