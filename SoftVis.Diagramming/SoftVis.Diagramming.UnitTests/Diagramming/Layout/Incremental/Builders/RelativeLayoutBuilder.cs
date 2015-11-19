using System.Linq;
using Codartis.SoftVis.Diagramming.Layout.Incremental;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Builders
{
    internal class RelativeLayoutBuilder : BuilderBase
    {
        public LayeredLayoutGraphBuilder GraphBuilder { get; }
        public LayoutVertexLayersBuilder LayersBuilder { get; }
        public RelativeLayout RelativeLayout { get; }

        public RelativeLayoutBuilder()
        {
            GraphBuilder = new LayeredLayoutGraphBuilder();
            LayersBuilder = new LayoutVertexLayersBuilder(GraphBuilder.Graph.ProperGraph);
            RelativeLayout = new RelativeLayout(GraphBuilder.Graph, LayersBuilder.Layers);
        }

        public void SetUpGraph(params string[] pathSpecification)
        {
            GraphBuilder.SetUp(pathSpecification);

        }

        public void SetUpLayers(params string[] layerSpecifications)
        {
            LayersBuilder.SetUp(layerSpecifications);
        }

        public LayoutVertexBase GetVertex(string name)
        {
            return GraphBuilder.Graph
                .ProperGraph.Vertices.FirstOrDefault(i => i.ToString() == name);
        }
    }
}
