using System.Linq;
using Codartis.SoftVis.Diagramming.Layout.Incremental;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Helpers
{
    internal class LowLevelLayoutGraphBuilder
    {
        public LowLevelLayoutGraph LowLevelLayoutGraph { get; }

        public LowLevelLayoutGraphBuilder()
        {
            LowLevelLayoutGraph = new LowLevelLayoutGraph();
        }

        public void SetUp(params string[] pathSpecifications)
        {
            foreach (var pathSpecification in pathSpecifications)
            {
                foreach (var vertexName in BuilderHelper.PathSpecificationToVertexNames(pathSpecification))
                    AddVertex(vertexName);

                foreach (var edgeSpecification in BuilderHelper.StringToEdgeSpecifications(pathSpecification))
                    AddEdge(edgeSpecification.SourceVertexName, edgeSpecification.TargetVertexName);
            }
        }

        public LayoutVertexBase AddVertex(string vertexName, int priority = 1)
        {
            var vertex = CreateVertex(vertexName, priority);
            LowLevelLayoutGraph.AddVertex(vertex);
            return vertex;
        }

        public LayoutEdge AddEdge(string sourceVertexName, string targetVertexName)
        {
            var edge =  CreateEdge(sourceVertexName, targetVertexName);
            LowLevelLayoutGraph.AddVertex(edge.Source);
            LowLevelLayoutGraph.AddVertex(edge.Target);
            LowLevelLayoutGraph.AddEdge(edge);
            return edge;
        }

        public LayoutEdge RemoveEdge(string sourceVertexName, string targetVertexName)
        {
            var source = GetVertex(sourceVertexName);
            var target = GetVertex(targetVertexName);
            var edge = LowLevelLayoutGraph.OutEdges(source).First(i => i.Target == target);
            LowLevelLayoutGraph.RemoveEdge(edge);
            return edge;
        }

        public LayoutVertexBase GetVertex(string vertexName)
        {
            return LowLevelLayoutGraph.Vertices.FirstOrDefault(i => i.Name == vertexName);
        }

        private LayoutEdge GetEdge(string sourceName, string targetName)
        {
            return LowLevelLayoutGraph.Edges
                .FirstOrDefault(i => i.Source == GetVertex(sourceName) && i.Target == GetVertex(targetName));
        }

        private LayoutVertexBase CreateVertex(string name, int priority = 1)
        {
            return GetVertex(name) ?? LayoutGraphFixtureHelper.CreateVertex(name, priority);
        }

        private LayoutEdge CreateEdge(string sourceVertexName, string targetVertexName)
        {
            var edge = GetEdge(sourceVertexName, targetVertexName);
            if (edge != null)
                return edge;

            var source = CreateVertex(sourceVertexName);
            var target = CreateVertex(targetVertexName);
            return new LayoutEdge(source, target, null);
        }
    }
}