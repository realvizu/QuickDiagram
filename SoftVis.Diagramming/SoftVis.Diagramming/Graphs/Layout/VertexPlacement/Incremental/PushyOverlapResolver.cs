using System.Diagnostics;
using System.Linq;
using Codartis.SoftVis.Geometry;
using MoreLinq;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.Incremental
{
    /// <summary>
    /// Resolves the overlap of two vertices by pushing away the placed vertex.
    /// If the moving and the placed vertex are not siblings then it pushes away all of the placed vertex' siblings too.
    /// </summary>
    internal class PushyOverlapResolver : IOverlapResolver
    {
        private readonly LayoutGraph _layoutGraph;
        private readonly double _horizontalGap;
        private readonly double _insertionCenterX;

        public PushyOverlapResolver(LayoutGraph layoutGraph, double horizontalGap, double insertionCenterX)
        {
            _layoutGraph = layoutGraph;
            _horizontalGap = horizontalGap;
            _insertionCenterX = insertionCenterX;
        }

        public VertexMoveSpecification GetResolution(LayoutVertex movingVertex, LayoutVertex placedVertex)
        {
            var parentsOfPlacedVertex = _layoutGraph.GetOutNeighbours(placedVertex).ToArray();

            LayoutVertex pushedVertex;
            Rect2D pushedRect;
            if (!AreSiblings(movingVertex, placedVertex) && parentsOfPlacedVertex.Any())
            {
                pushedVertex = GetNearestParent(parentsOfPlacedVertex);
                pushedRect = _layoutGraph.GetInNeighbours(pushedVertex).Select(i => i.Rect).Union();
            }
            else
            {
                pushedVertex = placedVertex;
                pushedRect = placedVertex.Rect;
            }

            var pushDirection = _insertionCenterX >= pushedVertex.Center.X
                        ? TranslateDirection.Left
                        : TranslateDirection.Right;

            var translateVectorX = pushDirection == TranslateDirection.Left
                ? movingVertex.Left - pushedRect.Right - _horizontalGap
                : movingVertex.Right - pushedRect.Left + _horizontalGap;

            Debug.WriteLine($"Resolving overlap of ({movingVertex},{placedVertex}) by moving {pushedVertex} by X {translateVectorX} with children (pushDirection:{pushDirection}).");

            var targetCenterX = pushedVertex.Center.X + translateVectorX;
            return new VertexMoveSpecification(pushedVertex, pushedVertex.Center.X, targetCenterX, this);
        }

        private bool AreSiblings(LayoutVertex movingVertex, LayoutVertex placedVertex)
        {
            return _layoutGraph.AreSiblings(movingVertex, placedVertex, EdgeDirection.Out);
        }

        private LayoutVertex GetNearestParent(LayoutVertex[] parentsOfPlacedVertex)
        {
            return parentsOfPlacedVertex.MinBy(i => i.Center.X - _insertionCenterX);
        }
    }
}
