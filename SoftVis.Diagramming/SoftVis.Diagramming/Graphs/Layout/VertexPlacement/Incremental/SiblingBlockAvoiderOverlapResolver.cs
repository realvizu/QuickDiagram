using System.Diagnostics;
using MoreLinq;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.Incremental
{
    internal class SiblingBlockAvoiderOverlapResolver : IOverlapResolver
    {
        private readonly LayoutGraph _layoutGraph;
        private readonly double _insertionCenterX;
        private readonly double _horizontalGap;

        public SiblingBlockAvoiderOverlapResolver(LayoutGraph layoutGraph, double insertionCenterX, double horizontalGap)
        {
            _layoutGraph = layoutGraph;
            _insertionCenterX = insertionCenterX;
            _horizontalGap = horizontalGap;
        }

        public VertexMoveSpecification GetResolution(LayoutVertex movingVertex, LayoutVertex placedVertex)
        {
            var parentsOfPlacedVertex = _layoutGraph.GetOutNeighbours(placedVertex);
            var nearestParent = parentsOfPlacedVertex.MinBy(i => i.Center.X - _insertionCenterX);

            var pushDirection = _insertionCenterX >= nearestParent.Center.X
                       ? TranslateDirection.Left
                       : TranslateDirection.Right;

            var translateVectorX = pushDirection == TranslateDirection.Left
                ? movingVertex.Left - placedVertex.Right - _horizontalGap
                : movingVertex.Right - placedVertex.Left + _horizontalGap;

            Debug.WriteLine($"Resolving overlap of ({movingVertex},{placedVertex}) by moving {placedVertex} of sibling block by {translateVectorX} with children (pushDirection:{pushDirection}).");

            var targetCenterX = placedVertex.Center.X + translateVectorX;
            var resolverForChildren = new PushyOverlapResolver(_horizontalGap, _insertionCenterX);
            return new VertexMoveSpecification(placedVertex, placedVertex.Center.X, targetCenterX, resolverForChildren);

        }
    }
}