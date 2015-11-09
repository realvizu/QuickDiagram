namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative
{
    public struct RelativeLocation
    {
        public int LayerIndex { get; }
        public int IndexInLayer { get; }

        public RelativeLocation(int layerIndex, int indexInLayer)
        {
            LayerIndex = layerIndex;
            IndexInLayer = indexInLayer;
        }
    }
}
