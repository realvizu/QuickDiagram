using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Abstraction for the UI of a diagram shape.
    /// </summary>
    public interface IDiagramShapeUi : ICloneable
    {
        [NotNull] string Stereotype { get; }

        IEnumerable<IMiniButton> CreateMiniButtons();
    }
}