using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.SimplifiedSugiyama
{
    internal class LayoutEdge : Edge<LayoutVertex>
    {
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
    }
}
