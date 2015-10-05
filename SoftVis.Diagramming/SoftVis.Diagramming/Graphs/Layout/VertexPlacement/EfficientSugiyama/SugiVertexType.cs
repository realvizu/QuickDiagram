namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.EfficientSugiyama
{
    internal enum SugiVertexType
    {
        /// <summary>
        /// This vertex existed in the original graph too.
        /// </summary>
        Original,
        /// <summary>
        /// The start of a segment. (When the original edge spans more than 2 layers.)
        /// </summary>
        PVertex,
        /// <summary>
        /// The end of a segment. (When the original edge spans more than 2 layers.)
        /// </summary>
        QVertex,
        /// <summary>
        /// The midpoint of the original edge that spans 2 layers.
        /// </summary>
        RVertex
    }
}