using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.VertexPlacement.EfficientSugiyama
{
    internal class SugiEdge : TaggedEdge<SugiVertex, IEdge<IExtent>>
    {
        internal SugiEdge(IEdge<IExtent> originalEdge, SugiVertex source, SugiVertex target)
            : base(source, target, originalEdge) { }

        public IEdge<IExtent> OriginalEdge => Tag;

        /// <summary>
        /// Gets or sets that the edge is included in a 
        /// type 1 conflict as a non-inner segment (true) or not (false).
        /// </summary>
        public bool Marked;
        private bool _tempMark;

        public void SaveMarkedToTemp()
        {
            _tempMark = Marked;
        }

        public void LoadMarkedFromTemp()
        {
            Marked = _tempMark;
        }

        public SugiEdge Reverse()
        {
            return new SugiEdge(OriginalEdge, Target, Source);
        }
    }
}