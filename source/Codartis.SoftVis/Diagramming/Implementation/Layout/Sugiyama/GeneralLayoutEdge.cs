using Codartis.SoftVis.Diagramming.Definition;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama
{
    /// <summary>
    /// An edge that can connect any subtypes of layout vertices.
    /// </summary>
    internal class GeneralLayoutEdge : LayoutEdge<LayoutVertexBase>
    {
        public GeneralLayoutEdge(LayoutVertexBase source, LayoutVertexBase target,
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
