using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition.Layout
{
    /// <summary>
    /// Calculates diagram node and connector layout.
    /// </summary>
    public interface ILayoutAlgorithm
    {
        [Pure]
        LayoutSpecification Calculate([NotNull] ILayoutGroup layoutGroup);
    }
}