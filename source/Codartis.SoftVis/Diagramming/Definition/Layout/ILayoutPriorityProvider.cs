using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition.Layout
{
    /// <summary>
    /// Provides layout priority information about diagram nodes.
    /// </summary>
    public interface ILayoutPriorityProvider
    {
        int GetPriority([NotNull] IDiagramNode diagramNode);
    }
}