using System.Collections.Generic;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.EfficientSugiyama
{
    internal class Segment : Data
    {
        public SugiVertex PVertex { get; }
        public SugiVertex QVertex { get; }

        private Segment(SugiVertex pVertex, SugiVertex qVertex)
        {
            PVertex = pVertex;
            QVertex = qVertex;
        }

        public static Segment Create(Layer pVertexLayer, Layer qVertexLayer)
        {
            var pVertex = SugiVertex.CreatePVertex(pVertexLayer);
            var qVertex = SugiVertex.CreateQVertex(qVertexLayer);

            var segment = new Segment(pVertex, qVertex);
            pVertex.Segment = segment;
            qVertex.Segment = segment;
            return segment;
        }

        public IEnumerable<SugiVertex> Vertices
        {
            get
            {
                yield return PVertex;
                yield return QVertex;
            }
        }
    }
}