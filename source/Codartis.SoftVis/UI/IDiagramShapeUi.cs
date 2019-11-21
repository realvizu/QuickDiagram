using System.Collections.Generic;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Abstraction for the UI of a diagram shape.
    /// </summary>
    public interface IDiagramShapeUi
    {
        [NotNull] string StereotypeName { get; }

        [NotNull]
        [ItemNotNull]
        IEnumerable<IMiniButton> CreateMiniButtons();
    }
}