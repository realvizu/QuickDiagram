using System;
using System.Diagnostics;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// An edge in the positioning graph.
    /// </summary>
    internal class PositioningEdge : Edge<PositioningVertexBase>
    {
        private readonly PositioningGraph _graph;
        public DiagramConnector DiagramConnector { get; }

        public PositioningEdge(PositioningGraph graph, PositioningVertexBase source, PositioningVertexBase target,
            DiagramConnector diagramConnector)
            : base(source, target)
        {
            _graph = graph;
            DiagramConnector = diagramConnector;
        }

        public bool IsPrimary => Source.GetPrimaryParent() == Target;
        public void ExecuteOnDescendantEdges(Action<PositioningEdge> action) => _graph.ExecuteOnDescendantEdges(this, action);

        public PositioningEdge Reverse()
        {
            return new PositioningEdge(_graph, Target, Source, DiagramConnector);
        }

        public override string ToString()
        {
            return $"{Source}->{Target}";
        }
    }
}
