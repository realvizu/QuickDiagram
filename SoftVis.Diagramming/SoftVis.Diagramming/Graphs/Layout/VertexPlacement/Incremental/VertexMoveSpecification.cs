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
        public IOverlapResolver ResolverForChildren { get; }

        public VertexMoveSpecification(LayoutVertex vertex, double fromCenterX, double toCenterX, IOverlapResolver resolverForChildren = null)
        {
            Vertex = vertex;
            FromCenterX = fromCenterX;
            ToCenterX = toCenterX;
            ResolverForChildren = resolverForChildren;
        }

        public double TranslateVectorX => ToCenterX - FromCenterX;
        public bool WithChildren => ResolverForChildren != null;
    }
}