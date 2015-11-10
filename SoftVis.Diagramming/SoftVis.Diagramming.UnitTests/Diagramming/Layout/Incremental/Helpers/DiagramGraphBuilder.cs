using System.Linq;
using Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.TestSubjects;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Helpers
{
    internal class DiagramGraphBuilder
    {
        public DiagramGraph DiagramGraph { get; }

        public DiagramGraphBuilder()
        {
            DiagramGraph = new DiagramGraph();
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

        public DiagramNode AddVertex(string vertexName)
        {
            var vertex = CreateVertex(vertexName);
            DiagramGraph.AddVertex(vertex);
            return vertex;
        }

        public DiagramConnector AddEdge(string sourceVertexName, string targetVertexName)
        {
            var edge =  CreateEdge(sourceVertexName, targetVertexName);
            DiagramGraph.AddVertex(edge.Source);
            DiagramGraph.AddVertex(edge.Target);
            DiagramGraph.AddEdge(edge);
            return edge;
        }

        public DiagramConnector RemoveEdge(string sourceVertexName, string targetVertexName)
        {
            var source = GetVertex(sourceVertexName);
            var target = GetVertex(targetVertexName);
            var edge = DiagramGraph.OutEdges(source).First(i => i.Target == target);
            DiagramGraph.RemoveEdge(edge);
            return edge;
        }

        public DiagramNode GetVertex(string vertexName)
        {
            return DiagramGraph.Vertices.FirstOrDefault(i => i.Name == vertexName);
        }

        public DiagramConnector GetEdge(string sourceName, string targetName)
        {
            return DiagramGraph.Edges
                .FirstOrDefault(i => i.Source == GetVertex(sourceName) && i.Target == GetVertex(targetName));
        }

        private DiagramNode CreateVertex(string name)
        {
            return GetVertex(name) ?? new TestDiagramNode(name);
        }

        private DiagramConnector CreateEdge(string sourceVertexName, string targetVertexName)
        {
            var edge = GetEdge(sourceVertexName, targetVertexName);
            if (edge != null)
                return edge;

            var source = CreateVertex(sourceVertexName);
            var target = CreateVertex(targetVertexName);
            return new TestDiagramConnector(source, target);
        }
    }
}