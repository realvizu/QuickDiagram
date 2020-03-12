using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;
using Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama;
using Codartis.SoftVis.Diagramming.Implementation.Layout.Vertical;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout.Selection
{
    public sealed class LayoutAlgorithmSelectionStrategy : ILayoutAlgorithmSelectionStrategy
    {
        [NotNull] private readonly SugiyamaLayoutAlgorithm _sugiyamaLayoutAlgorithm;
        [NotNull] private readonly VerticalNodeLayoutAlgorithm _verticalNodeLayoutAlgorithm;

        public LayoutAlgorithmSelectionStrategy(
            [NotNull] SugiyamaLayoutAlgorithm sugiyamaLayoutAlgorithm,
            [NotNull] VerticalNodeLayoutAlgorithm verticalNodeLayoutAlgorithm)
        {
            _sugiyamaLayoutAlgorithm = sugiyamaLayoutAlgorithm;
            _verticalNodeLayoutAlgorithm = verticalNodeLayoutAlgorithm;
        }

        public IGroupLayoutAlgorithm GetForRoot() => _sugiyamaLayoutAlgorithm;

        public IGroupLayoutAlgorithm GetForNode(IDiagramNode node) => _verticalNodeLayoutAlgorithm;
    }
}