namespace Codartis.SoftVis.Diagramming.Implementation.Layout.Incremental
{
    /// <summary>
    /// An edge that can connect any subtypes of layout vertices.
    /// </summary>
    internal class GeneralLayoutEdge : LayoutEdge<LayoutVertexBase>
    {
        public GeneralLayoutEdge(LayoutVertexBase source, LayoutVertexBase target,
            DiagramConnector diagramConnector)
            : base(source, target, diagramConnector)
        {
        }

        public GeneralLayoutEdge Reverse()
        {
            return new GeneralLayoutEdge(Target, Source, DiagramConnector);
        }
    }
}
