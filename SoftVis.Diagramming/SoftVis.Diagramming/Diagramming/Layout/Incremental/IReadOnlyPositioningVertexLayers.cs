using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// A read-only view of the positioning vertex layers.
    /// </summary>
    internal interface IReadOnlyPositioningVertexLayers : IEnumerable<IReadOnlyPositioningVertexLayer>
    {
    }
}
