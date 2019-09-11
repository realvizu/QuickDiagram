using Codartis.SoftVis.Diagramming.Definition;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama
{
    /// <summary>
    /// An edge in the layout graph.
    /// </summary>
    internal abstract class LayoutEdge<TVertex> : Edge<TVertex>
    {
        public IDiagramConnector DiagramConnector { get; }

        protected LayoutEdge(TVertex source, TVertex target, IDiagramConnector diagramConnector)
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
