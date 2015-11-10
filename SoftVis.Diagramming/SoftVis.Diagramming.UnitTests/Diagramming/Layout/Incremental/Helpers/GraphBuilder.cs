using System.Linq;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Helpers
{
    internal abstract class GraphBuilder<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : IMutableBidirectionalGraph<TVertex, TEdge>, new()
    {
        public TGraph Graph { get; }

        protected GraphBuilder()
        {
            Graph = new TGraph();
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

        public TVertex GetVertex(string name)
        {
            return Graph.Vertices.FirstOrDefault(i => i.ToString() == name);
        }

        public TEdge GetEdge(string sourceName, string targetName)
        {
            return Graph.Edges.FirstOrDefault(i => i.Source.Equals(GetVertex(sourceName)) && i.Target.Equals(GetVertex(targetName)));
        }

        public TVertex AddVertex(string name)
        {
            var vertex = CreateVertex(name);
            Graph.AddVertex(vertex);
            return vertex;
        }

        public TEdge AddEdge(string sourceName, string targetName)
        {
            var edge = CreateEdge(sourceName, targetName);
            Graph.AddVertex(edge.Source);
            Graph.AddVertex(edge.Target);
            Graph.AddEdge(edge);
            return edge;
        }

        public TVertex RemoveVertex(string name)
        {
            var vertex = GetVertex(name);
            Graph.RemoveVertex(vertex);
            return vertex;
        }

        public TEdge RemoveEdge(string sourceName, string targetName)
        {
            var edge = GetEdge(sourceName, targetName);
            Graph.RemoveEdge(edge);
            return edge;
        }

        protected abstract TVertex CreateNewVertex(string name);
        protected abstract TEdge CreateNewEdge(TVertex source, TVertex target);

        private TVertex CreateVertex(string name)
        {
            return GetVertex(name) ?? CreateNewVertex(name);
        }

        private TEdge CreateEdge(string sourceName, string targetName)
        {
            var edge = GetEdge(sourceName, targetName);
            if (edge != null)
                return edge;

            var source = CreateVertex(sourceName);
            var target = CreateVertex(targetName);
            return CreateNewEdge(source, target);
        }
    }
}
