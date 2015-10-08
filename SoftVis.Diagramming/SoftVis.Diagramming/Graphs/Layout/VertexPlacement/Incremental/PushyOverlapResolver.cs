using System.Diagnostics;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.Incremental
{
    /// <summary>
    /// Resolves the overlap of two vertices by pushing away the placed vertex.
    /// </summary>
    /// <remarks>
    /// At the first usage it calculates and stores the push direction and subsequently uses that.
    /// </remarks>
    internal class PushyOverlapResolver : IOverlapResolver
    {
        private readonly double _horizontalGap;
        private readonly double _originalCenterX;

        public PushyOverlapResolver(double horizontalGap, double originalCenterX)
        {
            _horizontalGap = horizontalGap;
            _originalCenterX = originalCenterX;
        }

        public VertexMoveSpecification GetResolution(LayoutVertex movingVertex, LayoutVertex placedVertex)
        {
             var pushDirection = _originalCenterX >= placedVertex.Center.X
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
