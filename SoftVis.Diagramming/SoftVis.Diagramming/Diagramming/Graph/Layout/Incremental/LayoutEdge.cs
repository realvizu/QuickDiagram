using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.Incremental
{
    /// <summary>
    /// An edge in the layout graph.
    /// </summary>
    internal abstract class LayoutEdge<TVertex> : Edge<TVertex>
    {
        public DiagramConnector DiagramConnector { get; }

        protected LayoutEdge(TVertex source, TVertex target, DiagramConnector diagramConnector)
            : base(source, target)
        {
            DiagramConnector = diagramConnector;
        }

        public override string ToString()
        {
            return $"{Source}->{Target}";
        }
    }
}
