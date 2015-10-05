using System.Diagnostics;
using Codartis.SoftVis.Geometry;
using QuickGraph;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.SimplifiedSugiyama
{
    [DebuggerDisplay("{ToString()}")]
    internal class LayoutEdge : Edge<LayoutVertex>
    { 
        // TODO: eliminate original?
        public IEdge<ISized> OriginalEdge { get; }
        public bool IsReversed { get; }

        public LayoutEdge(IEdge<ISized> originalEdge, LayoutVertex source, LayoutVertex target, bool isReversed = false) 
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
