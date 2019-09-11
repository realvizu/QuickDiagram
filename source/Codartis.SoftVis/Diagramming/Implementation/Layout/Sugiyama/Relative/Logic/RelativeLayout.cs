using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama.Relative.Logic
{
    /// <summary>
    /// Summarizes the data structures that make up the relative layout and provides operations on them.
    /// </summary>
    internal sealed class RelativeLayout : IReadOnlyRelativeLayout
    {
        private readonly LayeredLayoutGraph _layeredLayoutGraph;
        private readonly LayoutVertexLayers _layers;
        private readonly RelativeLocationCalculator _relativeLocationCalculator;

        public RelativeLayout()
        {
            _layeredLayoutGraph = new LayeredLayoutGraph();
            _layers = new LayoutVertexLayers();
            _relativeLocationCalculator = new RelativeLocationCalculator(ProperLayeredLayoutGraph, LayoutVertexLayers);
        }

        public IReadOnlyLayeredLayoutGraph LayeredLayoutGraph => _layeredLayoutGraph;
        public IReadOnlyQuasiProperLayoutGraph ProperLayeredLayoutGraph => _layeredLayoutGraph.ProperGraph;
        public IReadOnlyLayoutVertexLayers LayoutVertexLayers => _layers;

        public void AddVertex(DiagramNodeLayoutVertex vertex)
        {
            _layeredLayoutGraph.AddVertex(vertex);
            SetLocation(vertex);
        }

        public void RemoveVertex(DiagramNodeLayoutVertex vertex)
        {
            _layers.RemoveVertex(vertex);
            _layeredLayoutGraph.RemoveVertex(vertex);
        }

        public void AddEdge(LayoutPath edge)
        {
            GetAffectedVertices(edge).ForEach(RemoveFromLayers);
            _layeredLayoutGraph.AddEdge(edge);
            GetAffectedVertices(edge).OrderBy(ProperLayeredLayoutGraph.GetLayerIndex).ForEach(SetLocation);
        }

        public void RemoveEdge(LayoutPath layoutPath)
        {
            GetAffectedVertices(layoutPath).ForEach(RemoveFromLayers);
            _layeredLayoutGraph.RemoveEdge(layoutPath);
            GetAffectedVertices(layoutPath).OrderBy(ProperLayeredLayoutGraph.GetLayerIndex).ForEach(SetLocation);
        }

        public void RemoveFromLayers(LayoutVertexBase vertex)
        {
            if (ProperLayeredLayoutGraph.ContainsVertex(vertex))
                _layers.RemoveVertex(vertex);
        }

        public void SetLocation(LayoutVertexBase vertex)
        {
            if (!ProperLayeredLayoutGraph.ContainsVertex(vertex))
                return;

            var targetLocation = _relativeLocationCalculator.GetTargetLocation(vertex);
            _layers.AddVertex(vertex, targetLocation);
        }

        public IEnumerable<LayoutVertexBase> GetAffectedVertices(LayoutPath layoutPath)
        {
            var diagramNodeVertices = _layeredLayoutGraph.GetVertexAndDescendants(layoutPath.PathSource).ToList();
            var dummyVertices = diagramNodeVertices.SelectMany(i => _layeredLayoutGraph.OutEdges(i)).SelectMany(i => i.InterimVertices);
            return diagramNodeVertices.Concat((IEnumerable<LayoutVertexBase>) dummyVertices);
        }

        public void Clear()
        {
            _layers.Clear();
            _layeredLayoutGraph.Clear();
        }
    }
}