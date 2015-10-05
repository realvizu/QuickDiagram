using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Geometry;
using QuickGraph;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.EfficientSugiyama
{
    internal class EdgeRoutingAlgorithm : IEdgeRoutingAlgorithm<ISized, IEdge<ISized>> 
    {
        private readonly IEnumerable<IEdge<ISized>> _originalEdges;
        private readonly SugiGraph _sugiGraph;
        private readonly EfficientSugiyamaLayoutParameters _layoutParameters;
        private readonly Layers _layers;
        private readonly EdgeToDummyVerticesMap _edgeToDummyVerticesMap;

        public IDictionary<IEdge<ISized>, Route> EdgeRoutes { get; private set; }

        public EdgeRoutingAlgorithm(IEnumerable<IEdge<ISized>> originalEdges, SugiGraph sugiGraph, 
            EfficientSugiyamaLayoutParameters layoutParameters, Layers layers, EdgeToDummyVerticesMap edgeToDummyVerticesMap)
        {
            _originalEdges = originalEdges;
            _sugiGraph = sugiGraph;
            _layoutParameters = layoutParameters;
            _layers = layers;
            _edgeToDummyVerticesMap = edgeToDummyVerticesMap;
        }

        public void Compute()
        {
            var edgeRoutingType = _layoutParameters.EdgeRoutingType;

            switch (edgeRoutingType)
            {
                case EdgeRoutingType.Straight:
                    EdgeRoutes = CalculateStraightEdgeRouting();
                    break;
                case EdgeRoutingType.Orthogonal:
                    EdgeRoutes = CalculateOrthogonalEdgeRouting();
                    break;
                default:
                    throw new Exception($"Unexpected EdgeRoutingType: {edgeRoutingType}");
            }
        }

        private Dictionary<IEdge<ISized>, Route> CalculateStraightEdgeRouting()
        {
            var edgeRoutes = new Dictionary<IEdge<ISized>, Route>();

            foreach (var edge in _originalEdges)
            {
                var internalRoutePoints = _edgeToDummyVerticesMap.GetRoutePoints(edge)?.ToList();

                var secondPoint = internalRoutePoints?.First() ?? NewCenter(edge.Target);
                var firstPoint = NewRect(edge.Source).GetAttachPointToward(secondPoint);

                var penultimatePoint = internalRoutePoints?.Last() ?? NewCenter(edge.Source);
                var lastPoint = NewRect(edge.Target).GetAttachPointToward(penultimatePoint);

                var route = new Route {firstPoint, internalRoutePoints, lastPoint};
                edgeRoutes.Add(edge, route);
            }

            return edgeRoutes;
        }

        private Dictionary<IEdge<ISized>, Route> CalculateOrthogonalEdgeRouting()
        {
            var edgeRoutes = new Dictionary<IEdge<ISized>, Route>();

            var layerDistance = _layoutParameters.LayerDistance;

            foreach (var edge in _originalEdges)
            {
                var sourceVertex = _sugiGraph.GetSugiVertexByOriginal(edge.Source);
                var targetVertex = _sugiGraph.GetSugiVertexByOriginal(edge.Target);

                var sourceLayer = _layers[sourceVertex];
                var targetLayer = _layers[targetVertex];

                var isUpsideDown = sourceVertex.LayerIndex > targetVertex.LayerIndex;

                var sourceVertical = isUpsideDown
                    ? sourceLayer.Position - layerDistance / 2.0
                    : sourceLayer.Position + sourceLayer.Height + layerDistance / 2.0;

                var secondPoint = new Point2D(sourceVertex.HorizontalPosition, sourceVertical);
                var firstPoint = NewRect(edge.Source).GetAttachPointToward(secondPoint);

                var targetVertical = isUpsideDown
                    ? targetLayer.Position + targetLayer.Height + layerDistance / 2.0
                    : targetLayer.Position - layerDistance / 2.0;

                var penultimatePoint = new Point2D(targetVertex.HorizontalPosition, targetVertical);
                var lastPoint = NewRect(edge.Target).GetAttachPointToward(penultimatePoint);

                var dummyVertexPoints = _edgeToDummyVerticesMap[edge]?.Select(i => new Point2D(i.HorizontalPosition, i.VerticalPosition)).ToList();

                Point2D? thirdPoint = null;
                Point2D? beforePenultimatePoint = null;
                if (dummyVertexPoints != null)
                {
                    thirdPoint = new Point2D(dummyVertexPoints.First().X, secondPoint.Y);
                    beforePenultimatePoint = new Point2D(dummyVertexPoints.Last().X, penultimatePoint.Y);
                }

                var route = new Route
                {
                    firstPoint, secondPoint, thirdPoint,
                    dummyVertexPoints,
                    beforePenultimatePoint, penultimatePoint, lastPoint
                };

                edgeRoutes.Add(edge, route);
            }

            return edgeRoutes;
        }

        private Point2D NewCenter(ISized vertex)
        {
            return _sugiGraph.GetNewCenter(vertex);
        }

        private Rect2D NewRect(ISized vertex)
        {
            var newCenter = NewCenter(vertex);
            var size = vertex.Size;
            return Rect2D.CreateFromCenterAndSize(newCenter, size);
        }
    }
}