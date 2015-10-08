using System.Diagnostics;
using Codartis.SoftVis.Geometry;
using QuickGraph;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.Incremental
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
        public IEdge<IRect> OriginalEdge { get; }
        public bool IsReversed { get; }

        public LayoutEdge(IEdge<IRect> originalEdge, LayoutVertex source, LayoutVertex target, bool isReversed = false) 
            : base(source, target)
        {
            OriginalEdge = originalEdge;
            IsReversed = isReversed;
        }

        public LayoutEdge Reverse()
        {
            return new LayoutEdge(OriginalEdge, Target, Source, true);
        }

        private string IsReversedAsString => IsReversed ? " (reversed)" : "";

        public override string ToString()
        {
            return $"{Source}->{Target} Original: {OriginalEdge}{IsReversedAsString}";
        }
    }
}
