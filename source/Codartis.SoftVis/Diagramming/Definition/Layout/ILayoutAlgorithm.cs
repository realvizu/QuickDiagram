using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition.Layout
{
    public interface ILayoutAlgorithm
    {
        LayoutSpecification Calculate([NotNull] ILayoutGroup layoutGroup);
    }
}