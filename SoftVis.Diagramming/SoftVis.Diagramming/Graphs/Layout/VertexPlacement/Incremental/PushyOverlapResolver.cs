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
        private TranslateDirection? _pushDirection;

        public PushyOverlapResolver(double horizontalGap)
        {
            _horizontalGap = horizontalGap;
        }

        public VertexMoveSpecification GetResolution(LayoutVertex movingVertex, LayoutVertex placedVertex)
        {
            if (_pushDirection == null)
                _pushDirection = movingVertex.Center.X >= placedVertex.Center.X
                        ? TranslateDirection.Left
                        : TranslateDirection.Right;

            var translateVectorX = _pushDirection == TranslateDirection.Left
                ? movingVertex.Left - placedVertex.Right - _horizontalGap
                : movingVertex.Right - placedVertex.Left + _horizontalGap;

            Debug.WriteLine($"Resolving overlap of ({movingVertex},{placedVertex}) by moving {placedVertex} by X {translateVectorX} with children.");

            return new VertexMoveSpecification(placedVertex, placedVertex.Center.X, 
                placedVertex.Center.X + translateVectorX, withChildren: true);
        }
    }
}
