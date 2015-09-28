using System.Diagnostics;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.VertexPlacement.SimplifiedSugiyama
{
    [DebuggerDisplay("{ToString()}")]
    internal class LayoutEdge : Edge<LayoutVertex>
    { 
        // TODO: eliminate original?
        public IEdge<IExtent> OriginalEdge { get; }
        public bool IsReversed { get; }

        public LayoutEdge(IEdge<IExtent> originalEdge, LayoutVertex source, LayoutVertex target, bool isReversed = false) 
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
