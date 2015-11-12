using System.Linq;
using Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Helpers;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Builders
{
    internal abstract class GraphBuilderBase<TVertex, TEdge, TGraph> : BuilderBase
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : IMutableBidirectionalGraph<TVertex, TEdge>, new()
    {
        public TGraph Graph { get; }

        protected GraphBuilderBase()
        {
            Graph = new TGraph();
        }

        public void SetUp(params string[] pathStrings)
        {
            foreach (var pathString in pathStrings)
            {
                var pathSpecification = GetPathSpecification(pathString);

                foreach (var vertexName in pathSpecification)
                    AddVertex(vertexName);

                foreach (var edgeSpecification in pathSpecification.ToEdgeSpecifications())
                    AddEdge(edgeSpecification.SourceVertexName, edgeSpecification.TargetVertexName);
            }
        }

        protected virtual PathSpecification GetPathSpecification(string pathString)
        {
            return PathSpecification.Parse(pathString);
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
            var vertex = GetOrCreateVertex(name);
            if (vertex != null)
                Graph.AddVertex(vertex);
            return vertex;
        }

        public TEdge AddEdge(string sourceName, string targetName)
        {
            var edge = GetOrCreateEdge(sourceName, targetName);
            if (edge != null)
            {
                Graph.AddVertex(edge.Source);
                Graph.AddVertex(edge.Target);
                Graph.AddEdge(edge);
            }
            return edge;
        }

        public TVertex RemoveVertex(string name)
        {
            var vertex = GetVertex(name);
            if (vertex != null)
                Graph.RemoveVertex(vertex);
            return vertex;
        }

        public TEdge RemoveEdge(string sourceName, string targetName)
        {
            var edge = GetEdge(sourceName, targetName);
            if (edge != null)
                Graph.RemoveEdge(edge);
            return edge;
        }

        protected abstract TVertex CreateGraphVertex(string name);
        protected abstract TEdge CreateGraphEdge(TVertex source, TVertex target);

        private TVertex GetOrCreateVertex(string name)
        {
            return GetVertex(name) ?? CreateGraphVertex(name);
        }

        private TEdge GetOrCreateEdge(string sourceName, string targetName)
        {
            var edge = GetEdge(sourceName, targetName);
            if (edge != null)
                return edge;

            var source = GetOrCreateVertex(sourceName);
            var target = GetOrCreateVertex(targetName);
            return CreateGraphEdge(source, target);
        }
    }
}
