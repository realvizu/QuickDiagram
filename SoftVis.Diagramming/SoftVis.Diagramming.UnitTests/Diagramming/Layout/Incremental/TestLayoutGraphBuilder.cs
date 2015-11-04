using System.Linq;
using Codartis.SoftVis.Diagramming.Layout.Incremental;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental
{
    internal class TestLayoutGraphBuilder
    {
        public LayoutGraph LayoutGraph { get; }

        public TestLayoutGraphBuilder()
        {
            LayoutGraph = new LayoutGraph();
        }

        public void SetUp(params string[] pathSpecifications)
        {
            foreach (var pathSpecification in pathSpecifications)
            {
                foreach (var vertexName in BuilderHelper.GetVertexNames(pathSpecification))
                    AddVertex(vertexName);

                foreach (var edgeSpecification in BuilderHelper.GetEdgeSpecifications(pathSpecification))
                    AddEdge(edgeSpecification.SourceVertexName, edgeSpecification.TargetVertexName);
            }
        }

        public LayoutVertexBase AddVertex(string vertexName, int priority = 1)
        {
            var vertex = CreateVertex(vertexName, priority);
            LayoutGraph.AddVertex(vertex);
            return vertex;
        }

        public LayoutEdge AddEdge(string sourceVertexName, string targetVertexName)
        {
            var edge = CreateEdge(sourceVertexName, targetVertexName);
            LayoutGraph.AddEdge(edge);
            return edge;
        }

        public LayoutEdge RemoveEdge(string sourceVertexName, string targetVertexName)
        {
            var source = GetVertex(sourceVertexName);
            var target = GetVertex(targetVertexName);
            var edge = LayoutGraph.OutEdges(source).First(i => i.Target == target);
            LayoutGraph.RemoveEdge(edge);
            return edge;
        }

        public LayoutVertexBase GetVertex(string vertexName)
        {
            return LayoutGraph.Vertices.FirstOrDefault(i => i.Name == vertexName);
        }

        private static LayoutVertexBase CreateVertex(string name, int priority = 1)
        {
            return name.StartsWith("*")
                ? (LayoutVertexBase)new TestDummyLayoutVertex(int.Parse(name.Substring(1)), true)
                : new TestLayoutVertex(name, true, priority);
        }

        private LayoutEdge CreateEdge(string sourceVertexName, string targetVertexName)
        {
            var source = GetVertex(sourceVertexName);
            var target = GetVertex(targetVertexName);
            return new LayoutEdge(source, target, null);
        }
    }
}