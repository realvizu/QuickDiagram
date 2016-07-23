using System.Linq;
using Codartis.SoftVis.Diagramming.Implementation.Layout.Incremental;
using Codartis.SoftVis.Diagramming.Implementation.Layout.Incremental.Relative;
using Codartis.SoftVis.Diagramming.Implementation.Layout.Incremental.Relative.Logic;
using MoreLinq;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Builders
{
    internal class LayoutVertexLayersBuilder : BuilderBase
    {
        private readonly IReadOnlyQuasiProperLayoutGraph _layoutGraph;
        public LayoutVertexLayers Layers { get; }

        public LayoutVertexLayersBuilder(IReadOnlyQuasiProperLayoutGraph layoutGraph)
        {
            _layoutGraph = layoutGraph;
            Layers = new LayoutVertexLayers();
        }

        public void SetUp(params string[] layerSpecifications)
        {
            for (var layerIndex = 0; layerIndex < layerSpecifications.Length; layerIndex++)
            {
                var layerSpecification = layerSpecifications[layerIndex];
                var index = layerIndex;
                layerSpecification.Split(',').ForEach(i => AddVertexToLayer(i, index));
            }
        }

        private void AddVertexToLayer(string vertexName, int layerIndex)
        {
            var vertex = GetVertex(vertexName);
            var relativeLocation = new RelativeLocation(layerIndex, Layers.GetLayer(layerIndex).Count);
            Layers.AddVertex(vertex, relativeLocation);
        }

        private LayoutVertexBase GetVertex(string name)
        {
            return _layoutGraph.Vertices.FirstOrDefault(i => i.ToString() == name);
        }
    }
}
