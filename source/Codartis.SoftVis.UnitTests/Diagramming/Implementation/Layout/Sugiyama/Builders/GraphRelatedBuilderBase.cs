using System.Linq;
using Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout.Sugiyama.Helpers;
using QuickGraph;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout.Sugiyama.Builders
{
    internal abstract class GraphRelatedBuilderBase<TVertex, TEdge, TGraph> : BuilderBase
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : IMutableBidirectionalGraph<TVertex, TEdge>, new()
    {
        public abstract TGraph Graph { get; }

        protected abstract TVertex CreateVertex(string name, int priority = 1);
        protected abstract TEdge CreateEdge(TVertex source, TVertex target);
        public abstract TVertex AddVertex(string name, int priority = 1);
        public abstract TEdge AddEdge(string sourceName, string targetName);
        public abstract TVertex RemoveVertex(string name);
        public abstract TEdge RemoveEdge(string sourceName, string targetName);

        protected virtual PathSpecification GetPathSpecification(string pathString)
        {
            return PathSpecification.Parse(pathString);
        }

        public void SetUp(params string[] pathStrings)
        {
            foreach (var pathString in pathStrings)
            {
                var pathSpecification = GetPathSpecification(pathString);

                foreach (var vertexName in pathSpecification)
                    AddVertex(vertexName);

                foreach (var edgeSpecification in pathSpecification.ToEdgeSpecifications().Reverse())
                    AddEdge(edgeSpecification.SourceVertexName, edgeSpecification.TargetVertexName);
            }
        }

        public TVertex GetVertex(string name)
        {
            return Graph.Vertices.FirstOrDefault(i => i.ToString() == name);
        }

        public TEdge GetEdge(string edgeString)
        {
            var edgeSpecification = EdgeSpecification.Parse(edgeString);
            return GetEdge(edgeSpecification.SourceVertexName, edgeSpecification.TargetVertexName);
        }

        public TEdge GetEdge(string sourceName, string targetName)
        {
            return Graph.Edges.FirstOrDefault(i => i.Source.Equals(GetVertex(sourceName)) && i.Target.Equals(GetVertex(targetName)));
        }

        public TEdge AddEdge(string edgeString)
        {
            var edgeSpec = EdgeSpecification.Parse(edgeString);
            return AddEdge(edgeSpec.SourceVertexName, edgeSpec.TargetVertexName);
        }

        public TEdge RemoveEdge(string edgeString)
        {
            var edgeSpec = EdgeSpecification.Parse(edgeString);
            return RemoveEdge(edgeSpec.SourceVertexName, edgeSpec.TargetVertexName);
        }

        protected TVertex GetOrCreateVertex(string name, int priority = 1)
        {
            return GetVertex(name) ?? CreateVertex(name, priority);
        }

        protected TEdge GetOrCreateEdge(string sourceName, string targetName)
        {
            var edge = GetEdge(sourceName, targetName);
            if (edge != null)
                return edge;

            var source = GetOrCreateVertex(sourceName);
            var target = GetOrCreateVertex(targetName);
            return CreateEdge(source, target);
        }
    }
}
