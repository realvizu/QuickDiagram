using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Layout
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
    internal class LayoutEdge : Edge<LayoutVertexBase>
    {
        private readonly LayoutGraph _graph;
        public DiagramConnector DiagramConnector { get; }
        public bool IsReversed { get; }

        public LayoutEdge(LayoutGraph graph, LayoutVertexBase source, LayoutVertexBase target,
            DiagramConnector diagramConnector, bool isReversed = false)
            : base(source, target)
        {
            _graph = graph;
            DiagramConnector = diagramConnector;
            IsReversed = isReversed;
        }

        public bool IsPrimary => Source.GetPrimaryParent() == Target;
        public void ExecuteOnDescendantEdges(Action<LayoutEdge> action) => _graph.ExecuteOnDescendantEdges(this, action);

        public LayoutEdge Reverse()
        {
            return new LayoutEdge(_graph, Target, Source, DiagramConnector, true);
        }

        public IEnumerable<LayoutEdge> Split(LayoutVertexBase layoutVertex)
        {
            yield return new LayoutEdge(_graph, Source, layoutVertex, DiagramConnector, IsReversed);
            yield return new LayoutEdge(_graph, layoutVertex, Target, DiagramConnector, IsReversed);
        }

        public IEnumerable<LayoutEdge> Split(IEnumerable<LayoutVertexBase> layoutVertices)
        {
            var layoutVertexBases = layoutVertices as LayoutVertexBase[] ?? layoutVertices.ToArray();

            if (!layoutVertexBases.Any())
            {
                yield return this;
                yield break;
            }

            var edges = Split(layoutVertexBases.First()).ToArray();
            yield return edges.First();

            foreach (var edge in edges.Last().Split(layoutVertexBases.Skip(1)))
                yield return edge;
        }

        private string IsReversedAsString => IsReversed ? " (reversed)" : "";

        public override string ToString()
        {
            return $"{Source}->{Target} Original: {DiagramConnector}{IsReversedAsString}";
        }

    }
}
