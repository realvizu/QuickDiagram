using System.Diagnostics;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.Incremental
{
    /// <summary>
    /// Resolves the overlap of two vertices by sliding back the moving vertices in the direction of its original place.
    /// </summary>
    internal class BackSlidingOverlapResolver : IOverlapResolver
    {
        private readonly double _horizontalGap;
        private readonly double _originalCenterX;

        public BackSlidingOverlapResolver(double horizontalGap, double originalCenterX)
        {
            _horizontalGap = horizontalGap;
            _originalCenterX = originalCenterX;
        }

        public VertexMoveSpecification GetResolution(LayoutVertex movingVertex, LayoutVertex placedVertex)
        {
            var currentCenterX = movingVertex.Center.X;

            var newCenterX = _originalCenterX < currentCenterX
                ? placedVertex.Rect.Left - _horizontalGap - movingVertex.Width/2
                : placedVertex.Rect.Right + _horizontalGap + movingVertex.Width/2;

            Debug.WriteLine($"Resolving overlap ({movingVertex},{placedVertex}) by moving back {movingVertex} to X {newCenterX}.");

            return new VertexMoveSpecification(movingVertex, currentCenterX, newCenterX, withChildren: false);
        }
    }
}
