namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.Incremental
{
    /// <summary>
    /// Describes a vertex movement.
    /// </summary>
    internal struct VertexMoveSpecification
    {
        public LayoutVertex Vertex { get; }
        public double FromCenterX { get; }
        public double ToCenterX { get; }
        public bool WithChildren { get; }

        public VertexMoveSpecification(LayoutVertex vertex, double fromCenterX, double toCenterX, bool withChildren)
        {
            Vertex = vertex;
            FromCenterX = fromCenterX;
            ToCenterX = toCenterX;
            WithChildren = withChildren;
        }

        public double TranslateVectorX => ToCenterX - FromCenterX;
    }
}