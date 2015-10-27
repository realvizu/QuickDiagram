using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// An edge in the layering graph.
    /// Corresponds to a DiagramConnector but can be reversed to avoid cycles in the layering graph.
    /// </summary>
    internal class LayeringEdge : Edge<LayeringVertex>
    {
        public bool IsReversed { get; set; }

        public LayeringEdge(LayeringVertex source, LayeringVertex target) 
            : base(source, target)
        {
        }

        public int LayerSpan => Source.LayerIndex - Target.LayerIndex;

        public override string ToString()
        {
            return $"{Source}->{Target}";
        }
    }
}
