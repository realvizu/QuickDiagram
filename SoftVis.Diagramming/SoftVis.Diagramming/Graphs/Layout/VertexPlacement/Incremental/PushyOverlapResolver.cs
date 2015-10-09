using System.Diagnostics;
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
            double leftRightPushCutOffX;
            if (_layoutGraph.AreSiblings(movingVertex, placedVertex, EdgeDirection.Out))
            {
                leftRightPushCutOffX = placedVertex.Center.X;
            }
            else
            {
                var parentsOfPlacedVertex = _layoutGraph.GetOutNeighbours(placedVertex);
                var nearestParent = parentsOfPlacedVertex.MinBy(i => i.Center.X - _insertionCenterX);
                leftRightPushCutOffX = nearestParent.Center.X;
            }

            var pushDirection = _insertionCenterX >= leftRightPushCutOffX
                        ? TranslateDirection.Left
                        : TranslateDirection.Right;

            var translateVectorX = pushDirection == TranslateDirection.Left
                ? movingVertex.Left - placedVertex.Right - _horizontalGap
                : movingVertex.Right - placedVertex.Left + _horizontalGap;

            Debug.WriteLine($"Resolving overlap of ({movingVertex},{placedVertex}) by moving {placedVertex} by X {translateVectorX} with children (pushDirection:{pushDirection}).");

            var targetCenterX = placedVertex.Center.X + translateVectorX;
            return new VertexMoveSpecification(placedVertex, placedVertex.Center.X, targetCenterX, this);
        }
    }
}
