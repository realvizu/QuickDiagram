namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.Incremental
{
    /// <summary>
    /// Calculates a resolution for the overlap of two vertices (a moving and a placed) by moving one of them.
    /// </summary>
    internal interface IOverlapResolver
    {
        VertexMoveSpecification GetResolution(LayoutVertex movingVertex, LayoutVertex placedVertex);
    }
}
