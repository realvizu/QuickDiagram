using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic
{
    /// <summary>
    /// Calculates the arrangement of layout vertices relative to each other.
    /// </summary>
    /// <remarks>
    /// Arranges vertices into layers so that:
    /// <para>all edges point "upward" (to a layer with lower index)</para>
    /// <para>all edges span exactly 2 layers (by using dummy vertices as necessary)</para>
    /// <para>vertices in all layers ar ordered so that primary edges never cross.</para>
    /// </remarks>
    public class RelativeLayoutCalculator
    {
        private readonly LayeredLayoutGraph _layoutGraph;
        private readonly LayoutVertexLayers _layers;
        private readonly RelativeLayout _relativeLayout;

        private readonly RelativeLocationCalculator _locationCalculator;

        public RelativeLayoutCalculator()
        {
            _layoutGraph = new LayeredLayoutGraph();
            _layers = new LayoutVertexLayers();
            _relativeLayout = new RelativeLayout(_layoutGraph, _layers);

            _locationCalculator = new RelativeLocationCalculator(
                _relativeLayout.ProperLayeredLayoutGraph, _relativeLayout.LayoutVertexLayers);
        }

        public IReadOnlyQuasiProperLayoutGraph ProperLayoutGraph => _layoutGraph.ProperGraph;
        public IReadOnlyRelativeLayout RelativeLayout => _relativeLayout;

        public void OnDiagramCleared()
        {
            _layers.Clear();
            _layoutGraph.Clear();
        }

        public void OnDiagramNodeAdded(DiagramNodeLayoutVertex diagramNodeLayoutVertex)
        {
            _layoutGraph.AddVertex(diagramNodeLayoutVertex);
            SetLocation(diagramNodeLayoutVertex);
        }

        public void OnDiagramNodeRemoved(DiagramNodeLayoutVertex diagramNodeLayoutVertex)
        {
            _layers.RemoveVertex(diagramNodeLayoutVertex);
            _layoutGraph.RemoveVertex(diagramNodeLayoutVertex);
        }

        public void OnDiagramConnectorAdded(LayoutPath layoutPath)
        {
            GetAffectedVertices(layoutPath).ForEach(RemoveFromLayers);

            _layoutGraph.AddEdge(layoutPath);

            GetAffectedVertices(layoutPath).OrderBy(ProperLayoutGraph.GetLayerIndex).ForEach(SetLocation);
        }

        public void OnDiagramConnectorRemoved(LayoutPath layoutPath)
        {
            GetAffectedVertices(layoutPath).ForEach(RemoveFromLayers);

            _layoutGraph.RemoveEdge(layoutPath);

            GetAffectedVertices(layoutPath).OrderBy(ProperLayoutGraph.GetLayerIndex).ForEach(SetLocation);
        }

        private IEnumerable<LayoutVertexBase> GetAffectedVertices(LayoutPath layoutPath)
        {
            var diagramNodeVertices = _layoutGraph.GetVertexAndDescendants(layoutPath.PathSource).ToList();
            var dummyVertices = diagramNodeVertices.SelectMany(i => _layoutGraph.OutEdges(i))
                .SelectMany(i => i.InterimVertices);
            return diagramNodeVertices.Concat((IEnumerable<LayoutVertexBase>)dummyVertices);
        }

        private void RemoveFromLayers(LayoutVertexBase vertex)
        {
            if (ProperLayoutGraph.ContainsVertex(vertex))
                _layers.RemoveVertex(vertex);
        }

        private void SetLocation(LayoutVertexBase vertex)
        {
            if (!ProperLayoutGraph.ContainsVertex(vertex))
                return;

            var targetLocation = _locationCalculator.GetTargetLocation(vertex);
            _layers.AddVertex(vertex, targetLocation);
        }
    }
}