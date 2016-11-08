using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative
{
    /// <summary>
    /// Provides a read-only view of a layered graph used for layout calculation.
    /// </summary>
    internal interface IReadOnlyLayeredLayoutGraph : IReadOnlyLayeredGraph<DiagramNodeLayoutVertex, LayoutPath>
    {
    }
}