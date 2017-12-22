using System;
using Codartis.SoftVis.Util.UI.Wpf.ViewModels;

namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Abstraction of a MiniButton that can be placed on a diagram shape.
    /// </summary>
    public interface IMiniButton : IDecoratorViewModel<IDiagramShapeUi>, IDisposable
    {
    }
}
