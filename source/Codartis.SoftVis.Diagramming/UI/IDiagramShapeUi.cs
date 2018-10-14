using System;
using System.Collections.Generic;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Abstraction for the UI of a diagram shape.
    /// </summary>
    public interface IDiagramShapeUi : ICloneable
    {
        IEnumerable<IMiniButton> CreateMiniButtons();
    }
}
