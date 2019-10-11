using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition.Layout
{
    public interface ILayoutAlgorithmSelectionStrategy
    {
        [NotNull]
        IGroupLayoutAlgorithm GetForRoot();

        [NotNull]
        IGroupLayoutAlgorithm GetForNode([NotNull] IDiagramNode node);
    }
}