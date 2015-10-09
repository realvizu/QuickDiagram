using System.Diagnostics;
using System.Linq;
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

            var pushDirectionCutOffPointX = !AreSiblings(movingVertex, placedVertex) && parentsOfPlacedVertex.Any()
                ? GetNearestParent(parentsOfPlacedVertex).Center.X
                : placedVertex.Center.X;

            var pushDirection = _insertionCenterX >= pushDirectionCutOffPointX
                        ? TranslateDirection.Left
                        : TranslateDirection.Right;

            var translateVectorX = pushDirection == TranslateDirection.Left
                ? movingVertex.Left - placedVertex.Right - _horizontalGap
                : movingVertex.Right - placedVertex.Left + _horizontalGap;

            Debug.WriteLine($"Resolving overlap of ({movingVertex},{placedVertex}) by moving {placedVertex} by X {translateVectorX} with children (pushDirection:{pushDirection}).");

            var targetCenterX = placedVertex.Center.X + translateVectorX;
            return new VertexMoveSpecification(placedVertex, placedVertex.Center.X, targetCenterX, this);
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
