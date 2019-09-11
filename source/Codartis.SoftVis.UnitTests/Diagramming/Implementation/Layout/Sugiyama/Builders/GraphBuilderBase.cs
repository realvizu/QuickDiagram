using QuickGraph;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout.Sugiyama.Builders
{
    internal abstract class GraphBuilderBase<TVertex, TEdge, TGraph> : GraphRelatedBuilderBase<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : IMutableBidirectionalGraph<TVertex, TEdge>, new()
    {
        public override TGraph Graph { get; }

        protected GraphBuilderBase()
        {
            Graph = new TGraph();
        }

        public override TVertex AddVertex(string name, int priority = 1)
        {
            var vertex = GetOrCreateVertex(name, priority);
            if (vertex != null)
                Graph.AddVertex(vertex);
            return vertex;
        }

        public override TEdge AddEdge(string sourceName, string targetName)
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

        public override TVertex RemoveVertex(string name)
        {
            var vertex = GetVertex(name);
            if (vertex != null)
                Graph.RemoveVertex(vertex);
            return vertex;
        }

        public override TEdge RemoveEdge(string sourceName, string targetName)
        {
            var edge = GetEdge(sourceName, targetName);
            if (edge != null)
                Graph.RemoveEdge(edge);
            return edge;
        }
    }
}
