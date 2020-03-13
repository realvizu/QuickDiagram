using System.Collections.Generic;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UI
{
    public interface IMiniButtonFactory
    {
        [NotNull]
        [ItemNotNull]
        IEnumerable<IMiniButton> CreateForDiagramShape([NotNull] IDiagramShapeUi diagramShapeUi);
    }
}