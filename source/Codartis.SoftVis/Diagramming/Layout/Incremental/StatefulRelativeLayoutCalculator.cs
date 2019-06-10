using Codartis.SoftVis.Diagramming.Layout.Nodes.Layered.Sugiyama;
using Codartis.SoftVis.Diagramming.Layout.Nodes.Layered.Sugiyama.Relative;
using Codartis.SoftVis.Diagramming.Layout.Nodes.Layered.Sugiyama.Relative.Logic;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Calculates the arrangement of layout vertices relative to each other.
    /// Stateful.
    /// </summary>
    /// <remarks>
    /// Arranges vertices into layers so that:
    /// <para>all edges point "upward" (to a layer with lower index)</para>
    /// <para>all edges span exactly 2 layers (by using dummy vertices as necessary)</para>
    /// <para>vertices in all layers are ordered so that primary edges never cross.</para>
    /// </remarks>
    internal class StatefulRelativeLayoutCalculator
    {
        private readonly RelativeLayout _relativeLayout;

        public StatefulRelativeLayoutCalculator()
        {
            _relativeLayout = new RelativeLayout();
        }

        public IReadOnlyQuasiProperLayoutGraph ProperLayoutGraph => _relativeLayout.ProperLayeredLayoutGraph;
        public IReadOnlyRelativeLayout RelativeLayout => _relativeLayout;

        public void OnDiagramCleared()
        {
            _relativeLayout.Clear();
        }

        public void OnDiagramNodeAdded(DiagramNodeLayoutVertex diagramNodeLayoutVertex)
        {
            _relativeLayout.AddVertex(diagramNodeLayoutVertex);
        }

        public void OnDiagramNodeRemoved(DiagramNodeLayoutVertex diagramNodeLayoutVertex)
        {
            _relativeLayout.RemoveVertex(diagramNodeLayoutVertex);
        }

        public void OnDiagramConnectorAdded(LayoutPath layoutPath)
        {
            _relativeLayout.AddEdge(layoutPath);
        }

        public void OnDiagramConnectorRemoved(LayoutPath layoutPath)
        {
            _relativeLayout.RemoveEdge(layoutPath);
        }
    }
}