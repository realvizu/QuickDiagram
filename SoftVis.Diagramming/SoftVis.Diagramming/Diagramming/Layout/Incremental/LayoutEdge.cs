using System;
using System.Diagnostics;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// An edge used in LayoutGraphs.
    /// Always resembles an edge from the original graph.
    /// </summary>
    /// <remarks>
    /// <para>More than one LayouEdge can resemble the same original 
    /// because original edges are broken into multiple LayoutEdges at dummy vertices.</para>
    /// <para>A layout edge can be the reverse of the original to ensure an acyclic layout graph.
    /// These reversed edges must be interpreted backwards when drawing the original graph.</para>
    /// </remarks>
    [DebuggerDisplay("{ToString()}")]
    internal class LayoutEdge : Edge<LayoutVertex>
    {
        private readonly LayoutGraph _graph;
        public DiagramConnector DiagramConnector { get; }
        public bool IsReversed { get; }

        public LayoutEdge(LayoutGraph graph, LayoutVertex source, LayoutVertex target, 
            DiagramConnector diagramConnector, bool isReversed = false) 
            : base(source, target)
        {
            _graph = graph;
            DiagramConnector = diagramConnector;
            IsReversed = isReversed;
        }

        public void TraverseInEdges(Action<LayoutEdge> action)
        {
            _graph.TraverseInEdges(this, action);
        }

        public LayoutEdge Reverse()
        {
            return new LayoutEdge(_graph, Target, Source, DiagramConnector, true);
        }

        private string IsReversedAsString => IsReversed ? " (reversed)" : "";

        public override string ToString()
        {
            return $"{Source}->{Target} Original: {DiagramConnector}{IsReversedAsString}";
        }
    }
}
