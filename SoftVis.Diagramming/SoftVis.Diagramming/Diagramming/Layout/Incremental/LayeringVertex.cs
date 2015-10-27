namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// A vertex in the layering graph.
    /// Corresponds to a DiagramNode. 
    /// </summary>
    internal class LayeringVertex
    {
        public DiagramNode DiagramNode { get; }
        public int LayerIndex { get; set; }

        public LayeringVertex(DiagramNode diagramNode)
        {
            DiagramNode = diagramNode;
        }

        public override string ToString()
        {
            return $"{DiagramNode.Name}";
        }
    }
}
