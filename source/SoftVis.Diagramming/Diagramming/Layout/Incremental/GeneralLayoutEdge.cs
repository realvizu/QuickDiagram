namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// An edge that can connect any subtypes of layout vertices.
    /// </summary>
    public class GeneralLayoutEdge : LayoutEdge<LayoutVertexBase>
    {
        public GeneralLayoutEdge(
            LayoutVertexBase source,
            LayoutVertexBase target,
            IDiagramConnector diagramConnector)
            : base(source, target, diagramConnector)
        {
        }

        public GeneralLayoutEdge Reverse()
        {
            return new GeneralLayoutEdge(Target, Source, DiagramConnector);
        }
    }
}