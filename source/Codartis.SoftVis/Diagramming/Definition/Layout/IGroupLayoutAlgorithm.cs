using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition.Layout
{
    /// <summary>
    /// Calculates node positions and connector routes for a layout group.
    /// </summary>
    public interface IGroupLayoutAlgorithm
    {
        [Pure]
        LayoutInfo Calculate([NotNull] ILayoutGroup layoutGroup);
    }
}