using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// An edge in the layout graph.
    /// </summary>
    internal class LayoutEdge : Edge<LayoutVertexBase>
    {
        public DiagramConnector DiagramConnector { get; }

        public LayoutEdge(LayoutVertexBase source, LayoutVertexBase target,
            DiagramConnector diagramConnector)
            : base(source, target)
        {
            DiagramConnector = diagramConnector;
        }

        public LayoutEdge Reverse()
        {
            return new LayoutEdge(Target, Source, DiagramConnector);
        }

        public override string ToString()
        {
            return $"{Source}->{Target}";
        }
    }
}
