using System.Collections.Generic;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition
{
    /// <summary>
    /// Can order diagram nodes.
    /// </summary>
    public interface IDiagramNodeOrderProvider
    {
        [NotNull]
        [ItemNotNull]
        IEnumerable<IDiagramNode> OrderNodes([NotNull] [ItemNotNull] IEnumerable<IDiagramNode> diagramNodes);
    }
}