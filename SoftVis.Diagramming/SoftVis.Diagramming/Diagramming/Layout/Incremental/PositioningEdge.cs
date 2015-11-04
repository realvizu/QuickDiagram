using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// An edge in the positioning graph.
    /// </summary>
    internal class PositioningEdge : Edge<PositioningVertexBase>
    {
        public DiagramConnector DiagramConnector { get; }

        public PositioningEdge(PositioningVertexBase source, PositioningVertexBase target,
            DiagramConnector diagramConnector)
            : base(source, target)
        {
            DiagramConnector = diagramConnector;
        }

        public PositioningEdge Reverse()
        {
            return new PositioningEdge(Target, Source, DiagramConnector);
        }

        public override string ToString()
        {
            return $"{Source}->{Target}";
        }
    }
}
