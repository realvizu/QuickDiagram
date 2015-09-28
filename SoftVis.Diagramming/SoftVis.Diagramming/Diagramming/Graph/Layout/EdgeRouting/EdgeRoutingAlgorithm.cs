using System;
using System.Collections.Generic;
using System.Linq;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.EdgeRouting
{
    internal class EdgeRoutingAlgorithm<TVertex, TEdge> : IEdgeRoutingAlgorithm<TVertex, TEdge>
        where TVertex: IPositionedExtent
        where TEdge: IEdge<TVertex>
    {
        private readonly IEnumerable<TEdge> _originalEdges;
        private readonly EdgeRoutingType _edgeRoutingType;
        private readonly IDictionary<TEdge, DiagramPoint[]> _interimRoutePointsOfEdges;

        public IDictionary<TEdge, DiagramPoint[]> EdgeRoutes { get; private set; }

        public EdgeRoutingAlgorithm(IEnumerable<TEdge> originalEdges, EdgeRoutingType edgeRoutingType, 
            IDictionary<TEdge, DiagramPoint[]> interimRoutePointsOfEdges)
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

        private Dictionary<TEdge, DiagramPoint[]> CalculateStraightEdgeRouting()
        {
            var edgeRoutes = new Dictionary<TEdge, DiagramPoint[]>();

            foreach (var edge in _originalEdges)
            {
                var source = edge.Source;
                var target = edge.Target;

                DiagramPoint[] interimRoutePoints;
                _interimRoutePointsOfEdges.TryGetValue(edge, out interimRoutePoints);

                var secondPoint = interimRoutePoints?.FirstOrDefault() ?? target.Center;
                var firstPoint = source.Rect.GetAttachPointToward(secondPoint);

                var penultimatePoint = interimRoutePoints?.LastOrDefault() ?? source.Center;
                var lastPoint = target.Rect.GetAttachPointToward(penultimatePoint);

                edgeRoutes.Add(edge, DiagramPoint.CreateRoute(firstPoint, interimRoutePoints, lastPoint));
            }

            return edgeRoutes;
        }

        private Dictionary<TEdge, DiagramPoint[]> CalculateOrthogonalEdgeRouting()
        {
            var edgeRoutes = new Dictionary<TEdge, DiagramPoint[]>();

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

            //    var secondPoint = new DiagramPoint(sourceVertex.HorizontalPosition, sourceVertical);
            //    var firstPoint = NewRect(edge.Source).GetAttachPointToward(secondPoint);

            //    var targetVertical = isUpsideDown
            //        ? targetLayer.Position + targetLayer.Height + layerDistance / 2.0
            //        : targetLayer.Position - layerDistance / 2.0;

            //    var penultimatePoint = new DiagramPoint(targetVertex.HorizontalPosition, targetVertical);
            //    var lastPoint = NewRect(edge.Target).GetAttachPointToward(penultimatePoint);

            //    var dummyVertexPoints = _edgeToDummyVerticesMap[edge]?.Select(i => new DiagramPoint(i.HorizontalPosition, i.VerticalPosition)).ToList();

            //    DiagramPoint? thirdPoint = null;
            //    DiagramPoint? beforePenultimatePoint = null;
            //    if (dummyVertexPoints != null)
            //    {
            //        thirdPoint = new DiagramPoint(dummyVertexPoints.First().X, secondPoint.Y);
            //        beforePenultimatePoint = new DiagramPoint(dummyVertexPoints.Last().X, penultimatePoint.Y);
            //    }

            //    var route = DiagramPoint.CreateRoute(firstPoint, secondPoint, thirdPoint, dummyVertexPoints,
            //        beforePenultimatePoint, penultimatePoint, lastPoint);
            //    route = RemoveConsecutiveSamePoints(route);

            //    edgeRoutes.Add(edge, route);
            //}

            return edgeRoutes;
        }

        private static DiagramPoint[] RemoveConsecutiveSamePoints(DiagramPoint[] route)
        {
            var resultPoints = new List<DiagramPoint>();

            var previousPoint = DiagramPoint.Extreme;
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